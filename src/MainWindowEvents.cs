namespace ElfBot;

using System.Windows;
using EventArgs = System.EventArgs;
using CheckBox = System.Windows.Controls.CheckBox;
using System.Diagnostics;

public sealed partial class MainWindow : Window
{
    #region Button Events

    /// <summary> Tries to hook process </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HookBtn_Click(object sender, RoutedEventArgs e)
    {
        if (!TryOpenProcess()) 
        { 
            return; 
        }
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
                    Globals.Logger.Debug($"Added active combat key {_keyMap[i].ToString()}", 
                        LogEntryTag.System);
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
                    Globals.Logger.Debug($"Added active HP key {_keyMap[i].ToString()}", 
                        LogEntryTag.System);
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
                    Globals.Logger.Debug($"Added active MP key {_keyMap[i].ToString()}", 
                        LogEntryTag.System);
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

	/// <summary> Loads a config to file </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void SaveConfigBtn_Click(object sender, RoutedEventArgs e)
	{
		if (_config == null)
            return;
        _config.TrySaveConfigToFile(GenerateConfigData());
	}

	/// <summary> Saves the current config to file </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void LoadConfigBtn_Click(object sender, RoutedEventArgs e)
	{
		if (_config == null)
			return;
		_config.LoadConfigFromFile();
	}

	/// <summary> Opens the control panel </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void ControlPanelBtn_Click(object sender, RoutedEventArgs e)
	{
		HideAllPanels();
		ControlPanel.Visibility = Visibility.Visible;
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

	/// <summary> Opens the zhack options menu </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void ZHackOptionsBtn_Click(object sender, RoutedEventArgs e)
	{
		HideAllPanels();
		ZHackOptionsPanel.Visibility = Visibility.Visible;
	}

	/// <summary> Clears the log </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void ClearLogsButton_Click(object sender, EventArgs e)
	{
		Globals.Logger.Clear();
		SystemMsgLog.Content = "";
	}

	#endregion

	#region InputBox Events

	// All inputbox inputchanged events

	private void lootTimeInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
		Util.TryFloatFromInputBox(CombatLootTimeInputBox, ref _combatLootSeconds);
    }

    private void actionDelayInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
		Util.TryFloatFromInputBox(ActionDelayInputBox, ref _actionDelay);
    }

    private void RetargetTimeoutInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
		Util.TryFloatFromInputBox(RetargetTimeoutInputBox, ref _retargetTimeout);
    }

    private void CombatKeyDelayInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
		Util.TryFloatFromInputBox(CombatKeyDelayInputBox, ref _combatKeyDelay);
    }

    private void FoodDelayInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
		Util.TryFloatFromInputBox(FoodDelayInputBox, ref _foodDelay);
    }

    private void EatKeyDelayInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
		Util.TryFloatFromInputBox(EatKeyDelayInputBox, ref _hpKeyDelay);
		Util.TryFloatFromInputBox(EatKeyDelayInputBox, ref _mpKeyDelay);
    }

    private void FoodMpPercentInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
		Util.TryFloatFromInputBox(FoodMpPercentInputBox, ref _currentFoodMPThreshold, true);
    }

    private void FoodHpPercentInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
		Util.TryFloatFromInputBox(FoodHpPercentInputBox, ref _currentFoodHPThreshold, true);
    }

	private void TimedZHackDelayInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
	{
		Util.TryFloatFromInputBox(TimedZHackDelayInputBox, ref _zHackDelay);
	}

	private void TimedZHackAmountInputBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
	{
		Util.TryFloatFromInputBox(TimedZHackAmountInputBox, ref _zHackDelayAmount);
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
            _autoHP = true;
            return;
        }

        Globals.Logger.Info("Disabled auto-HP food consumption", LogEntryTag.Food);
		StopTimer(HpFoodTimer);
		_autoHP = false;
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
			_autoMP = true;
			return;
        }

        Globals.Logger.Info("Disabled auto-MP food consumption", LogEntryTag.Food);
		StopTimer(MpFoodTimer);
		_autoMP = false;
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

	/// <summary> Enables/disables combat looting </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void CombatLootCheckbox_Checked(object sender, RoutedEventArgs e)
	{
		if (CombatLootCheckbox.IsChecked == true)
		{
			Globals.Logger.Info("Enabled combat loot", LogEntryTag.Combat);
			_combatLoot = true;
			return;
		}

		Globals.Logger.Info("Disabled combat loot", LogEntryTag.Combat);
		_combatLoot = false;
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

    /// <summary> Enables / disables timed zhack </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	private void TimedZHackCheckBox_Checked(object sender, RoutedEventArgs e)
	{
		if (!Globals.Hooked)
			return;

		if (!TimedZHackCheckBox.IsChecked == true)
		{
			Globals.Logger.Debug("Disabled Timed ZHack", LogEntryTag.Combat);
			StopTimer(ZHackTimer);
			return;
		}

		Globals.Logger.Info("Enabled Timed ZHack", LogEntryTag.Combat);
		if (!ZHackTimer.IsEnabled)
			StartTimer(ZHackTimer, (int)(_zHackDelay * 1000));
	}

	#endregion

}