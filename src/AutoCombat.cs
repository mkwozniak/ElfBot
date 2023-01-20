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
	CheckTarget,
	StartAttack,
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
		Interval = TimeSpan.FromMilliseconds(50)
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
		    || _state.Status == AutoCombatStatus.Inactive
		    || _context.MonsterTable.Count == 0)
		{
			Trace.WriteLine("Canceled auto-combat due to invalid state");
			MainWindow.Logger.Warn("Auto-combat disabled, please ensure that ROSE is hooked and that " +
			                       "a monster table is loaded");
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
				AutoCombatStatus.Targeting => _selectNewTarget(),
				AutoCombatStatus.CheckTarget => _checkTarget(),
				AutoCombatStatus.StartAttack => _prepareAttacking(),
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
			return;
		}

		Trace.WriteLine($"Auto-combat heartbeat completed (status: {_state.Status}");
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

		if (CombatOptions.PriorityTargetScan
		    && _context.MonsterTable.Any(x => x.Priority))
		{
			_state.PriorityCheckCount = 0;
			_state.ScanningForPriority = true;
		}

		_state.ChangeStatus(AutoCombatStatus.CheckTarget, TimeSpan.FromMilliseconds(250));
		if (CombatOptions.TargetCheckDelay > 0)
		{
			_state.SetCooldown(TimeSpan.FromMilliseconds(CombatOptions.TargetCheckDelay));
		}

		return true;
	}

	private bool _summons()
	{
		if (!_canSummon())
		{
			_state.Reset();
			if (CombatOptions.BuffsEnabled) _state.ChangeStatus(AutoCombatStatus.Buffing);
			else _state.ChangeStatus(AutoCombatStatus.CheckTarget, TimeSpan.FromMilliseconds(100));
			return true;
		}

		var activeSummonKeys = _context.Settings.FindKeybindings(KeybindAction.Summon);
		if (activeSummonKeys.Count == 0)
		{
			Trace.WriteLine("No summon keys have been set");
			MainWindow.Logger.Warn("Tried to summon, but no keys are set");
			_state.Reset();
			_state.ChangeStatus(AutoCombatStatus.CheckTarget, TimeSpan.FromMilliseconds(100));
			return false;
		}

		// Select a random slot to attack/skill from and then go on cooldown for a little bit.
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
		var currentSummonMeter = _context.ActiveCharacter!.ConsumedSummonsMeter;
		Trace.Write($"Current summon meter {currentSummonMeter}");
		var summonCost = _context.Settings.GeneralOptions.SummonCost;
		var maxSummons = _context.Settings.GeneralOptions.MaxSummonCount;
		return currentSummonMeter + summonCost <= maxSummons;
	}

	/// <summary>
	/// Attempts to select a new target monster by pressing the TAB key.
	/// </summary>
	/// <returns>whether the tab key was pressed</returns>
	private bool _selectNewTarget()
	{
		_state.ResetTarget();
		RoseProcess.SendKeypress(Messaging.VKeys.KEY_TAB);
		Trace.WriteLine("Sent tab key press to simulator to attempt selecting a new target");
		_state.ChangeStatus(AutoCombatStatus.CheckTarget, TimeSpan.FromMilliseconds(250));
		if (CombatOptions.TargetCheckDelay > 0)
		{
			_state.SetCooldown(TimeSpan.FromMilliseconds(CombatOptions.TargetCheckDelay));
		}
		return true;
	}

	/// <summary>
	/// Checks to see if a target has been selected and moves
	/// to start attacking the monster if it is part of the
	/// monster table.
	/// </summary>
	/// <returns>true if attacking will start</returns>
	private bool _checkTarget()
	{
		if (_state.IsExpired()) // if the mob selection has taken too long, attempt to find a new monster
		{
			Trace.WriteLine("Target selection expired");
			_state.ResetTarget();
			_state.ChangeStatus(AutoCombatStatus.Targeting);
			return false;
		}

		var name = _context.ActiveCharacter!.TargetName;
		var id = _context.ActiveCharacter.LastTargetId;
		Trace.WriteLine($"Target name is {name} with id {id}");

		// If a target is not yet found, we may need to wait more time
		// for the addresses to update. Otherwise, we may time out and 
		// attempt to select a new monster.
		if (id == 0 || name == null) return false;

		_state.CurrentTargetId = id;
		_state.CurrentTarget = name;

		var target = new TargetedEntity(id);
		if (_context.ActiveCharacter.GetDistanceTo(target) > CombatOptions.MaximumAttackDistance)
		{
			_state.ResetTarget();
			_state.ChangeStatus(AutoCombatStatus.Targeting);
			return false;
		}

		var monsterTableEntry = _context.MonsterTable.SingleOrDefault(v => v.Name == name.Trim());

		if (CombatOptions.PriorityTargetScan && _state.ScanningForPriority)
		{
			if (_state.PriorityCheckCount > CombatOptions.MaxPriorityChecks)
			{
				Trace.WriteLine("Priority target selection expired");
				_state.ScanningForPriority = false;
				_context.ActiveCharacter.ResetTargetMemory();
				_state.ResetTarget();
				_state.ChangeStatus(AutoCombatStatus.Targeting);
				return false;
			}

			if (!monsterTableEntry?.Priority == true)
			{
				Trace.WriteLine("Priority Monster name not in table");
				_state.PriorityCheckCount++;
				_state.ResetTarget();
				_state.ChangeStatus(AutoCombatStatus.Targeting);
				return false;
			}
		}

		if (monsterTableEntry == null)
		{
			Trace.WriteLine("Monster name not in table");
			_state.ResetTarget();
			_state.ChangeStatus(AutoCombatStatus.Targeting);
			return false;
		}

		// A whitelisted monster was finally targeted, so we can 
		// now move to start attacking it.
		Trace.WriteLine("Found monster to attack");
		_state.ChangeStatus(AutoCombatStatus.StartAttack);
		if (CombatOptions.DelayBeforeAttack > 0)
		{
			_state.SetCooldown(TimeSpan.FromSeconds(CombatOptions.DelayBeforeAttack));
		}

		return true;
	}

	/// <summary>
	/// Prepares the state to begin attacking a targeted monster.
	/// </summary>
	/// <returns>true if attacking will begin</returns>
	private bool _prepareAttacking()
	{
		_state.StartingXp = _context.ActiveCharacter!.Xp;
		_state.StartingLevel = _context.ActiveCharacter.Level;
		_state.ChangeStatus(AutoCombatStatus.Attacking);
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
			Trace.WriteLine("Tried to attack but all attacks are on cooldown");
			return false;
		}

		// Select a random slot to attack/skill from and then go on cooldown for a little bit.
		var randomKeyIndex = Random.Next(0, notOnCooldown.Count);
		var chosenKey = notOnCooldown[randomKeyIndex];

		if (chosenKey.Cooldown > 0)
		{
			_state.SetHotkeyCooldown(chosenKey, TimeSpan.FromSeconds(chosenKey.Cooldown + 0.1f));
		}

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
			_state.ChangeStatus(AutoCombatStatus.CheckTarget, TimeSpan.FromMilliseconds(100));
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
			_state.ChangeStatus(AutoCombatStatus.CheckTarget, TimeSpan.FromMilliseconds(100));
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

	public bool ScanningForPriority { get; set; }
	public int PriorityCheckCount { get; set; }

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

	public int StartingXp { get; set; }

	public int StartingLevel { get; set; }

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
		Status = status;
		StatusTimeout = duration == null ? null : DateTime.Now.Add(duration.Value);
		Cooldown = null;
		//Trace.WriteLine($"Auto-combat status changed to {status} (duration={duration})");
	}

	/// <summary>
	/// Sets a cooldown period until the next time the
	/// current state can be processed.
	/// </summary>
	/// <param name="duration">cooldown duration</param>
	public void SetCooldown(TimeSpan duration)
	{
		Trace.WriteLine($"Set cooldown of {duration} for current state ({Status})");
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
		StartingXp = 0;
		StartingLevel = 0;
		PriorityCheckCount = 0;
		CurrentCastingBuff = 0;
		Cooldown = null;
		Trace.WriteLine("Auto-combat state was fully reset");
	}

	/// <summary>
	/// Resets the last target selected by the user.
	/// </summary>
	public void ResetTarget()
	{
		_currentTarget = null;
		_currentTargetId = 0;
		Trace.WriteLine("Auto-combat target was reset");
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