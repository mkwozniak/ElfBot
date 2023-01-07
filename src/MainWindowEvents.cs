using System;
using System.IO;
using System.Windows.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace ElfBot;

using System.Windows;
using EventArgs = System.EventArgs;

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
	private void SaveConfiguration(object sender, RoutedEventArgs e)
	{
		var dialog = new SaveFileDialog()
		{
			Filter = "JSON (*.json)|*.json",
			FileName = "config.json"
		};
		
		if (dialog.ShowDialog() == true)
		{
			var json = JsonConvert.SerializeObject(ApplicationContext.Settings, Formatting.Indented);
			File.WriteAllText(dialog.FileName, json);
		}
		else
		{
			Globals.Logger.Warn("Failed to show save configuration dialog");
		}
	}

	/// <summary> Saves the current config to file </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void LoadConfiguration(object sender, RoutedEventArgs e)
	{
		var dialog = new OpenFileDialog()
		{
			Filter = "Json files (*.json)|*.json"
		};

		if (!dialog.ShowDialog() == true)
		{
			Globals.Logger.Warn("Failed to show open configuration dialog");
			return;
		}

		try
		{
			using var reader = new StreamReader(dialog.FileName);
			var json = reader.ReadToEnd();
			var settings = JsonConvert.DeserializeObject<Settings>(json);

			if (settings is null)
			{
				Globals.Logger.Warn($"Settings file had no data");
				return;
			}
			ApplicationContext.Settings = settings;
		}
		catch (Exception ex) 
		{
			Globals.Logger.Error($"Failed to load config file: {ex.Message}");
		}
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
	
	#region Checkbox Events

	private void EnableAutoCombat(object sender, RoutedEventArgs e)
	{
		StopAllCombatRelatedTimers();
		if (_monsterTable?.Count == 0)
		{
			Globals.Logger.Error("Could not enable auto-combat due to empty monster table", 
				LogEntryTag.System);
			if (sender is CheckBox c) c.IsChecked = false;
			return;
		}
		
		Globals.Logger.Info("Enabled auto-combat", LogEntryTag.Combat);
		_xpBeforeKill = -1;
		SwitchToTargetting();
	}

	private void DisableAutoCombat(object sender, RoutedEventArgs e)
	{
		StopAllCombatRelatedTimers();
		AutoCombatState.Content = "Combat State: Inactive";
		Globals.Logger.Debug("Disabled auto-combat", LogEntryTag.Combat);
	}

	private void EnableHpTimer(object sender, RoutedEventArgs e)
	{
		StopTimer(HpFoodTimer); // just in case it's already running
		StartTimer(HpFoodTimer, (int) (Settings.FoodOptions.CheckFrequency * 1000));
		Globals.Logger.Info("Enabled auto-HP food consumption", LogEntryTag.Food);
	}

	private void DisableHpTimer(object sender, RoutedEventArgs e)
	{
		StopTimer(HpFoodTimer);
		Globals.Logger.Info("Disabled auto-HP food consumption", LogEntryTag.Food);
	}

	private void EnableMpTimer(object sender, RoutedEventArgs e)
	{
		StopTimer(MpFoodTimer); // just in case it's already running
		StartTimer(MpFoodTimer, (int) (Settings.FoodOptions.CheckFrequency * 1000));
		Globals.Logger.Info("Enabled auto-MP food consumption", LogEntryTag.Food);
	}

	private void DisableMpTimer(object sender, RoutedEventArgs e)
	{
		StopTimer(MpFoodTimer);
		Globals.Logger.Info("Disabled auto-MP food consumption", LogEntryTag.Food);
	}

	private void EnableZHack(object sender, RoutedEventArgs e)
    {
	    StopTimer(ZHackTimer); 
	    StartTimer(ZHackTimer, (int)(Settings.ZHackOptions.Frequency * 1000));
	    Globals.Logger.Info("Enabled Timed ZHack", LogEntryTag.Combat);
    }

    private void DisableZHack(object sender, RoutedEventArgs e)
    {
	    StopTimer(ZHackTimer);
	    Globals.Logger.Info("Disabled Timed ZHack", LogEntryTag.Combat);
    }

	#endregion

}