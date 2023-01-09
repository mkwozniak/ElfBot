using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace ElfBot;

/// <summary>
/// Enumeration of possible statuses that the auto-combat system can be in.
/// </summary>
public enum AutoCombatStatus
{
	Inactive,
	Starting,
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
	public SendingKey? OnSendKey;

	private ApplicationContext _context;
	private AutoCombatState _state = new();
	private int _tabKeyCode = 0x09;
	private int _lootKeyCode = 0x54;

	private readonly DispatcherTimer _autoCombatTimer = new()
	{
		Interval = TimeSpan.FromMilliseconds(50)
	};

	public AutoCombat(ApplicationContext context)
	{
		_context = context;
		_autoCombatTimer.Tick += Tick;
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
		_context.Settings.CombatOptions.AutoCombatEnabled = false;
		_state.Reset();
		_autoCombatTimer.Stop();
	}

	/// <summary>
	/// Master heartbeat method to handle the movement of state.
	/// </summary>
	private void Tick(object? sender, EventArgs e)
	{
		if (!_context.Hooked
		    || !_context.Settings.CombatOptions.AutoCombatEnabled
		    || _state.Status == AutoCombatStatus.Inactive
		    || _context.MonsterTable.Count == 0)
		{
			Trace.WriteLine("Canceled auto-combat due to invalid state");
			MainWindow.Logger.Warn("Auto-combat disabled, please ensure that ROSE is hooked and that " +
			                       "a monster table is loaded");
			Stop();
			return;
		}
		
		if (_state.isOnCooldown())
		{
			return;
		}
		
		Trace.WriteLine($"Auto-combat heartbeat (current state: {_state.Status})");

		try
		{
			_context.CharacterData.Update(); // Make sure character data is up to date
			_ = _state.Status switch
			{
				AutoCombatStatus.Starting => _start(),
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

		Trace.WriteLine("Auto-combat heartbeat completed");
	}

	private bool _start()
	{
		_state.Reset();
		_state.ChangeStatus(AutoCombatStatus.Targeting);
		return true;
	}

	/// <summary>
	/// Attempts to select a new target monster by pressing the TAB key.
	/// </summary>
	/// <returns>whether the tab key was pressed</returns>
	private bool _selectNewTarget()
	{
		_state.ResetTarget();
		OnSendKey?.Invoke(_tabKeyCode);
		Trace.WriteLine("Sent tab key press to simulator to attempt selecting a new target");
		_state.ChangeStatus(AutoCombatStatus.CheckTarget, TimeSpan.FromMilliseconds(250));
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

		var name = _context.CharacterData.CurrentTarget.Name;
		var id = _context.CharacterData.CurrentTarget.Id;
		Trace.WriteLine($"Target name is {name} with id {id}");

		// If a target is not yet found, we may need to wait more time
		// for the addresses to update. Otherwise, we may time out and 
		// attempt to select a new monster.
		if (id == 0) return false;

		_state.CurrentTargetId = id;
		_state.CurrentTarget = name;

		if(_context.Settings.CombatOptions.PriorityTargetScan && _state.ScanningForPriority) 
		{
			if (_state.PriorityCheckCount > _context.Settings.CombatOptions.MaxPriorityChecks)
			{
				Trace.WriteLine("Priority target selection expired");
				_state.ScanningForPriority = false;
				Addresses.Target.writeValue("");
				Addresses.TargetId.writeValue(0);
				_state.ResetTarget();
				// _state.Reset();
				_state.ChangeStatus(AutoCombatStatus.Targeting);
				return false;
			}

			Trace.WriteLine($"Priority Target Check: {_state.PriorityCheckCount} / {_context.Settings.CombatOptions.MaxPriorityChecks}");
			// If the selected monster is not whitelisted in the monster table,
			// we need to restart our target search.
			if (_context.MonsterTable.Contains($"*{name}"))
			{
				// A whitelisted monster was finally targeted, so we can 
				// now move to start attacking it.
				Trace.WriteLine("Found priority monster to attack");
				_state.ChangeStatus(AutoCombatStatus.StartAttack);
				return true;
			}

			Trace.WriteLine("Priority Monster name not in table");
			_state.PriorityCheckCount++;
			_state.ResetTarget();
			_state.ChangeStatus(AutoCombatStatus.Targeting);
			return false;
		}

		// If the selected monster is not whitelisted in the monster table,
		// we need to restart our target search.
		if (!_context.MonsterTable.Contains(name))
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
		return true;
	}

	/// <summary>
	/// Prepares the state to begin attacking a targeted monster.
	/// </summary>
	/// <returns>true if attacking will begin</returns>
	private bool _prepareAttacking()
	{
		_state.StartingXp = _context.CharacterData.Xp;
		_state.StartingLevel = _context.CharacterData.Level;
		_state.ChangeStatus(AutoCombatStatus.Attacking,
			TimeSpan.FromSeconds(_context.Settings.CombatOptions.AttackTimeout));
		return true;
	}

	/// <summary>
	/// Attacks the currently selected monster. 
	/// </summary>
	/// <returns>whether an attack will be performed</returns>
	private bool _attack()
	{
		if (_state.IsExpired()
		    || _state.CurrentTargetId != _context.CharacterData.CurrentTarget.Id)
		{
			Trace.WriteLine("Canceling attack due to expiration or target ID changing");
			_state.Reset();
			_state.ChangeStatus(AutoCombatStatus.Starting);
			return false;
		}

		// Player XP is checked to determine whether the monster has died. This
		// is the best current method we have, and in the future this will change
		// to checking the monsters HP or alive status. When the monster has died,
		// we need to move into either looting or restart the cycle.
		if (_context.CharacterData.Xp > _state.StartingXp
		    || _context.CharacterData.Level > _state.StartingLevel)
		{
			Trace.WriteLine($"Character XP or level changed. " +
			                $"Previous had {_state.StartingXp} XP at level {_state.StartingLevel} " +
			                $"and now has {_context.CharacterData.Xp} XP at level {_context.CharacterData.Level}");

			if (_context.Settings.LootOptions.LootAfterCombatEnabled)
			{
				var lootTimeSeconds = _context.Settings.LootOptions.Duration;
				_state.ChangeStatus(AutoCombatStatus.Looting, TimeSpan.FromSeconds(lootTimeSeconds));
			}
			else
			{
				_state.Reset();
				_state.ChangeStatus(AutoCombatStatus.Targeting);
			}

			if(_context.Settings.CombatOptions.PriorityTargetScan)
			{
				_state.PriorityCheckCount = 0;
				_state.ScanningForPriority = true;
			}

			return false;
		}

		// TODO: Implement skill cooldowns in the future
		var activeCombatKeys = _context.Settings.Keybindings
			.FindAll(kb => kb.Type is KeybindType.Attack or KeybindType.Skill)
			.ToArray();
		if (activeCombatKeys.Length == 0)
		{
			Trace.WriteLine("No attack keys have been set");
			MainWindow.Logger.Warn("Tried to attack, but no keys are set");
			return false;
		}

		// Select a random slot to attack/skill from and then go on cooldown for a little bit.
		var ranSkill = MainWindow.Ran.Next(0, activeCombatKeys.Length);
		var keyCode = activeCombatKeys[ranSkill].KeyCode;
		OnSendKey?.Invoke(activeCombatKeys[ranSkill].KeyCode);
		var delayBetweenAttacks = _context.Settings.CombatOptions.CombatKeyDelay;
		_state.SetCooldown(TimeSpan.FromSeconds(delayBetweenAttacks));
		Trace.WriteLine($"Running skill in slot {ranSkill} by pressing keycode {keyCode}. " +
		                $"Waiting {delayBetweenAttacks} second...");
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
			_state.ChangeStatus(AutoCombatStatus.Targeting);
			return false;
		}

		OnSendKey?.Invoke(_lootKeyCode);
		_state.SetCooldown(TimeSpan.FromMilliseconds(250));
		Trace.WriteLine("Sent 'T' keypress to loot, waiting 250ms");
		return true;
	}
}

public sealed class AutoCombatState : PropertyNotifyingClass
{
	private string? _currentTarget;
	private int _currentTargetId; // 0 indicates there is no active target
	private AutoCombatStatus _status = AutoCombatStatus.Inactive;
	private DateTime? _cooldown;

