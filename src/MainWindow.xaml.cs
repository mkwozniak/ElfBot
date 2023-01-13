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

using System.Diagnostics;
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
		{ "=", 0xBB }
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
		// Reset visibility & colors of all panels + navigation buttons
		CombatOptionsPanel.Visibility = Visibility.Hidden;
		FoodOptionsPanel.Visibility = Visibility.Hidden;
		MonsterTablePanel.Visibility = Visibility.Hidden;
		KeybindOptionsPanel.Visibility = Visibility.Hidden;
		LoggingOptionsPanel.Visibility = Visibility.Hidden;
		ZHackOptionsPanel.Visibility = Visibility.Hidden;
		BuffOptionsPanel.Visibility = Visibility.Hidden;
		AutoCombatPanelButton.Background = Brushes.Black;
		MonsterTablePanelButton.Background = Brushes.Black;
		FoodOptionsPanelButton.Background = Brushes.Black;
		BuffOptionsPanelButton.Background = Brushes.Black;
		ZHackPanelButton.Background = Brushes.Black;
		KeybindingsPanelButton.Background = Brushes.Black;
		LogsViewPanelButton.Background = Brushes.Black;
		BuffOptionsPanel.Background = Brushes.Black;
		// Show the newly selected panel and highlight the button to indicate it is selected
		target.Visibility = Visibility.Visible;
		((Button)sender).Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#468cc0")!;
	}

	/// <summary>
	/// Called when the hook button is clicked. Attempts to find the
	/// ROSE (trose.exe) application and access its memory.
	/// </summary>
	private void HookApplication(object sender, RoutedEventArgs e)
	{
		var brushConverter = new BrushConverter();
		Process process = RoseProcess.GetProcess(ApplicationContext.UseSecondClient);
		
		if (process == null)
		{
			Logger.Warn($"The ROSE process was not found when attempting to hook trose.exe", LogEntryTag.System);
			HookBtn.Content = "F A I L E D";
			HookBtn.Background = (SolidColorBrush)brushConverter.ConvertFromString("#D60B00")!;
			return;
		}

		TargetApplicationMemory.OpenProcess(process.Id);
		RoseProcess.HookedProcess = process;
		ApplicationContext.Hooked = true;
		HookBtn.Content = "H O O K E D";
		HookBtn.Background = (SolidColorBrush)brushConverter.ConvertFromString(" #1F7D1F ")!;
		Logger.Info($"Hooked ROSE process with PID {process.Id}", LogEntryTag.System);
	}

	private void _onRoseUnhook()
	{
		HookBtn.Content = "H O O K";
		HookBtn.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(" #36719E ")!;
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

/// <summary>
/// Delegate method to call once the ROSE process unhooks.
/// </summary>
public delegate void OnRoseUnhook();

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
	private Character _hookedCharacterMemRef = new();
	public OnRoseUnhook? RoseUnhookDelegate { get; set; }

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
		UiData = new UiData(this);
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

	public Character? ActiveCharacter => Hooked && _hookedCharacterMemRef.IsValid() ? _hookedCharacterMemRef : null;

	public UiData UiData { get; }

	public AutoCombat AutoCombat { get; }
	
	public AutoFood AutoFood { get; }

	public bool Hooked
	{
		get
		{
			if (RoseProcess.HookedProcess == null) return false;
			// ReSharper disable once InvertIf
			if (RoseProcess.HookedProcess.HasExited)
			{
				RoseProcess.HookedProcess = null;
				Hooked = false;
				return false;
			}
			return true;
		}
		set
		{
			if (_hooked == value) return;
			_hooked = value;
			if (!value && RoseUnhookDelegate != null) 
				RoseUnhookDelegate.Invoke();
			NotifyPropertyChanged();
		}
	}

	public bool UseSecondClient { get; set; }
}

public class UiData : PropertyNotifyingClass
{
	private readonly ApplicationContext _context;

	public string Name => _context.ActiveCharacter?.Name ?? "N/A";
	public int Level => _context.ActiveCharacter?.Level ?? -1;
	public int Xp => _context.ActiveCharacter?.Xp ?? -1;
	public int Zuly => _context.ActiveCharacter?.Zuly ?? -1;
	public string HpText => $"{_context.ActiveCharacter?.Hp ?? 0} / {_context.ActiveCharacter?.MaxHp ?? 0}";
	public string MpText => $"{_context.ActiveCharacter?.Mp ?? 0} / {_context.ActiveCharacter?.MaxMp ?? 0}";
	public string? CurrentTargetName => _context.ActiveCharacter?.TargetName ?? "N/A";
	public int LastTargetId => _context.ActiveCharacter?.LastTargetId ?? -1;
	public float PositionX => _context.ActiveCharacter?.PositionX ?? 0f;
	public float PositionY => _context.ActiveCharacter?.PositionY ?? 0f;
	public float PositionZ => _context.ActiveCharacter?.PositionZ ?? 0f;
	public int MapId => _context.ActiveCharacter?.MapId ?? -1;
	public float CameraZoom => _context.ActiveCharacter?.Camera.Zoom ?? 0f;
	public float CameraPitch => _context.ActiveCharacter?.Camera.Pitch ?? 0f;
	public float CameraYaw => _context.ActiveCharacter?.Camera.Yaw ?? 0f;

	public UiData(ApplicationContext context)
	{
		_context = context;
	}

	public void Update()
	{
		foreach (var property in GetType().GetProperties())
		{
			NotifyPropertyChanged(property.Name);
		}
	}
}