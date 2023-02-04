using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using ElfBot.Util;

#pragma warning disable CS4014

namespace ElfBot;

/// <summary>
/// Enumeration of possible statuses that the auto-cleric system can be in.
/// </summary>
public enum AutoClericStatus
{
	Inactive, // stopped
	Monitoring, // no heals needed
	Buffing,
	Summoning,
	Casting,
}

/// <summary>
/// Auto-combat state machine. Handles the status and actions
/// of combat when enabled.
/// </summary>
public sealed class AutoCleric
{
	private static readonly Random Random = new();

	private const int MaxAoeDistance = 15;
	private const int MaxTargetDistance = 25;

	internal readonly ApplicationContext Context;
	private readonly AutoClericState _state;

	private ClericOptions ClericOptions => Context.Settings.ClericOptions;

	private readonly DispatcherTimer _autoClericTimer = new()
	{
		Interval = TimeSpan.FromMilliseconds(250)
	};

	public AutoCleric(ApplicationContext context)
	{
		Context = context;
		_autoClericTimer.Tick += Tick;
		_state = new AutoClericState(this);
	}

	public AutoClericState State => _state;

	/// <summary>
	/// Starts the automation of combat.
	/// </summary>
	public void Start()
	{
		_state.Reset();
		_state.ChangeStatus(AutoClericStatus.Monitoring);
		_autoClericTimer.Start();
	}

	/// <summary>
	/// Stops the automation of combat.
	/// </summary>
	public void Stop()
	{
		ClericOptions.AutoClericEnabled = false;
		_state.Reset();
		_state.ClearHotkeyCooldowns();
		_state.LastBuffTime = null;
		_autoClericTimer.Stop();
	}

	/// <summary>
	/// Master heartbeat method to handle the movement of state.
	/// </summary>
	private void Tick(object? sender, EventArgs e)
	{
		if (Context.ActiveCharacter == null
		    || !ClericOptions.AutoClericEnabled
		    || _state.Status == AutoClericStatus.Inactive)
		{
			Trace.WriteLine("Canceled auto-cleric due to invalid state");
			MainWindow.Logger.Warn("Auto-cleric disabled, please ensure that ROSE is hooked");
			Stop();
			return;
		}

		if (Context.ActiveCharacter.IsDead)
		{
			if (Context.Settings.GeneralOptions.DeathAction != DeathActions.CANCEL_TIMERS) return;
			Trace.WriteLine("Canceled auto-cleric due to player death");
			MainWindow.Logger.Warn("Disabling auto-cleric due to player death");
			Stop();
			return;
		}

		if (_state.isOnCooldown())
		{
			return;
		}

		if (_state.Status == AutoClericStatus.Casting && _state.IsExpired())
		{
			_state.ChangeStatus(AutoClericStatus.Monitoring);
			return;
		}

		try
		{
			_ = _state.Status switch
			{
				AutoClericStatus.Monitoring => _checkStatus(),
				AutoClericStatus.Summoning => _summons(),
				AutoClericStatus.Buffing => _buff(),
				_ => true
			};
		}
		catch (Exception ex)
		{
			MainWindow.Logger.Error(
				$"An exception occurred when attempting to process auto-cleric state {_state.Status}");
			MainWindow.Logger.Error($"Disabling auto-cleric");
			MainWindow.Logger.Error(ex.Message);
			if (ex.StackTrace != null)
			{
				MainWindow.Logger.Error(ex.StackTrace);
			}

			Stop();
		}
	}

	private bool _castTarget(PartyMember target, KeybindAction action)
	{
		Context.ActiveCharacter!.LastTargetId = target.Id;
		Trace.WriteLine($"Applying action {action} to player {target.Name} (HP: {target.Entity!.Hp})");
		return _cast(action);
	}

	private bool _cast(KeybindAction action)
	{
		var hotkeys = _findHotkeys(action);
		if (hotkeys.Length == 0)
		{
			_state.ChangeStatus(AutoClericStatus.Monitoring);
			return false;
		}

		var chosenKey = hotkeys[Random.Next(0, hotkeys.Length)];
		Trace.WriteLine($"Running action {action} in hotkey slot {chosenKey.KeyCode}");
		RoseProcess.SendKeypress(chosenKey.KeyCode, chosenKey.IsShift);
		_state.SetHotkeyCooldown(chosenKey, TimeSpan.FromSeconds(chosenKey.Cooldown + 0.1f));
		_state.ChangeStatus(AutoClericStatus.Casting, TimeSpan.FromSeconds(2));
		return true;
	}

