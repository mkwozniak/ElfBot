using System;
using System.Diagnostics;
using System.Windows.Threading;
using ElfBot.Util;
using System.Linq;

#pragma warning disable CS4014

namespace ElfBot;

/// <summary>
/// Enumeration of possible statuses that the auto-combat system can be in.
/// </summary>
public enum AutoCombatStatus
{
	Inactive,
	Starting,
	Summoning,
	Buffing,
	Targeting,
	Attacking,
	Looting,
}

/// <summary>
/// Auto-combat state machine. Handles the status and actions
/// of combat when enabled.
/// </summary>
public sealed class AutoCombat
{
	public static readonly Random Random = new();

	private readonly ApplicationContext _context;
	private readonly AutoCombatState _state;

	internal CombatOptions CombatOptions => _context.Settings.CombatOptions;

	private readonly DispatcherTimer _autoCombatTimer = new()
	{
		Interval = TimeSpan.FromMilliseconds(250)
	};

	public AutoCombat(ApplicationContext context)
	{
		_context = context;
		_autoCombatTimer.Tick += Tick;
		_state = new AutoCombatState(this);
	}

	public AutoCombatState State => _state;

	/// <summary>
	/// Starts the automation of combat.
	/// </summary>
	public void Start()
	{
		_state.Reset();
		_state.ChangeStatus(AutoCombatStatus.Starting);
		_autoCombatTimer.Start();
	}

	/// <summary>
	/// Stops the automation of combat.
	/// </summary>
	public void Stop()
	{
		CombatOptions.AutoCombatEnabled = false;
		_state.Reset();
		_state.ClearHotkeyCooldowns();
		_state.LastBuffTime = null;
		_autoCombatTimer.Stop();
	}

	/// <summary>
	/// Master heartbeat method to handle the movement of state.
	/// </summary>
	private void Tick(object? sender, EventArgs e)
	{
		if (_context.ActiveCharacter == null
		    || !CombatOptions.AutoCombatEnabled
		    || _state.Status == AutoCombatStatus.Inactive)
		{
			Trace.WriteLine("Canceled auto-combat due to invalid state");
			MainWindow.Logger.Warn("Auto-combat disabled, please ensure that ROSE is hooked");
			Stop();
			return;
		}

		if (_context.ActiveCharacter.IsDead)
		{
			if (_context.Settings.GeneralOptions.DeathAction != DeathActions.CANCEL_TIMERS) return;
			Trace.WriteLine("Canceled auto-combat due to player death");
			MainWindow.Logger.Warn("Disabling auto-combat due to player death");
			Stop();
			return;
		}

		if (_state.isOnCooldown())
		{
			return;
		}

		try
		{
			_ = _state.Status switch
			{
				AutoCombatStatus.Starting => _start(),
				AutoCombatStatus.Summoning => _summons(),
				AutoCombatStatus.Buffing => _buff(),
				AutoCombatStatus.Targeting => _findTarget(),
				AutoCombatStatus.Attacking => _attack(),
				AutoCombatStatus.Looting => _loot(),
				_ => true
			};
		}
		catch (Exception ex)
		{
			MainWindow.Logger.Error($"An exception occurred when attempting to process state {_state.Status}");
			MainWindow.Logger.Error($"Disabling auto-combat");
			MainWindow.Logger.Error(ex.Message);
			if (ex.StackTrace != null)
			{
				MainWindow.Logger.Error(ex.StackTrace);
			}

			Stop();
		}
	}

	private bool _start()
	{
		_state.Reset();

		if (_context.Settings.GeneralOptions.SummonsEnabled && _canSummon())
		{
			_state.ChangeStatus(AutoCombatStatus.Summoning);
			return true;
		}

		if (_state.CanApplyBuffs())
		{
			_state.ChangeStatus(AutoCombatStatus.Buffing);
			return true;
		}

		_state.ChangeStatus(AutoCombatStatus.Targeting);
		return true;
	}

	private bool _summons()
	{
		if (!_canSummon())
		{
			_state.Reset();
			if (CombatOptions.BuffsEnabled) _state.ChangeStatus(AutoCombatStatus.Buffing);
			else _state.ChangeStatus(AutoCombatStatus.Targeting);
			return true;
		}

		// Select a random slot to attack/skill from and then go on cooldown for a little bit.
		var activeSummonKeys = _context.Settings.FindKeybindings(KeybindAction.Summon);
		var chosenKey = activeSummonKeys[0];
		if (_state.isHotkeyOnCooldown(chosenKey))
		{
			Trace.WriteLine("Attempted summon was on cooldown");
			MainWindow.Logger.Warn("Attempted summon was on cooldown");
			return false;
		}

		RoseProcess.SendKeypress(chosenKey.KeyCode, chosenKey.IsShift);
		_state.SetHotkeyCooldown(chosenKey, TimeSpan.FromSeconds(chosenKey.Cooldown + 0.1f));
		_state.SetCooldown(TimeSpan.FromSeconds(2)); // Wait for animation
		Trace.WriteLine($"Running summon in slot {chosenKey.Key} by pressing keycode {chosenKey.KeyCode}. ");
		return true;
	}

