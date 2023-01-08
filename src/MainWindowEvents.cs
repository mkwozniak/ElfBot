﻿using System;
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
    private void LoadMonsterTable(object sender, RoutedEventArgs e)
    {
	    var dialog = new OpenFileDialog();
	    dialog.Filter = "Plain Text (*.txt)|*.txt";
	    if (dialog.ShowDialog() is not true) return;

	    try
	    {
		    using var reader = new StreamReader(dialog.FileName);
		    var contents = reader.ReadToEnd();
		    var monsters = contents.Split(',');

		    ApplicationContext.MonsterTable.Clear();
		    ApplicationContext.MonsterTable.UnionWith(monsters);

		    MonsterTableText.Content = monsters.Length == 0 ? "Empty" : string.Join("\n", monsters);
	    }
	    catch (System.Security.SecurityException ex)
	    {
		    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
		                    $"Details:\n\n{ex.StackTrace}");
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
			Logger.Warn("Failed to show save configuration dialog");
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
			Logger.Warn("Failed to show open configuration dialog");
			return;
		}

		try
		{
			using var reader = new StreamReader(dialog.FileName);
			var json = reader.ReadToEnd();
			var settings = JsonConvert.DeserializeObject<Settings>(json);

			if (settings is null)
			{
				Logger.Warn($"Settings file had no data");
				return;
			}
			ApplicationContext.Settings = settings;
		}
		catch (Exception ex) 
		{
			Logger.Error($"Failed to load config file: {ex.Message}");
		}
	}

	/// <summary>
	/// Navigates to another panel when a properly configured button
	/// is clicked. The button must have the 'Tag' attribute set
	/// and bound to the target Panel.
	/// </summary>
	private void NavigatePanel(object sender, RoutedEventArgs e)
	{
		if (sender is not Button { Tag: UIElement target }) return;
		HideAllPanels();
		target.Visibility = Visibility.Visible;
	}

	/// <summary> Clears the log </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void ClearLogsButton_Click(object sender, EventArgs e)
	{
		//SendKey(0x09);
		SendKey(0x31);
		Logger.Clear();
		SystemMsgLog.Content = "";
	}

	#endregion
	
	#region Checkbox Events

	private void StartAutoCombat(object sender, RoutedEventArgs e)
	{
		ApplicationContext.AutoCombat.Start();
	}

	private void StopAutoCombat(object sender, RoutedEventArgs e)
	{
		ApplicationContext.AutoCombat.Stop();
	}
	
	private void EnableHpTimer(object sender, RoutedEventArgs e)
	{
		StopTimer(HpFoodTimer); // just in case it's already running
		StartTimer(HpFoodTimer, (int) (Settings.FoodOptions.CheckFrequency * 1000));
		Logger.Info("Enabled auto-HP food consumption", LogEntryTag.Food);
	}

	private void DisableHpTimer(object sender, RoutedEventArgs e)
	{
		StopTimer(HpFoodTimer);
		Logger.Info("Disabled auto-HP food consumption", LogEntryTag.Food);
	}

	private void EnableMpTimer(object sender, RoutedEventArgs e)
	{
		StopTimer(MpFoodTimer); // just in case it's already running
		StartTimer(MpFoodTimer, (int) (Settings.FoodOptions.CheckFrequency * 1000));
		Logger.Info("Enabled auto-MP food consumption", LogEntryTag.Food);
	}

	private void DisableMpTimer(object sender, RoutedEventArgs e)
	{
		StopTimer(MpFoodTimer);
		Logger.Info("Disabled auto-MP food consumption", LogEntryTag.Food);
	}

	private void EnableZHack(object sender, RoutedEventArgs e)
    {
	    StopTimer(ZHackTimer); 
	    StartTimer(ZHackTimer, (int)(Settings.ZHackOptions.Frequency * 1000));
	    Logger.Info("Enabled Timed ZHack", LogEntryTag.Combat);
    }

    private void DisableZHack(object sender, RoutedEventArgs e)
    {
	    StopTimer(ZHackTimer);
	    Logger.Info("Disabled Timed ZHack", LogEntryTag.Combat);
    }

	#endregion

}