	/// <summary>
	/// Checks the status of all party members and decides what action to take.
	///
	/// Action priority is:
	/// 1. Revive any dead players
	/// 2. Party heal if enough damaged players are nearby
	/// 3. Party restore if enough damaged players are nearby
	/// 4. Focus the most damaged player and heal/restore as necessary
	/// 5. Summons/buffs
	/// </summary>
	/// <returns>whether an action was taken</returns>
	private bool _checkStatus()
	{
		var partyMembers = Context.ActiveCharacter!.Party.PartyMembers.Where(m => m.IsVisible).ToArray();

		var nearbyRestoreableMembers = 0;
		var nearbyHealableMembers = 0;

		foreach (var member in partyMembers)
		{
			if (!member.IsVisible || member.Name == Context.ActiveCharacter.Name) continue;
			var player = member.Entity!;
			var dist = Context.ActiveCharacter.GetDistanceTo(player);

			// The first priority is a dead player, if they are within range
			// we attempt to revive them.
			if (player.IsDead && dist <= MaxTargetDistance
			                  && _findHotkeys(KeybindAction.Revive).Length > 0)
			{
				return _castTarget(member, KeybindAction.Revive);
			}

			if (dist <= MaxAoeDistance &&
			    _shouldTrigger(player.Hp, player.MaxHp, ClericOptions.RestoreHpThresholdPercent))
				nearbyRestoreableMembers += 1;
			if (dist <= MaxAoeDistance && _shouldTrigger(player.Hp, player.MaxHp, ClericOptions.HealHpThresholdPercent))
				nearbyHealableMembers += 1;
		}

		if (nearbyHealableMembers > 0
		    && _findHotkeys(KeybindAction.HealParty).Length > 0)
		{
			return _cast(KeybindAction.HealParty);
		}

		if (nearbyRestoreableMembers > 0
		    && _findHotkeys(KeybindAction.RestoreParty).Length > 0)
		{
			return _cast(KeybindAction.RestoreParty);
		}

		var mostDamaged = partyMembers
			.Where(m => m.Name != Context.ActiveCharacter.Name)
			.Where(m => Context.ActiveCharacter.GetDistanceTo(m.Entity!) < MaxTargetDistance)
			.Where(m => m.Entity!.Hp < m.Entity.MaxHp)
			.MinBy(m => m.Entity!.Hp / (float)m.Entity.MaxHp * 100);
		if (mostDamaged != null)
		{
			if (_findHotkeys(KeybindAction.Heal).Length > 0
			    && _shouldTrigger(mostDamaged.Entity!.Hp, mostDamaged.Entity.MaxHp,
				    ClericOptions.HealHpThresholdPercent))
			{
				return _castTarget(mostDamaged, KeybindAction.Heal);
			}

			if (_findHotkeys(KeybindAction.Restore).Length > 0
			    && _shouldTrigger(mostDamaged.Entity!.Hp, mostDamaged.Entity.MaxHp,
				    ClericOptions.RestoreHpThresholdPercent))
			{
				return _castTarget(mostDamaged, KeybindAction.Restore);
			}
		}

		if (Context.Settings.GeneralOptions.SummonsEnabled && _canSummon())
		{
			_state.ChangeStatus(AutoClericStatus.Summoning);
			return true;
		}

		if (_state.CanApplyBuffs())
		{
			_state.ChangeStatus(AutoClericStatus.Buffing);
			return true;
		}

		return false;
	}

	private HotkeySlot[] _findHotkeys(KeybindAction action)
	{
		var activeKeys = Context.Settings.FindKeybindings(action);
		return activeKeys.Count == 0
			? Array.Empty<HotkeySlot>()
			: activeKeys.Where(k => !_state.isHotkeyOnCooldown(k)).ToArray();
	}