	private bool _canSummon()
	{
		if (_context.Settings.FindKeybindings(KeybindAction.Summon).Count == 0) return false;
		var currentSummonMeter = _context.ActiveCharacter!.ConsumedSummonsMeter;
		var summonCost = _context.Settings.GeneralOptions.SummonCost;
		var maxSummons = _context.Settings.GeneralOptions.MaxSummonCount;
		return currentSummonMeter + summonCost <= maxSummons;
	}

	private bool _isAttackingPlayer(TargetedEntity entity)
	{
		return entity.IsAttacking && entity.ActiveObjectId == _context.ActiveCharacter!.Id;
	}

	private bool _isAttackingParty(TargetedEntity entity)
	{
		var party = _context.ActiveCharacter!.Party;
		return party.IsInParty
		       && entity.IsAttacking
		       && party.PartyMembers.Any(pm => pm.Id == entity.ActiveObjectId);
	}

	private bool _isPriority(TargetedEntity entity)
	{
		var entry = _context.MonsterTable.SingleOrDefault(v => v.Name == entity.Name);
		return entry is { Priority: true };
	}

	/// <summary>
	/// Checks to see if a target has been selected and moves
	/// to start attacking the monster if it is part of the
	/// monster table.
	/// </summary>
	/// <returns>true if attacking will start</returns>
	private bool _findTarget()
	{
		var monsters = GameObjects.GetVisibleMonsters()
			.Where(t => t.IsValid() && !t.IsDead)
			.Where(t => t.Name != "Bonfire" 
			            && t.Name != "Salamander Flame"
			            && t.Name != "Mana Flame")
			.Where(t => CombatOptions.MaximumAttackDistance == 0
			            || _context.ActiveCharacter!.GetDistanceTo(t) <= CombatOptions.MaximumAttackDistance)
			.OrderByDescending(t => _context.ActiveCharacter!.GetDistanceTo(t))
			.ToArray();

		if (monsters.Length == 0)
		{
			return false;
		}

		TargetedEntity? target = null;

		// The first priority is a monster that is attacking the current player, or
		// is attacking a party member.
		foreach (var monster in monsters)
		{
			// We don't care if the monster is in the mob list at this point,
			// the goal is to not let the player die, so we omit the mob table.
			if (_isAttackingPlayer(monster))
			{
				Trace.WriteLine("Found a monster attacking the current player");
				target = monster;
				break;
			}

			if (_isAttackingParty(monster))
			{
				Trace.WriteLine("Found a monster attacking a party member");
				target = monster;
			}
		}

		// If we haven't yet found a monster lets reference the priority list
		// and see if anything can be attacked - otherwise we opt for any monster.
		if (target == null)
		{
			var inMobTable = monsters.Where(m => _context.MonsterTable.Any(v => v.Name == m.Name)).ToArray();
			if (inMobTable.Length == 0)
			{
				return false;
			}
			foreach (var entry in inMobTable)
			{
				target = entry;
				if (_isPriority(entry)) break;
			}
		}

		// We check again and proceed if a mob has been found to start attacking
		if (target == null)
		{
			Trace.WriteLine("Did not find a target");
			return false;
		}
		Trace.WriteLine($"Found monster {target.Id:x4} ({target.Name}) to attack");
		_context.ActiveCharacter!.LastTargetId = target.Id;
		_state.CurrentTargetId = target.Id;
		_state.CurrentTarget = target.Name; 
		_state.ChangeStatus(AutoCombatStatus.Attacking);
		if (CombatOptions.DelayBeforeAttack > 0)
		{
			_state.SetCooldown(TimeSpan.FromSeconds(CombatOptions.DelayBeforeAttack));
		}

		return true;
	}

