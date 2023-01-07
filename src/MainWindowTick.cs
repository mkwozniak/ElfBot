namespace ElfBot;

using System.Windows;
using System.Linq;
using EventArgs = System.EventArgs;
using EventHandler = System.EventHandler;
using Timer = System.Windows.Threading.DispatcherTimer;
using Math = System.Math;
using Enum = System.Enum;
using Environment = System.Environment;
using Trace = System.Diagnostics.Trace;

using VirtualKeyCode = WindowsInput.Native.VirtualKeyCode;

public sealed partial class MainWindow : Window
{
    #region Timer Methods

    private void ListenToTimer(Timer timer, EventHandler del)
    {
        timer.Tick += new EventHandler(del);
    }

    private void StartTimer(Timer timer, int msDelay)
    {
        timer.Interval = new System.TimeSpan(0, 0, 0, 0, msDelay);
        timer.Start();
    }

    private void StopTimer(Timer timer)
    {
        timer.Stop();
    }

    private void StopAllCombatRelatedTimers()
    {
        StopTimer(CombatTimer);
        StopTimer(TargettingTimer);
        StopTimer(CheckTimer);
        StopTimer(LootingEndTimer);
        StopTimer(LootingTimer);
        StopTimer(AttackTimeoutTimer);
    }

    #endregion

    #region Tick Methods

    /// <summary> Timer tick for when bot is targetting </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Targetting_Tick(object? sender, EventArgs e)
    {
        if (_monsterTable == null || _sim == null)
            return;

        AutoCombatState.Content = $"Combat State: {_combatState}";

        StopTimer(TargettingTimer);

        if (_currentTargetUID != -1)
        {
            _currentTarget = Addresses.Target.GetValue(); // make sure we are on the target we want.
            _currentTargetUID = Addresses.TargetId.GetValue();
        }

        // if current target isnt in monstertable or there is no unique target id
        if ((!_monsterTable.Contains(_currentTarget) || _currentTargetUID == 0) && !_pressedTargetting)
        {
            // press targetting key
            _sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
            _pressedTargetting = true;

            Logger.Debug("Pressed tab key to select next target",  LogEntryTag.Combat);

            _currentXP = Addresses.Xp.GetValue();
            _xpBeforeKill = _currentXP;

            // go into checking target mode to make sure the tab target was OK
            // update labels
            XpBeforeKillLabel.Content = $@"XP Before Kill: {_currentXP}";
            StartTimer(CheckTimer, (int)(Settings.CombatOptions.ActionTimerDelay * 1000));
            _combatState = CombatStates.CheckingTarget;
            return;
        }

        SwitchToTargetting();
    }

    /// <summary> Timer tick for when bot is checking its target </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CheckingTarget_Tick(object? sender, EventArgs e)
    {
		if (_monsterTable == null)
			return;

		AutoCombatState.Content = $"Combat State: {_combatState}";

        // get target memory
        _currentTarget = Addresses.Target.GetValue();
        _currentTargetUID = Addresses.TargetId.GetValue();

        Logger.Debug("Checking active target...", LogEntryTag.Combat);

        StopTimer(CheckTimer);

        // if current target is in monster table
        if (_monsterTable.Contains(_currentTarget) && _currentTargetUID != 0)
        {
            StopTimer(AttackTimeoutTimer); // stop timeout timer

            // go into attack state
            StopTimer(CombatTimer);

            // keep track of last position before going into attack mode 
            _lastXPos = Addresses.PositionX.GetValue();
            _lastYPos = Addresses.PositionY.GetValue();

            _targetDefeatedMsg = "";
            _combatState = CombatStates.Attacking;

            if (!CombatTimer.IsEnabled)
            {
                StartTimer(CombatTimer, (int)(Settings.CombatOptions.CombatKeyDelay * 1000));
            }

            return;
        }

        SwitchToTargetting();
    }

    /// <summary> Timer tick for when bot is attacking its target </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Attacking_Tick(object? sender, EventArgs e)
    {
		if (_monsterTable == null)
			return;

		AutoCombatState.Content = $"Combat State: {_combatState}";

        _currentXP = Addresses.Xp.GetValue(); // get current xp
        _currentTarget = Addresses.Target.GetValue(); // make sure we are on the target we want.
        _currentTargetUID = Addresses.TargetId.GetValue();
        _playerMaxMP = Addresses.MaxMp.GetValue();

        if (_monsterTable.Contains(_currentTarget))
        {
            CheckTargetKilled();
            CheckShouldAttackTarget();
        }

        // no proper target while attacking
        if (_currentTargetUID == 0)
        {
            // back to targetting
            StopTimer(CombatTimer);
            SwitchToTargetting();
        }

        // if the current XP is less than the xp before the last kill, then the char leveled up
        if (_currentXP < _xpBeforeKill)
        {
            Logger.Debug("Level up detected, resetting state", LogEntryTag.Combat);

            // reset the xp before kill to -1
            _xpBeforeKill = -1;
            StopAllCombatRelatedTimers();

            // back to targetting
            SwitchToTargetting(true);
        }
    }

