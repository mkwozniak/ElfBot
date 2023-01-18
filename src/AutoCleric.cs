using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	Idle, // no heals needed
	PrepareScanning,
	Scanning, // Scanning and collecting nearby party members
	Buffing,
	Summoning,
	Casting,
	Reviving
}

/// <summary>
/// Auto-combat state machine. Handles the status and actions
/// of combat when enabled.
/// </summary>
public sealed class AutoCleric
{
	public static readonly Random Random = new();
	
	private readonly ApplicationContext _context;
	private readonly AutoClericState _state;

	internal ClericOptions ClericOptions => _context.Settings.ClericOptions;

	private readonly DispatcherTimer _autoClericTimer = new()
	{
		Interval = TimeSpan.FromMilliseconds(250)
	};

	public AutoCleric(ApplicationContext context)
	{
		_context = context;
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
		_state.ChangeStatus(AutoClericStatus.Idle);
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
		
		
		
		// TODO
		requiresScanTest = true;
	}

	/// <summary>
	/// Master heartbeat method to handle the movement of state.
	/// </summary>
	private void Tick(object? sender, EventArgs e)
	{
		if (_context.ActiveCharacter == null
		    || !ClericOptions.AutoClericEnabled
		    || _state.Status == AutoClericStatus.Inactive)
		{
			Trace.WriteLine("Canceled auto-cleric due to invalid state");
			MainWindow.Logger.Warn("Auto-cleric disabled, please ensure that ROSE is hooked");
			Stop();
			return;
		}

		if (_context.ActiveCharacter.IsDead)
		{
			if (_context.Settings.GeneralOptions.DeathAction != DeathActions.CANCEL_TIMERS) return;
			Trace.WriteLine("Canceled auto-cleric due to player death");
			MainWindow.Logger.Warn("Disabling auto-cleric due to player death");
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
				AutoClericStatus.Idle => _checkStatus(),
				AutoClericStatus.PrepareScanning => _prepareScan(),
				AutoClericStatus.Scanning => _scan(),
				AutoClericStatus.Summoning => _summons(),
				AutoClericStatus.Buffing => _buff(),
				_ => true
			};
		}
		catch (Exception ex)
		{
			MainWindow.Logger.Error($"An exception occurred when attempting to process auto-cleric state {_state.Status}");
			MainWindow.Logger.Error($"Disabling auto-cleric");
			MainWindow.Logger.Error(ex.Message);
			if (ex.StackTrace != null)
			{
				MainWindow.Logger.Error(ex.StackTrace);
			}

			Stop();
		}
	}

	private bool _requiresScan()
	{
		
		// TODO: Rescan if party size has changed
		// or if a party member has become invalid
		
		return requiresScanTest;
	}

	
	// Scanning
	private int? _lastTargetScanned;

	private bool requiresScanTest = true;

	private List<TargetedEntity> _party = new(); // TODO: don't have player name. maybe targeted player class?
	
	private bool _prepareScan()
	{
		_party.Clear();
		
		if (_context.ActiveCharacter!.LastTargetId != _context.ActiveCharacter.Id)
		{
			// F1 selects the current player, so we are ensuring that the scan 
			// starts with the current player selected, and then rotates through
			// the party members.
			Trace.WriteLine("Waiting for self selection");
			RoseProcess.SendKeypress(Messaging.VKeys.KEY_F1);
			return false;
		}
		
		Trace.WriteLine("Starting to scan");
		_state.ChangeStatus(AutoClericStatus.Scanning);
		_lastTargetScanned = _context.ActiveCharacter.Id;
		// immediately start scanning to avoid starting the scan at the same player
		RoseProcess.SendKeypress(Messaging.VKeys.KEY_F3); 
		return true;
	}
	
	// TODO: Current player target name is available and can be used to reinforce scanning checks
	// TODO: it appears possible to be able to get the current selected party member index, this would be the most reliable method
	private bool _scan()
	{
		var currentTarget = _context.ActiveCharacter!.LastTargetId;

		if (_lastTargetScanned == currentTarget)  
		{
			return false;
		}

		if (currentTarget == _context.ActiveCharacter.Id) 
		{
			Trace.WriteLine("Back at self, scanning completed");
			_lastTargetScanned = 0;
			requiresScanTest = false;
			_state.ChangeStatus(AutoClericStatus.Idle);
			return true;
		}
		
		if (currentTarget <= 0) // player might not be in range, skip to next
		{
			Trace.WriteLine("Invalid target, skipping");
			_context.ActiveCharacter.LastTargetId = -1;
			_lastTargetScanned = -1;
			RoseProcess.SendKeypress(Messaging.VKeys.KEY_F3);
			return false;
		}
		
		// TODO: Set current target to -1 or some arbitrary value to prevent double 0 skips
		
		// Keep scanning
		Trace.WriteLine($"Found target {currentTarget}"); // TODO: Get the current target and track them
		_party.Add(new TargetedEntity(currentTarget));
		RoseProcess.SendKeypress(Messaging.VKeys.KEY_F3);
		_lastTargetScanned = currentTarget;
		return true;
	}

	private bool _checkStatus()
	{
		// TODO: check self health
		
		//??_state.Reset();
		if (_requiresScan())
		{
			// TODO: Do we need to initialize?
			_state.ChangeStatus(AutoClericStatus.PrepareScanning);
			return true;
		}
		
		// find player with lowest hp
		// determine when to use restore
		// determine when to use party restore/heals
		
		
		// check if we need to scan
		// check if we're even in a party - pause if not
		
		// check party members health, handle healing and revive logic

		if (_context.Settings.GeneralOptions.SummonsEnabled && _canSummon())
		{
			_state.ChangeStatus(AutoClericStatus.Summoning);
			return true;
		}

		if (_state.CanApplyBuffs())  // TODO: Check if buffing keypresses are set
		{
			_state.ChangeStatus(AutoClericStatus.Buffing);
			return true;
		}

		//??_state.ChangeStatus(AutoClericStatus.Idle);
		return true;
	}
	
	private bool _summons()
	{
		if (!_canSummon()) 
		{
			_state.Reset();
			if (_context.Settings.CombatOptions.BuffsEnabled) _state.ChangeStatus(AutoClericStatus.Buffing);
			else _state.ChangeStatus(AutoClericStatus.Idle);
			return true;
		}
		
		var activeSummonKeys = _context.Settings.FindKeybindings(KeybindAction.Summon);
		if (activeSummonKeys.Count == 0)
		{
			Trace.WriteLine("No summon keys have been set");
			MainWindow.Logger.Warn("Tried to summon, but no keys are set");
			_state.Reset();
			_state.ChangeStatus(AutoClericStatus.Idle);
			return false;
		}

		// Select a random slot to attack/skill from and then go on cooldown for a little bit.
		var nextKey = _state.CurrentCastingBuff;
		var chosenKey = activeSummonKeys[nextKey];
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
	
	private bool _canSummon() // TODO: Check if theres a summoning key
	{
		var currentSummonMeter = _context.ActiveCharacter!.ConsumedSummonsMeter;
		Trace.Write($"Current summon meter {currentSummonMeter}");
		var summonCost = _context.Settings.GeneralOptions.SummonCost;
		var maxSummons = _context.Settings.GeneralOptions.MaxSummonCount;
		return currentSummonMeter + summonCost <= maxSummons;
	}

	private bool _buff()
	{
		var activeBuffKeys = _context.Settings.FindKeybindings(KeybindAction.Buff);
		if (activeBuffKeys.Count == 0)
		{
			Trace.WriteLine("No buff keys have been set");
			MainWindow.Logger.Warn("Tried to buff, but no keys are set");
			_state.Reset();
			_state.ChangeStatus(AutoClericStatus.Idle, TimeSpan.FromMilliseconds(100));
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
			_state.ChangeStatus(AutoClericStatus.Idle, TimeSpan.FromMilliseconds(100));
		}
		else
		{
			_state.SetCooldown(TimeSpan.FromSeconds(2));
		}
		return true;
	}
}

public sealed class AutoClericState : PropertyNotifyingClass
{
	private readonly AutoCleric _autoCleric;
	private string? _currentTarget;
	private int _currentTargetId; // 0 indicates there is no active target
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
		// TODO;
		return false;
//		return Settings.CombatOptions.BuffsEnabled
			//&& LastBuffTime == null || _isDateInPast(LastBuffTime?.AddSeconds(_autoCleric.CombatOptions.BuffFrequency));
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