	/// <summary>
	/// Attacks the currently selected monster. 
	/// </summary>
	/// <returns>whether an attack will be performed</returns>
	private bool _attack()
	{
		if (_context.ActiveCharacter?.TargetEntity == null)
		{
			Trace.WriteLine("Canceling attack due to expiration or invalid target.");
			_state.Reset();
			_state.ChangeStatus(AutoCombatStatus.Starting);
			_state.SetCooldown(TimeSpan.FromSeconds(3));
			return false;
		}

		// Player XP is checked to determine whether the monster has died. This
		// is the best current method we have, and in the future this will change
		// to checking the monsters HP or alive status. When the monster has died,
		// we need to move into either looting or restart the cycle.
		if (_context.ActiveCharacter.TargetEntity?.Hp <= 0)
		{
			Trace.WriteLine($"Target entity is dead");
			_context.ActiveCharacter.LastTargetId = 0;

			if (_context.Settings.LootOptions.LootAfterCombatEnabled)
			{
				var lootTimeSeconds = _context.Settings.LootOptions.Duration;
				_state.ChangeStatus(AutoCombatStatus.Looting, TimeSpan.FromSeconds(lootTimeSeconds));
			}
			else
			{
				_state.Reset();
				_state.ChangeStatus(AutoCombatStatus.Starting);
			}

			return false;
		}

		var activeCombatKeys = _context.Settings.FindKeybindings(KeybindAction.Attack, KeybindAction.Skill);
		if (activeCombatKeys.Count == 0)
		{
			Trace.WriteLine("No attack keys have been set");
			MainWindow.Logger.Warn("Tried to attack, but no keys are set");
			return false;
		}

		var notOnCooldown = activeCombatKeys.FindAll(hk => !_state.isHotkeyOnCooldown(hk));
		if (notOnCooldown.Count == 0)
		{
			return false;
		}

		// Select a random slot to attack/skill from and then go on cooldown for a little bit.
		var randomKeyIndex = Random.Next(0, notOnCooldown.Count);
		var chosenKey = notOnCooldown[randomKeyIndex];

		if (chosenKey.Cooldown > 0)
		{
			_state.SetHotkeyCooldown(chosenKey, TimeSpan.FromSeconds(chosenKey.Cooldown + 0.1f));
		}

		_context.ActiveCharacter.LastTargetId = _state.CurrentTargetId;
		RoseProcess.SendKeypress(chosenKey.KeyCode, chosenKey.IsShift);
		Trace.WriteLine($"Running skill in slot {chosenKey.Key} by pressing keycode {chosenKey.KeyCode}. ");
		_state.SetCooldown(TimeSpan.FromMilliseconds(250)); // Wait 250ms before next attack
		return true;
	}

	/// <summary>
	/// Attempts to loot items by pressing the 'T' key.
	/// </summary>
	/// <returns>whether the loot key was pressed</returns>
	private bool _loot()
	{
		if (_state.IsExpired())
		{
			_state.Reset();
			_state.ChangeStatus(AutoCombatStatus.Starting);
			return false;
		}

		RoseProcess.SendKeypress(Messaging.VKeys.KEY_T);
		_state.SetCooldown(TimeSpan.FromMilliseconds(250));
		Trace.WriteLine("Sent 'T' keypress to loot, waiting 250ms");
		return true;
	}

	private bool _buff()
	{
		var activeBuffKeys = _context.Settings.FindKeybindings(KeybindAction.Buff);
		if (activeBuffKeys.Count == 0)
		{
			Trace.WriteLine("No buff keys have been set");
			MainWindow.Logger.Warn("Tried to buff, but no keys are set");
			_state.Reset();
			_state.ChangeStatus(AutoCombatStatus.Targeting);
			return false;
		}

		// Select a random slot to attack/skill from and then go on cooldown for a little bit.
		var nextKey = _state.CurrentCastingBuff;
		var chosenKey = activeBuffKeys[nextKey];
		if (_state.isHotkeyOnCooldown(chosenKey))
		{
			Trace.WriteLine("Attempted buff was on cooldown");
			MainWindow.Logger.Warn("Attempted buff was on cooldown");
			return false;
		}

		RoseProcess.SendKeypress(chosenKey.KeyCode, chosenKey.IsShift);
		_state.SetHotkeyCooldown(chosenKey, TimeSpan.FromSeconds(chosenKey.Cooldown + 0.1f));
		Trace.WriteLine($"Running buff in slot {chosenKey.Key} by pressing keycode {chosenKey.KeyCode}. ");
		_state.CurrentCastingBuff++;
		if (_state.CurrentCastingBuff >= activeBuffKeys.Count)
		{
			_state.LastBuffTime = DateTime.Now;
			_state.Reset();
			_state.ChangeStatus(AutoCombatStatus.Targeting);
		}
		else
		{
			_state.SetCooldown(TimeSpan.FromSeconds(2));
		}

		return true;
	}
}

public sealed class AutoCombatState : PropertyNotifyingClass
{
	private readonly AutoCombat _autoCombat;
	private string? _currentTarget;
	private int _currentTargetId; // 0 indicates there is no active target
	private AutoCombatStatus _status = AutoCombatStatus.Inactive;
	private HotkeyCooldownTracker _hotkeyCooldowns = new();

