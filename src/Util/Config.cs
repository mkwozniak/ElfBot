namespace ElfBot;

using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using ConfigFloatCallbackDict = System.Collections.Generic.Dictionary<string, SettingConfigFloat>;
using ConfigBoolCallbackDict = System.Collections.Generic.Dictionary<string, SettingConfigBool>;
using ConfigKeyCallbackDict = System.Collections.Generic.Dictionary<string, SettingConfigKey>;
using File = System.IO.File;
using System.Windows;
using System.Diagnostics;
using WindowsInput.Native;

public sealed class Config
{
	private const string _configFilter = "Config files(*.cfg)| *.cfg";

	private SaveFileDialog? _saveConfigDialog;
	private OpenFileDialog? _loadConfigDialog;
	public ConfigFloatCallbackDict FloatCallbacks = new ConfigFloatCallbackDict();
	public ConfigBoolCallbackDict BoolCallbacks = new ConfigBoolCallbackDict();
	public ConfigKeyCallbackDict KeyCallbacks = new ConfigKeyCallbackDict();

	public Config()
	{
		_saveConfigDialog = new SaveFileDialog();
		_saveConfigDialog.Filter = _configFilter;
	}

	/// <summary> Tries to save string data to a config file </summary>
	/// <param name="configData"> The config data. </param>
	/// <returns> Returns false is the file fails. </returns>
	public bool TrySaveConfigToFile(string configData)
	{
		_saveConfigDialog = new SaveFileDialog();
		_saveConfigDialog.Filter = _configFilter;

		if (_saveConfigDialog.ShowDialog() == true)
		{
			File.WriteAllText(_saveConfigDialog.FileName, configData);
			return true;
		}

		return false;
	}

	/// <summary> Loads raw config data from file. </summary>
	public void LoadConfigFromFile()
	{
		_loadConfigDialog = new OpenFileDialog();
		_loadConfigDialog.Filter = _configFilter;
		if (_loadConfigDialog?.ShowDialog() == true)
		{
			try
			{
				var filePath = _loadConfigDialog.FileName;
				var sr = new System.IO.StreamReader(_loadConfigDialog.FileName);
				string contents = sr.ReadToEnd();
				string[] configOptions = contents.Split(';');
				if (configOptions.Length > 0) 
				{
					ParseAndLoadConfigData(configOptions);
				}
			}
			catch (System.Security.SecurityException ex)
			{
				Globals.Logger.Error("Failed to load config file");
				MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
				$"Details:\n\n{ex.StackTrace}");
			}
		}
	}

	/// <summary> Parses and loads large split config data. </summary>
	/// <param name="data"></param>
	private void ParseAndLoadConfigData(string[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			string[] mainSplit = data[i].Split(':');
			if (mainSplit.Length < 2) continue;

			string[] pairSplit = mainSplit[1].Split('=');
			if (pairSplit.Length < 2) continue;

			string valueType = mainSplit[0].Replace("\n", "").Replace("\r", "");
			string valueName = pairSplit[0];
			string valueEntry = pairSplit[1];

			if (valueType == "f")
			{
				LoadFloatFromConfig(valueName, valueEntry);
				continue;
			}

			if (valueType == "b")
			{
				LoadBoolFromConfig(valueName, valueEntry);
				continue;
			}

			if (valueType == "k")
			{
				string[] keys = valueEntry.Replace("\n", "").Replace("\r", "").Split(',');
				for(int j = 0; j < keys.Length; j++)
				{
					Trace.WriteLine($"{keys[j]}");
					LoadKeyFromConfig(valueName, keys[j]);
				}
			}
		}
	}

	/// <summary> Checks a float config entry and invokes the callback if valid and exists </summary>
	/// <param name="valueName"></param>
	/// <param name="entry"></param>
	private void LoadFloatFromConfig(string valueName, string entry)
	{
		float floatEntry;
		bool isValidFloat = float.TryParse(entry, out floatEntry);
		if (isValidFloat && FloatCallbacks.ContainsKey(valueName))
		{
			FloatCallbacks[valueName]?.Invoke(valueName, floatEntry);
		}
	}

	/// <summary> Checks a bool config entry and invokes the callback if valid and exists </summary>
	/// <param name="valueName"></param>
	/// <param name="entry"></param>
	private void LoadBoolFromConfig(string valueName, string entry)
	{
		bool value;
		if (entry == "True")
			value = true;
		else if (entry == "False")
			value = false;
		else
			return;

		if (BoolCallbacks.ContainsKey(valueName))
		{
			BoolCallbacks[valueName]?.Invoke(valueName, value);
		}
	}

	/// <summary> Checks a key config entry and invokes the callback if it exists </summary>
	/// <param name="valueName"></param>
	/// <param name="entry"></param>
	private void LoadKeyFromConfig(string valueName, string entry)
	{
		if (KeyCallbacks.ContainsKey(valueName) && Globals.ConfigKeyEntries.ContainsKey(entry))
		{
			KeyCallbacks[valueName]?.Invoke(valueName, Globals.ConfigKeyEntries[entry]);
		}
	}
}