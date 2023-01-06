namespace ElfBot
{
    using System.Windows;
    using EventArgs = System.EventArgs;
    using InputSimulator = WindowsInput.InputSimulator;
    using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
    using CheckBox = System.Windows.Controls.CheckBox;
    using System;

    public sealed partial class MainWindow : Window
    {
        #region Button Events

        /// <summary> Tries to hook process </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HookBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!TryOpenProcess()) { return; }  // TODO: add error to fail process open
        }

        /// <summary> Updates combat keys </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateCombatKeysBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < CombatKeys.Items.Count; i++)
            {
                CheckBox checkBox = (CheckBox)CombatKeys.Items.GetItemAt(i);

                if (checkBox == null)
                    continue;

                bool? isChecked = checkBox.IsChecked;

                if (isChecked == true)
                {
                    if (!_activeCombatKeys.Contains(_keyMap[i]))
                    {
                        Globals.Logger.Debug($"Added active combat key {_keyMap[i].ToString()}", LogEntryTag.System);
                        _activeCombatKeys.Add(_keyMap[i]);
                    }
                    continue;
                }
                _activeCombatKeys.Remove(_keyMap[i]);
            }
        }

        /// <summary> Updates food keys </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateFoodKeysBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < HpKeys.Items.Count; i++)
            {
                CheckBox checkBox = (CheckBox)HpKeys.Items.GetItemAt(i);

                if (checkBox == null)
                    continue;

                bool? isChecked = checkBox.IsChecked;

                if (isChecked == true)
                {
                    if (!_activeHPKeys.Contains(_keyMap[i]))
                    {
                        Globals.Logger.Debug($"Added active HP key {_keyMap[i].ToString()}", LogEntryTag.System);
                        _activeHPKeys.Add(_keyMap[i]);
                    }
                    continue;
                }
                _activeHPKeys.Remove(_keyMap[i]);
            }

            for (int i = 0; i < MpKeys.Items.Count; i++)
            {
                CheckBox checkBox = (CheckBox)MpKeys.Items.GetItemAt(i);

                if (checkBox == null)
                    continue;

                bool? isChecked = checkBox.IsChecked;

                if (isChecked == true)
                {
                    if (!_activeMPKeys.Contains(_keyMap[i]))
                    {
                        Globals.Logger.Debug($"Added active MP key {_keyMap[i].ToString()}", LogEntryTag.System);
                        _activeMPKeys.Add(_keyMap[i]);
                    }
                    continue;
                }
                _activeMPKeys.Remove(_keyMap[i]);
            }
        }

        /// <summary> Loads a monster table </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadTableBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_openMonsterTableDialog?.ShowDialog() == true)
            {
                try
                {
                    var filePath = _openMonsterTableDialog.FileName;
                    var sr = new System.IO.StreamReader(_openMonsterTableDialog.FileName);
                    string contents = sr.ReadToEnd();
                    string[] monsters = contents.Split(',');
                    if (monsters.Length > 0)
                    {
                        LoadToMonsterTable(monsters);
                    }
                }
                catch (System.Security.SecurityException ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        /// <summary> Opens the combat options menu </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CombatOptionsBtn_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            CombatOptionsPanel.Visibility = Visibility.Visible;
        }

        /// <summary> Options the loot options menu </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LootOptionsBtn_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            LootOptionsPanel.Visibility = Visibility.Visible;
        }

        /// <summary> Opens the food options menu </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FoodOptionsBtn_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            FoodOptionsPanel.Visibility = Visibility.Visible;
        }

        /// <summary> Opens the monster table options menu </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MonsterTableOptionsBtn_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            MonsterTablePanel.Visibility = Visibility.Visible;
        }

        /// <summary> Opens the keybind options menu </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeybindsOptionBtn_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            KeybindOptionsPanel.Visibility = Visibility.Visible;
        }

        /// <summary> Opens the logging options menu </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoggingOptionsBtn_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            LoggingOptionsPanel.Visibility = Visibility.Visible;
        }

        #endregion

        #region InputBox Events

        // All inputbox inputchanged events

        private void lootTimeInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TryFloatFromInputBox(lootTimeInputBox, ref _lootForSeconds);
        }

        private void actionDelayInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TryFloatFromInputBox(actionDelayInputBox, ref _actionDelay);
        }

        private void RetargetTimeoutInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TryFloatFromInputBox(retargetTimeoutInputBox, ref _retargetTimeout);
        }

        private void CombatKeyDelayInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TryFloatFromInputBox(combatKeyDelayInputBox, ref _combatKeyDelay);
        }

        private void FoodDelayInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TryFloatFromInputBox(FoodDelayInputBox, ref _foodDelay);
        }

        private void EatKeyDelayInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TryFloatFromInputBox(EatKeyDelayInputBox, ref _hpKeyDelay);
            TryFloatFromInputBox(EatKeyDelayInputBox, ref _mpKeyDelay);
        }

        private void FoodMpPercentInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TryFloatFromInputBox(FoodMpPercentInputBox, ref _currentFoodMPThreshold, true);
        }

        private void FoodHpPercentInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TryFloatFromInputBox(FoodHpPercentInputBox, ref _currentFoodHPThreshold, true);
        }

        #endregion

        #region Checkbox Events

        /// <summary> Enables/disables auto combat </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoCombatCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!Globals.Hooked)
                return;

            if (!AutoCombatCheckBox.IsChecked == true)
            {
                StopAllCombatRelatedTimers();
                AutoCombatState.Content = "INACTIVE";
                Globals.Logger.Debug("Disabled auto-combat", LogEntryTag.Combat);
                return;
            }

            if (_monsterTable?.Count == 0)
            {
                Globals.Logger.Error("Could not enable auto-combat due to empty monster table",
                    LogEntryTag.System);
                AutoCombatCheckBox.IsChecked = false;
                return;
            }

            Globals.Logger.Info("Enabled auto-combat", LogEntryTag.Combat);
            HpFoodCheckBox.IsEnabled = true;
            MpFoodCheckBox.IsEnabled = true;
            _xpBeforeKill = -1;
            SwitchToTargetting();
        }

        /// <summary> Enables/disables auto hp food </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HpFoodCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!Globals.Hooked)
                return;

            if (HpFoodCheckBox.IsChecked == true)
            {
                Globals.Logger.Info("Enabled auto-HP food consumption", LogEntryTag.Food);
                StartTimer(HpFoodTimer, (int)(_foodDelay * 1000));
                return;
            }

            Globals.Logger.Info("Disabled auto-HP food consumption", LogEntryTag.Food);
            StopTimer(HpFoodTimer);
        }

        /// <summary> Enables/disables auto mp food </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MpFoodCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!Globals.Hooked)
                return;

            if (MpFoodCheckBox.IsChecked == true)
            {
                Globals.Logger.Info("Enabled auto-MP food consumption", LogEntryTag.Food);
                StartTimer(MpFoodTimer, (int)(_foodDelay * 1000));
                return;
            }

            Globals.Logger.Info("Disabled auto-MP food consumption", LogEntryTag.Food);
            StopTimer(MpFoodTimer);
        }

		/// <summary> Enables/disables force max camera zoom </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CombatCameraCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if (CombatCameraCheckBox.IsChecked == true)
			{
                Globals.Logger.Info("Enabled combat camera", LogEntryTag.Camera);
				_combatCamera = true;
				return;
			}

            Globals.Logger.Info("Disabled combat camera", LogEntryTag.Camera);
			_combatCamera = false;
		}

		/// <summary> Enables/disables second client mode </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SecondClientCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (SecondClientCheckBox.IsChecked == true)
            {
                Globals.Logger.Info("Enabled 2nd client mode", LogEntryTag.System);
                _dualClient = true;
                return;
            }

            Globals.Logger.Info("Disabled 2nd client mode", LogEntryTag.System);
            _dualClient = false;
        }

		/// <summary> Enables/disables timed camera yaw </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TimedCameraYawCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if (TimedCameraYawCheckBox.IsChecked == true)
			{
                Globals.Logger.Info("Enabled timed camera yaw", LogEntryTag.System);
				_timedCameraYaw = true;
				return;
			}

            Globals.Logger.Info("Disabled timed camera yaw", LogEntryTag.System);
            _timedCameraYaw = false;
		}
        
        private void ClearLogsButton_Click(object sender, EventArgs e)
        {
            Globals.Logger.Clear();
            SystemMsgLog.Content = "";
        }

        #endregion

    }
}