	public bool ScanningForPriority { get; set; } = true;
	public int PriorityCheckCount { get; set; } = 0;

	public int FirstTargetIDSelected = 0;

	public AutoCombatStatus Status
	{
		get => _status;
		set
		{
			if (_status == value) return;
			_status = value;
			NotifyPropertyChanged();
		}
	}

	public DateTime? StatusTimeout { get; private set; }
	public DateTime? Cooldown => _cooldown; // cooldown for the current state

	public int StartingXp { get; set; }

	public int StartingLevel { get; set; }

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
		_cooldown = null;
		Trace.WriteLine($"Auto-combat status changed to {status} (duration={duration})");
	}

	/// <summary>
	/// Sets a cooldown period until the next time the
	/// current state can be processed.
	/// </summary>
	/// <param name="duration">cooldown duration</param>
	public void SetCooldown(TimeSpan duration)
	{
		Trace.WriteLine($"Set cooldown of {duration} for current state ({Status})");
		_cooldown = DateTime.Now.Add(duration);
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
		_cooldown = null;
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
	/// Returns true if this state has timed out. If the state
	/// does not have a timeout set, this method will always
	/// return false.
	/// </summary>
	/// <returns>expiry/timeout status</returns>
	public bool IsExpired()
	{
		return _isDateInPast(StatusTimeout);
	}

	/// <summary>
	/// Returns true if the current state is on a cooldown.
	/// </summary>
	/// <returns>cooldown state</returns>
	public bool isOnCooldown()
	{
		return Cooldown != null && !_isDateInPast(_cooldown);
	}

	private static bool _isDateInPast(DateTime? time)
	{
		var now = DateTime.Now;
		return time != null && now.CompareTo(time) > 0;
	}
}