using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Memory;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace ElfBot;

using System.Runtime.InteropServices;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	public ApplicationContext? ApplicationContext => TryFindResource("ApplicationContext") as ApplicationContext;
	public Settings? Settings => ApplicationContext?.Settings;

	public static readonly Mem TargetApplicationMemory = new();
	public static readonly Logger Logger = new();

	[DllImport("ROSE_Input.dll")]
	private static extern void SendKey(int key);

	[DllImport("ROSE_Input.dll")]
	private static extern void LeftClickOnWin(int x, int y);

	[DllImport("ROSE_Input.dll")]
	private static extern void MouseMove(int x, int y);

	// Timers
	private DispatcherTimer InterfaceTimer = new();

	// References
	public static readonly Random Ran = new();

	// Structures

	public static readonly Dictionary<string, int> KeyMap = new()
	{
		{ "1", 0x31 },
		{ "2", 0x32 },
		{ "3", 0x33 },
		{ "4", 0x34 },
		{ "5", 0x35 },
		{ "6", 0x36 },
		{ "7", 0x37 },
		{ "8", 0x38 },
		{ "9", 0x39 },
		{ "0", 0x30 },
		{ "-", 0xBD },
		{ "=", 0x6B }
	};

	// Values
	private bool _eatHPFood = true;
	private bool _eatMPFood = true;

	private int _playerMaxMP = 0;
	private int _playerMP = 0;
	private int _playerMaxHP = 0;
	private int _playerHP = 0;
	private int _interfaceUpdateTime = 60;
	private int _combatCameraTickTime = 500;
	private int _cameraYawTickTime = 50;
	private int _yawMouseScrollCounter = 0;
	private int _yawMouseScrollCounterMax = 50;

	private const float CameraMaxZoom = 100f;
	private const float CameraMaxPitch = 0.85f;

	private double _yawCounter = 0;
	private double _yawCounterIncrement = 0.05;
	
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

	/// <summary>
	/// Called when the hook button is clicked. Attempts to find the
	/// ROSE (trose.exe) application and access its memory.
	/// </summary>
	private void HookApplication(object sender, RoutedEventArgs e)
	{
		var pid = RoseProcess.GetProcIdFromName("trose", ApplicationContext.UseSecondClient);
		var brushConverter = new BrushConverter();
		
		if (pid <= 0)
		{
			Logger.Warn($"The ROSE process was not found when attempting to hook trose.exe", LogEntryTag.System);
			HookBtn.Content = "F A I L E D";
			HookBtn.Background = (SolidColorBrush)brushConverter.ConvertFromString("#D60B00");
			return;
		}

		TargetApplicationMemory.OpenProcess(pid);
		ApplicationContext.HookedProcessId = pid;
		ApplicationContext.Hooked = true;
		HookBtn.Content = "H O O K E D";
		HookBtn.Background = (SolidColorBrush)brushConverter.ConvertFromString(" #1F7D1F ");
		Logger.Info($"Hooked ROSE process with PID {pid}", LogEntryTag.System);
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
			var json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
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

		Settings? settings;
		try
		{
			using var reader = new StreamReader(dialog.FileName);
			var json = reader.ReadToEnd();
			settings = JsonConvert.DeserializeObject<Settings>(json);
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed to load config file: {ex.Message}");
			return;
		}

		if (settings is null)
		{
			Logger.Warn($"Settings file had no data");
			return;
		}
		
		if (Settings != null)
		{
			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (settings.Keybindings == null || settings.Keybindings.Count == 0)
			{
				settings.Keybindings = Settings.Keybindings;
			}

			// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
			if (settings.ShiftKeybindings == null || settings.ShiftKeybindings.Count == 0)
			{
				settings.ShiftKeybindings = Settings.ShiftKeybindings;
			}
		}

		ApplicationContext.Settings = settings;
	}
}

public class ApplicationContext : PropertyNotifyingClass
{
	private Settings _settings = new()
	{
		Keybindings = new List<HotkeySlot>
		{
			new() { Key = "1", Value = 0 },
			new() { Key = "2", Value = 0 },
			new() { Key = "3", Value = 0 },
			new() { Key = "4", Value = 0 },
			new() { Key = "5", Value = 0 },
			new() { Key = "6", Value = 0 },
			new() { Key = "7", Value = 0 },
			new() { Key = "8", Value = 0 },
			new() { Key = "9", Value = 0 },
			new() { Key = "0", Value = 0 },
			new() { Key = "-", Value = 0 },
			new() { Key = "=", Value = 0 }
		},
		ShiftKeybindings = new List<HotkeySlot>
		{
			new() { Key = "1", Value = 0, IsShift = true },
			new() { Key = "2", Value = 0, IsShift = true },
			new() { Key = "3", Value = 0, IsShift = true },
			new() { Key = "4", Value = 0, IsShift = true },
			new() { Key = "5", Value = 0, IsShift = true },
			new() { Key = "6", Value = 0, IsShift = true },
			new() { Key = "7", Value = 0, IsShift = true },
			new() { Key = "8", Value = 0, IsShift = true },
			new() { Key = "9", Value = 0, IsShift = true },
			new() { Key = "0", Value = 0, IsShift = true },
			new() { Key = "-", Value = 0, IsShift = true },
			new() { Key = "=", Value = 0, IsShift = true }
		}
	};

	private bool _hooked;

	public DispatcherTimer HpFoodTimer = new();
	public DispatcherTimer MpFoodTimer = new();
	public DispatcherTimer HpFoodKeyTimer = new();
	public DispatcherTimer MpFoodKeyTimer = new();
	public DispatcherTimer CombatCameraTimer = new();
	public DispatcherTimer CameraYawTimer = new();
	public DispatcherTimer ZHackTimer = new();

	public readonly HashSet<string> MonsterTable = new();

	public ApplicationContext()
	{
		AutoCombat = new AutoCombat(this);
		AutoFood = new AutoFood(this);
	}

	public Settings Settings
	{
		get => _settings;
		set
		{
			_settings = value;
			NotifyPropertyChanged();
		}
	}

	public CharacterData CharacterData { get; } = new();

	public AutoCombat AutoCombat { get; }
	
	public AutoFood AutoFood { get; }

	public int HookedProcessId { get; set; }

	public bool Hooked
	{
		get => _hooked;
		set
		{
			_hooked = value;
			NotifyPropertyChanged();
		}
	}

	public bool UseSecondClient { get; set; }
}