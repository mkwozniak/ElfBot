using System.Linq;

namespace ElfBot
{
    using System.Windows;
    using EventArgs = System.EventArgs;
    using EventHandler = System.EventHandler;
    using VirtualKeyCode = WindowsInput.Native.VirtualKeyCode;
    using Timer = System.Windows.Threading.DispatcherTimer;
    using Math = System.Math;
    using System.Windows.Controls;
    using System;
    using System.Diagnostics;

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

        #region Timer Tick Methods

        /// <summary> Timer tick for when bot is targetting </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Targetting_Tick(object sender, EventArgs e)
        {
            AutoCombatState.Content = _combatState.ToString();

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

                Globals.Logger.Debug("Pressed tab key to select next target",  LogEntryTag.Combat);

                _currentXP = Addresses.Xp.GetValue();
                _xpBeforeKill = _currentXP;

                // go into checking target mode to make sure the tab target was OK
                // update labels
                XPBeforeKillLabel.Content = $@"XP Before Kill: {_currentXP}";
                StartTimer(CheckTimer, (int)(_actionDelay * 1000));
                _combatState = CombatStates.CheckingTarget;
                return;
            }

            SwitchToTargetting();
        }

        /// <summary> Timer tick for when bot is checking its target </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckingTarget_Tick(object sender, EventArgs e)
        {
            AutoCombatState.Content = _combatState.ToString();

            // get target memory
            _currentTarget = Addresses.Target.GetValue();
            _currentTargetUID = Addresses.TargetId.GetValue();

            Globals.Logger.Debug("Checking active target...", LogEntryTag.Combat);

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
                    StartTimer(CombatTimer, (int)(_combatKeyDelay * 1000));
                }

                return;
            }

            SwitchToTargetting();
        }

        /// <summary>
        /// Timer tick for when bot is attacking its target
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Attacking_Tick(object sender, EventArgs e)
        {
            AutoCombatState.Content = _combatState.ToString();

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
                Globals.Logger.Debug("Level up detected, resetting state", LogEntryTag.Combat);

                // reset the xp before kill to -1
                _xpBeforeKill = -1;
                StopAllCombatRelatedTimers();

                // back to targetting
                SwitchToTargetting(true);
            }
        }

        /// <summary>
        /// Timer tick for bot interface values to update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Interface_Tick(object sender, EventArgs e)
        {
            // Handle logging view
            if (LoggingOptionsPanel.Visibility == Visibility.Visible)
            {
                RefreshLogs();
            }
            if (!Globals.Hooked)
            {
                return;
            }

            // Primary window
            AutoCombatState.Content = _combatState.ToString();

            // Character information
            string name = Addresses.CharacterName.GetValue();
            int level = Addresses.Level.GetValue();
            int xp = Addresses.Xp.GetValue();
            int zuly = Addresses.Zuly.GetValue();
            PlayerNameLabel.Content = $@"Name: {name}";
            PlayerLevelLabel.Content = $@"Level: {level}";
            CurrentXPLabel.Content = $@"XP: {xp:n0}";
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
            PlayerHPLabel.Content = $@"HP: {hp} / {maxHp}";
            PlayerMPLabel.Content = $@"MP: {mp} / {maxMp}";

            // Misc information
            TargetLabel.Content = $@"Target: {(string.IsNullOrEmpty(_currentTarget) ? "N/A" : _currentTarget)}";
            TargetUIDLabel.Content = $@"Target UID: {_currentTargetUID}";
        }

        private void RefreshLogs()
        {
            Level level = (Level) Enum.Parse(typeof(Level), LogLevelSelection.Text, true);
            var logEntries = Globals.Logger.Entries.Where(e => e.Level >= level).ToList();
            
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
            AutoCombatState.Content = _combatState.ToString();
            Globals.Logger.Debug("Looting items...", LogEntryTag.Combat);
            _sim.Keyboard.KeyPress(VirtualKeyCode.VK_4);
        }

        /// <summary> Timer tick for looting finished </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LootEnd_Tick(object sender, EventArgs e)
        {
            AutoCombatState.Content = _combatState.ToString();
            StopTimer(LootingEndTimer);
            StopTimer(LootingTimer);
            Globals.Logger.Debug("Finished looting items", LogEntryTag.Combat);

            SwitchToTargetting(true);
        }

        /// <summary> Timer tick for eating hp food </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HpFoodTimer_Tick(object sender, EventArgs e)
        {
            if (_activeHPKeys.Count == 0)
            {
				Trace.WriteLine("No active HP keys!");
                Globals.Logger.Warn("No active HP keys are set", LogEntryTag.System);
				return;
			}

            _playerHP = Addresses.Hp.GetValue();
            _playerMaxHP = Addresses.MaxHp.GetValue();

            if (_playerHP == 0 || _playerMaxHP == 0)
                return;

            float hpPercent = ((float)(_playerHP) / (float)(_playerMaxHP));

			Trace.WriteLine("Check HP Food Tick");
            Globals.Logger.Debug("Checking health...", LogEntryTag.Food);
            
            if (hpPercent < (_currentFoodHPThreshold / 100) && _eatHPFood)
            {
                int ranFood = _ran.Next(0, _activeHPKeys.Count);
				Trace.WriteLine("Eat HP Food Tick");
                Globals.Logger.Debug($"Health is low, eating food at slot {ranFood}", LogEntryTag.Food);
                _sim.Keyboard.KeyPress(_activeHPKeys[ranFood]); // food press
                _eatHPFood = false;
                // start the delay timer to press the key again
                StartTimer(HpFoodKeyTimer, (int)(_hpKeyDelay * 1000));
            }
        }

        /// <summary> Timer tick for eating mp food </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MpFoodTimer_Tick(object sender, EventArgs e)
        {
            if (_activeMPKeys.Count == 0)
            {
				Trace.WriteLine("No active MP keys!");
                Globals.Logger.Warn("No active MP keys are set", LogEntryTag.System);
				return;
			}

            _playerMP = Addresses.Mp.GetValue();
            _playerMaxMP = Addresses.MaxMp.GetValue();

            if (_playerMP == 0 || _playerMaxMP == 0)
                return;

            float mpPercent = ((float)(_playerMP) / (float)(_playerMaxMP));

			Trace.WriteLine("Check MP Food Tick");
            Globals.Logger.Debug("Checking mana...", LogEntryTag.Food);

            if (mpPercent < (_currentFoodMPThreshold / 100) && _eatMPFood)
            {
                int ranFood = _ran.Next(0, _activeMPKeys.Count);
				Trace.WriteLine("Eat MP Food Tick");
                Globals.Logger.Debug($"Mana is low, eating food at slot {ranFood}", LogEntryTag.Food);
                _sim.Keyboard.KeyPress(_activeMPKeys[ranFood]); // food press
                                                                // start the delay timer to press the key again
                StartTimer(MpFoodKeyTimer, (int)(_mpKeyDelay * 1000));
                _eatMPFood = false;
            }
        }

        /// <summary> Timer tick to reset hp food key </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HpFoodKeyTimer_Tick(object sender, EventArgs e)
        {
            _eatHPFood = true;
            StopTimer(HpFoodKeyTimer);
        }

        /// <summary> Timer tick to reset mp food key </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MpFoodKeyTimer_Tick(object sender, EventArgs e)
        {
            _eatMPFood = true;
            StopTimer(MpFoodKeyTimer);
        }

        /// <summary> Timer tick to timeout retarget </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RetargetTimeout_Tick(object sender, EventArgs e)
        {
            _currentXP = Addresses.Xp.GetValue();
            float x = Addresses.PositionX.GetValue();
            float y = Addresses.PositionY.GetValue();

            StopTimer(AttackTimeoutTimer);

            if ((_targetDefeatedMsg.Length == 0 && _currentXP == _xpBeforeKill) || (x == _lastXPos && y == _lastYPos))
            {
                Globals.Logger.Debug($"Attack timed out", LogEntryTag.Combat);
                StopAllCombatRelatedTimers();
                SwitchToTargetting(true);
            }
        }

        /// <summary> Timer tick for combat camera </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CombatCameraTimer_Tick(object sender, EventArgs e)
        {
            if (!Globals.Hooked)
                return;

            CameraZoomLabel.Content = $@"Zoom: {Addresses.CameraZoom.GetValue()}";
            CameraPitchLabel.Content = $@"Pitch: {Addresses.CameraPitch.GetValue()}";
            CameraYawLabel.Content = $@"Yaw: {Addresses.CameraYaw.GetValue()}";

            if (_combatCamera)
            {
                Addresses.CameraZoom.writeValue(CameraMaxZoom);
                Addresses.CameraPitch.writeValue(CameraMaxPitch);
            }
        }

        /// <summary> Timer tick for camera yaw spin </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraYawTimer_Tick(object sender, EventArgs e)
        {
			if (!Globals.Hooked || !_timedCameraYaw)
                return;

            float waveform = (float)((Math.PI / 1.1) * Math.Sin(0.25 * _yawCounter));
            Addresses.CameraYaw.writeValue(waveform);
            _yawCounter += 0.05;

            _rightClickCounter++;

            if (_rightClickCounter > 50)
            {
                _rightClickCounter = 0;
                _sim.Mouse.VerticalScroll(-1);
            }
        }

        #endregion
    }
}