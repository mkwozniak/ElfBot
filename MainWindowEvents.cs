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
                        LogDateMsg("Added Active Key: " + _keyMap[i].ToString(), LogTypes.System);
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
                        LogDateMsg("Added Active HP Key: " + _keyMap[i].ToString(), LogTypes.System);
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
                        LogDateMsg("Added Active MP Key: " + _keyMap[i].ToString(), LogTypes.System);
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
                LogDateMsg("Disabled AutoCombat", LogTypes.System);
                return;
            }

            if (_monsterTable?.Count == 0)
            {
                LogDateMsg("Error: Empty Monstertable", LogTypes.System);
                AutoCombatCheckBox.IsChecked = false;
                return;
            }

            LogDateMsg("Enabled AutoCombat", LogTypes.System);
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
                LogDateMsg("Enabled Auto Food HP", LogTypes.System);
                StartTimer(HpFoodTimer, (int)(_foodDelay * 1000));
                return;
            }

            LogDateMsg("Disabled Auto Food HP", LogTypes.System);
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
                LogDateMsg("Enabled Auto Food MP", LogTypes.System);
                StartTimer(MpFoodTimer, (int)(_foodDelay * 1000));
                return;
            }

            LogDateMsg("Disabled Auto Food MP", LogTypes.System);
            StopTimer(MpFoodTimer);
        }

		/// <summary> Enables/disables force max camera zoom </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CombatCameraCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if (CombatCameraCheckBox.IsChecked == true)
			{
				LogDateMsg("Enabled Combat Camera", LogTypes.System);
				_combatCamera = true;
				return;
			}

			LogDateMsg("Disabled Combat Camera", LogTypes.System);
			_combatCamera = false;
		}

		/// <summary> Enables/disables second client mode </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SecondClientCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (SecondClientCheckBox.IsChecked == true)
            {
                LogDateMsg("Enabled 2nd Client Mode.", LogTypes.System);
                _dualClient = true;
                return;
            }

            LogDateMsg("Disabled 2nd Client Mode.", LogTypes.System);
            _dualClient = false;
        }

		/// <summary> Enables/disables timed camera yaw </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TimedCameraYawCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if (TimedCameraYawCheckBox.IsChecked == true)
			{
				LogDateMsg("Enabled Timed Camera Yaw.", LogTypes.System);
				_timedCameraYaw = true;
				return;
			}

			LogDateMsg("Disabled Timed Camera Yaw.", LogTypes.System);
			_timedCameraYaw = false;
		}

		/// <summary> Enables/disables combat logging </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LogCombatCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			if (LogCombatCheckBox.IsChecked == true)
			{
				IgnoredLogTypes.Remove(LogTypes.Combat);
				return;
			}
			IgnoredLogTypes.Add(LogTypes.Combat);
		}

		/// <summary> Enables/disables food logging </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LogFoodCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (LogFoodCheckBox.IsChecked == true && IgnoredLogTypes.Contains(LogTypes.Food))
            {
				IgnoredLogTypes.Remove(LogTypes.Food);
				return;
            }
			IgnoredLogTypes.Add(LogTypes.Food);
        }

        /// <summary> Enables/disables camera logging </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogCameraCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (LogCameraCheckBox.IsChecked == true)
            {
				IgnoredLogTypes.Remove(LogTypes.Camera);
                return;
            }
			IgnoredLogTypes.Add(LogTypes.Camera);
		}

        #endregion

    }
}