    /// <summary> Timer tick for bot interface values to update </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Interface_Tick(object? sender, EventArgs e)
    {
        // Handle logging view
        if (LoggingOptionsPanel.Visibility == Visibility.Visible)
        {
            RefreshLogs();
        }

        if (!ApplicationContext.Hooked)
        {
            return;
        }

        // Primary window
        AutoCombatState.Content = $"Combat State: {_combatState}";

        // Character information
        string name = Addresses.CharacterName.GetValue();
        int level = Addresses.Level.GetValue();
        int xp = Addresses.Xp.GetValue();
        int zuly = Addresses.Zuly.GetValue();
        PlayerNameLabel.Content = $@"Name: {name}";
        PlayerLevelLabel.Content = $@"Level: {level}";
        CurrentXpLabel.Content = $@"XP: {xp:n0}";
        PlayerZulyLabel.Content = $@"Zuly: {zuly:n0}";

        // Location information
        float x = Addresses.PositionX.GetValue();
        float y = Addresses.PositionY.GetValue();
        float z = Addresses.PositionZ.GetValue();
        int mapId = Addresses.MapId.GetValue();
        PlayerPosXLabel.Content = $@"X: {x}";
        PlayerPosYLabel.Content = $@"Y: {y}";
        PlayerPosZLabel.Content = $@"Z: {z}";
        PlayerMapIdLabel.Content = $@"Map ID: {mapId}";

        // Status information
        int hp = Addresses.Hp.GetValue();
        int maxHp = Addresses.MaxHp.GetValue();
        int mp = Addresses.Mp.GetValue();
        int maxMp = Addresses.MaxMp.GetValue();
        PlayerHpLabel.Content = $@"HP: {hp} / {maxHp}";
        PlayerMpLabel.Content = $@"MP: {mp} / {maxMp}";

        // Misc information
        TargetLabel.Content = $@"Target: {(string.IsNullOrEmpty(_currentTarget) ? "N/A" : _currentTarget)}";
        TargetIdLabel.Content = $@"Target UID: {_currentTargetUID}";
    }

    /// <summary> Refreshes the log </summary>
    private void RefreshLogs()
    {
        Level level = (Level) Enum.Parse(typeof(Level), LogLevelSelection.Text, true);
        var logEntries = Logger.Entries.Where(e => e.Level >= level).ToList();
        
        var displayedLog = SystemMsgLog.Content;
        if (displayedLog is not string)
        {
            SystemMsgLog.Content = "";
        }

        if (logEntries.Count == 0)
        {
            SystemMsgLog.Content = "";
            return;
        }

        var lines = logEntries.Select(entry =>
        {
            var date = entry.TimeStamp.ToString("hh:mm:ss tt");
            return $"({date}) {entry.Level}: {entry.Text}";
        }).ToArray();
        SystemMsgLog.Content = string.Join(Environment.NewLine, lines);
    }

    /// <summary> Timer tick for looting </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Loot_Tick(object sender, EventArgs e)
    {
        AutoCombatState.Content = $"Combat State: {_combatState}";
        Logger.Debug("Looting items...", LogEntryTag.Combat);
        _sim.Keyboard.KeyPress(VirtualKeyCode.VK_T);
    }

    /// <summary> Timer tick for looting finished </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LootEnd_Tick(object? sender, EventArgs e)
    {
        AutoCombatState.Content = $"Combat State: {_combatState}";
        StopTimer(LootingEndTimer);
        StopTimer(LootingTimer);
        Logger.Debug("Finished looting items", LogEntryTag.Combat);

        SwitchToTargetting(true);
    }

    /// <summary> Timer tick for eating hp food </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HpFoodTimer_Tick(object? sender, EventArgs e)
    {
		if (_sim == null)
			return;
        var activeHpKeys = Settings.Keybindings
            .FindAll(kb => kb.Type is KeybindType.HpFood or KeybindType.HpInstant)
            .ToArray();
        if (activeHpKeys.Length == 0)
        {
	        Trace.WriteLine("No active HP keys!");
            Logger.Warn("No active HP keys are set", LogEntryTag.System);
	        return;
        }

        _playerHP = Addresses.Hp.GetValue();
        _playerMaxHP = Addresses.MaxHp.GetValue();

        if (_playerHP == 0 || _playerMaxHP == 0)
            return;

        float hpPercent = ((float)(_playerHP) / (float)(_playerMaxHP));

		Trace.WriteLine("Check HP Food Tick");
        Logger.Debug("Checking health...", LogEntryTag.Food);
        
        if (hpPercent < (Settings.FoodOptions.AutoHpThresholdPercent / 100) && _eatHPFood)
        {
            int ranFood = _ran.Next(0, activeHpKeys.Length);
				Trace.WriteLine("Eat HP Food Tick");
            Logger.Debug($"Health is low, eating food at slot {ranFood}", LogEntryTag.Food);
            _sim.Keyboard.KeyPress(activeHpKeys[ranFood].KeyCode); // food press
            _eatHPFood = false;
            // start the delay timer to press the key again
            StartTimer(HpFoodKeyTimer, (int)(Settings.FoodOptions.Cooldown * 1000));
        }
    }