	public AutoCombatState(AutoCombat autoCombat)
	{
		_autoCombat = autoCombat;
	}

	public AutoCombatStatus Status
	{
		get => _status;
		private set
		{
			if (_status == value) return;
			_status = value;
			NotifyPropertyChanged();
		}
	}

	private DateTime? StatusTimeout { get; set; }
	private DateTime? Cooldown { get; set; }

	/// <summary>
	/// Tracks the last time buffs were applied.
	/// </summary>
	public DateTime? LastBuffTime { get; set; }

	/// <summary>
	/// During buffing, tracks the current buff being cast.
	///
	/// As auto-combat ticks, it increments the buff count so that each
	/// buff keybind is ran only 1 time.
	/// </summary>
	public int CurrentCastingBuff { get; set; }

	public string? CurrentTarget // might not need this
	{
		get => _currentTarget;
		set
		{
			if (_currentTarget == value) return;
			_currentTarget = value;
			NotifyPropertyChanged();
		}
	}

	public int CurrentTargetId
	{
		get => _currentTargetId;
		set
		{
			if (_currentTargetId == value) return;
			_currentTargetId = value;
			NotifyPropertyChanged();
		}
	}

	/// <summary>
	/// Changes the current status to a new one, optionally
	/// with a specified duration.
	/// </summary>
	/// <param name="status">New status</param>
	/// <param name="duration">The maximum amount of time to stay in the new state for</param>
	public void ChangeStatus(AutoCombatStatus status, TimeSpan? duration = null)
	{
		Trace.WriteLine($"Auto-combat status changed to {status}");
		Status = status;
		StatusTimeout = duration == null ? null : DateTime.Now.Add(duration.Value);
		Cooldown = null;
	}

	/// <summary>
	/// Sets a cooldown period until the next time the
	/// current state can be processed.
	/// </summary>
	/// <param name="duration">cooldown duration</param>
	public void SetCooldown(TimeSpan duration)
	{
		Cooldown = DateTime.Now.Add(duration);
	}

	/// <summary>
	/// Returns true if the current state is on a cooldown.
	/// </summary>
	/// <returns>cooldown state</returns>
	public bool isOnCooldown()
	{
		return Cooldown != null && !_isDateInPast(Cooldown);
	}

	/// <summary>
	/// Marks a hotkey as being on cooldown for a specified duration.
	/// </summary>
	/// <param name="slot">hotkey slot</param>
	/// <param name="duration">cooldown duration</param>
	public void SetHotkeyCooldown(HotkeySlot slot, TimeSpan duration)
	{
		_hotkeyCooldowns.SetCooldown(slot, duration);
	}

	/// <summary>
	/// Returns true if a hotkey slot is currently on cooldown.
	/// </summary>
	/// <param name="slot">hotkey slot</param>
	/// <returns>hotkey cooldown status</returns>
	public bool isHotkeyOnCooldown(HotkeySlot slot)
	{
		return _hotkeyCooldowns.isOnCooldown(slot);
	}

	/// <summary>
	/// Clears all active hotkey cooldowns.
	/// </summary>
	public void ClearHotkeyCooldowns()
	{
		_hotkeyCooldowns.Clear();
	}

	/// <summary>
	/// Resets all properties tracked by the state.
	/// </summary>
	public void Reset()
	{
		ChangeStatus(AutoCombatStatus.Inactive);
		ResetTarget();
		CurrentCastingBuff = 0;
		Cooldown = null;
	}

	/// <summary>
	/// Resets the last target selected by the user.
	/// </summary>
	public void ResetTarget()
	{
		_currentTarget = null;
		_currentTargetId = 0;
	}

	/// <summary>
	/// Returns true if buffs are able to be applied.
	/// </summary>
	/// <returns>buff readiness status</returns>
	public bool CanApplyBuffs()
	{
		return _autoCombat.CombatOptions.BuffsEnabled
			&& LastBuffTime == null || _isDateInPast(LastBuffTime?.AddSeconds(_autoCombat.CombatOptions.BuffFrequency));
	}

	/// <summary>
	/// Returns true if this state has timed out. If the state
	/// does not have a timeout set, this method will always
	/// return false.
	/// </summary>
	/// <returns>expiry/timeout status</returns>
	public bool IsExpired()
	{
		return _isDateInPast(StatusTimeout);
	}

	private static bool _isDateInPast(DateTime? time)
	{
		var now = DateTime.Now;
		return time != null && now.CompareTo(time) > 0;
	}
}