using System;
using System.IO;
using System.Windows.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace ElfBot;

using System.Windows;

public sealed partial class MainWindow 
{
    #region Button Events

    /// <summary> Tries to hook process </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HookBtn_Click(object sender, RoutedEventArgs e)
    {
	    TryOpenProcess();
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


	#endregion

}