    /// <summary> Timer tick for eating mp food </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MpFoodTimer_Tick(object? sender, EventArgs e)
    {
		if (_sim == null)
			return;

        var activeMpKeys = Settings.Keybindings
            .FindAll(kb => kb.Type is KeybindType.MpFood or KeybindType.MpInstant)
            .ToArray();

        if (activeMpKeys.Length == 0)
        {
			Trace.WriteLine("No active MP keys!");
            Logger.Warn("No active MP keys are set", LogEntryTag.System);
			return;
		}

        _playerMP = Addresses.Mp.GetValue();
        _playerMaxMP = Addresses.MaxMp.GetValue();

        if (_playerMP == 0 || _playerMaxMP == 0)
            return;

        float mpPercent = ((float)(_playerMP) / (float)(_playerMaxMP));

		Trace.WriteLine("Check MP Food Tick");
        Logger.Debug("Checking mana...", LogEntryTag.Food);

        if (mpPercent < (Settings.FoodOptions.AutoMpThresholdPercent / 100) && _eatMPFood)
        {
            int ranFood = _ran.Next(0, activeMpKeys.Length);
			Trace.WriteLine("Eat MP Food Tick");
            Logger.Debug($"Mana is low, eating food at slot {ranFood}", LogEntryTag.Food);
            _sim.Keyboard.KeyPress(activeMpKeys[ranFood].KeyCode); // food press
            // start the delay timer to press the key again
            StartTimer(MpFoodKeyTimer, (int)(Settings.FoodOptions.Cooldown * 1000));
            _eatMPFood = false;
        }
    }

    /// <summary> Timer tick to reset hp food key </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HpFoodKeyTimer_Tick(object? sender, EventArgs e)
    {
        _eatHPFood = true;
        StopTimer(HpFoodKeyTimer);
    }

    /// <summary> Timer tick to reset mp food key </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MpFoodKeyTimer_Tick(object? sender, EventArgs e)
    {
        _eatMPFood = true;
        StopTimer(MpFoodKeyTimer);
    }

    /// <summary> Timer tick to timeout retarget </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RetargetTimeout_Tick(object? sender, EventArgs e)
    {
        _currentXP = Addresses.Xp.GetValue();
        float x = Addresses.PositionX.GetValue();
        float y = Addresses.PositionY.GetValue();

        StopTimer(AttackTimeoutTimer);

        if ((_targetDefeatedMsg.Length == 0 && _currentXP == _xpBeforeKill) || (x == _lastXPos && y == _lastYPos))
        {
            Logger.Debug($"Attack timed out", LogEntryTag.Combat);
            StopAllCombatRelatedTimers();
            SwitchToTargetting(true);
        }
    }

    /// <summary> Timer tick for combat camera </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CombatCameraTimer_Tick(object? sender, EventArgs e)
    {
        if (!ApplicationContext.Hooked)
            return;

        CameraZoomLabel.Content = $@"Zoom: {Addresses.CameraZoom.GetValue()}";
        CameraPitchLabel.Content = $@"Pitch: {Addresses.CameraPitch.GetValue()}";
        CameraYawLabel.Content = $@"Yaw: {Addresses.CameraYaw.GetValue()}";

        if (Settings.CombatOptions.ForceCameraOverhead)
        {
            Addresses.CameraPitch.writeValue(CameraMaxPitch);
        }
        if (Settings.CombatOptions.ForceCameraZoom)
        {
            Addresses.CameraZoom.writeValue(CameraMaxZoom);
        }
    }

    /// <summary> Timer tick for camera yaw spin </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CameraYawTimer_Tick(object? sender, EventArgs e)
    {
		if (!ApplicationContext.Hooked || !Settings.CombatOptions.CameraYawWaveEnabled || _sim == null)
            return;

        float waveform = (float)(Math.PI * Math.Sin(0.25 * _yawCounter));
        Addresses.CameraYaw.writeValue(waveform);
        _yawCounter += 0.05;

        _rightClickCounter++;

        if (_rightClickCounter > 50)
        {
            _rightClickCounter = 0;
            _sim.Mouse.VerticalScroll(-1);
        }
    }

	/// <summary> Timer tick for timed zhack </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void ZHackTimer_Tick(object? sender, EventArgs e)
	{
		if (!ApplicationContext.Hooked)
			return;

		Logger.Debug("ZHack Timer Tick", LogEntryTag.System);
		Addresses.PositionZ.writeValue(Settings.ZHackOptions.Amount);
	}

	#endregion
}