	private bool _summons()
	{
		if (!_canSummon())
		{
			_state.Reset();
			if (Context.Settings.CombatOptions.BuffsEnabled) _state.ChangeStatus(AutoClericStatus.Buffing);
			else _state.ChangeStatus(AutoClericStatus.Monitoring);
			return true;
		}

		var activeSummonKeys = Context.Settings.FindKeybindings(KeybindAction.Summon);
		if (activeSummonKeys.Count == 0)
		{
			Trace.WriteLine("No summon keys have been set");
			MainWindow.Logger.Warn("Tried to summon, but no keys are set");
			_state.Reset();
			_state.ChangeStatus(AutoClericStatus.Monitoring);
			return false;
		}

		// Select a random slot to attack/skill from and then go on cooldown for a little bit.
		var chosenKey = activeSummonKeys[_state.CurrentCastingSummon];
		if (_state.isHotkeyOnCooldown(chosenKey))
		{
			Trace.WriteLine("Attempted summon was on cooldown");
			MainWindow.Logger.Warn("Attempted summon was on cooldown");
			return false;
		}

		_state.CurrentCastingSummon++;
		if (_state.CurrentCastingSummon >= activeSummonKeys.Count)
		{
			_state.CurrentCastingSummon = 0;
		}

		RoseProcess.SendKeypress(chosenKey.KeyCode, chosenKey.IsShift);
		_state.SetHotkeyCooldown(chosenKey, TimeSpan.FromSeconds(chosenKey.Cooldown + 0.1f));
		_state.SetCooldown(TimeSpan.FromSeconds(2)); // Wait for animation
		Trace.WriteLine($"Running summon in slot {chosenKey.Key} by pressing keycode {chosenKey.KeyCode}. ");
		return true;
	}

	private bool _canSummon() // TODO: Check if theres a summoning key
	{
		var currentSummonMeter = Context.ActiveCharacter!.ConsumedSummonsMeter;
		var summonCost = Context.Settings.GeneralOptions.SummonCost;
		var maxSummons = Context.Settings.GeneralOptions.MaxSummonCount;
		return currentSummonMeter + summonCost <= maxSummons;
	}

	private bool _buff()
	{
		var activeBuffKeys = Context.Settings.FindKeybindings(KeybindAction.Buff);
		if (activeBuffKeys.Count == 0)
		{
			Trace.WriteLine("No buff keys have been set");
			MainWindow.Logger.Warn("Tried to buff, but no keys are set");
			_state.Reset();
			_state.ChangeStatus(AutoClericStatus.Monitoring, TimeSpan.FromMilliseconds(100));
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
			_state.ChangeStatus(AutoClericStatus.Monitoring, TimeSpan.FromMilliseconds(100));
		}
		else
		{
			_state.SetCooldown(TimeSpan.FromSeconds(2));
		}

		return true;
	}

	private static bool _shouldTrigger(int value, int maxValue, float threshold)
	{
		var percent = value / (float)maxValue;
		var thresholdPercent = threshold / 100;
		return percent <= thresholdPercent;
	}
}

public sealed class AutoClericState : PropertyNotifyingClass
{
	private readonly AutoCleric _autoCleric;
	private AutoClericStatus _status = AutoClericStatus.Inactive;
	private HotkeyCooldownTracker _hotkeyCooldowns = new();

	public AutoClericState(AutoCleric autoCleric)
	{
		_autoCleric = autoCleric;
	}

	public AutoClericStatus Status
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
	
	public int CurrentCastingSummon { get; set; }

	/// <summary>
	/// Changes the current status to a new one, optionally
	/// with a specified duration.
	/// </summary>
	/// <param name="status">New status</param>
	/// <param name="duration">The maximum amount of time to stay in the new state for</param>
	public void ChangeStatus(AutoClericStatus status, TimeSpan? duration = null)
	{
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
		ChangeStatus(AutoClericStatus.Inactive);
		CurrentCastingBuff = 0;
		CurrentCastingSummon = 0;
		Cooldown = null;
	}

	/// <summary>
	/// Returns true if buffs are able to be applied.
	/// </summary>
	/// <returns>buff readiness status</returns>
	public bool CanApplyBuffs()
	{
		return _autoCleric.Context.Settings.CombatOptions.BuffsEnabled
		       && LastBuffTime == null
		       || _isDateInPast(LastBuffTime?.AddSeconds(_autoCleric.Context.Settings.CombatOptions.BuffFrequency));
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