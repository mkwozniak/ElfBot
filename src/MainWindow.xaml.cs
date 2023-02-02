﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using ElfBot.Util;
using Memory;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace ElfBot;

using System.Diagnostics;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	private ApplicationContext ApplicationContext => (TryFindResource("ApplicationContext") as ApplicationContext)!;
	private Settings Settings => ApplicationContext.Settings;

	public static readonly Mem TargetApplicationMemory = new();
	public static readonly Logger Logger = new();

	// Timers
	private DispatcherTimer InterfaceTimer = new()
	{
		Interval = TimeSpan.FromMilliseconds(60)
	};

	// Structures

	public static readonly Dictionary<string, Messaging.VKeys> KeyMap = new()
	{
		{ "1", Messaging.VKeys.KEY_1 },
		{ "2", Messaging.VKeys.KEY_2 },
		{ "3", Messaging.VKeys.KEY_3 },
		{ "4", Messaging.VKeys.KEY_4 },
		{ "5", Messaging.VKeys.KEY_5 },
		{ "6", Messaging.VKeys.KEY_6 },
		{ "7", Messaging.VKeys.KEY_7 },
		{ "8", Messaging.VKeys.KEY_8 },
		{ "9", Messaging.VKeys.KEY_9 },
		{ "0", Messaging.VKeys.KEY_0 },
		{ "-", Messaging.VKeys.KEY_MINUS }, 
		{ "=", Messaging.VKeys.KEY_PLUS } 
	};

	// Values
	private int _yawMouseScrollCounter = 0;
	private int _yawMouseScrollCounterMax = 50;
	private double _yawCounter = 0;
	private double _yawCounterIncrement = 0.05;
	
	
	/// <summary> Form Constructor </summary>
	public MainWindow()
	{
		Closed += _exit;
		Loaded += _init;
		ApplicationContext.RoseUnhookDelegate += _onRoseUnhook;
		InitializeComponent();
	}

	private void _exit(object? sender, EventArgs e)
	{
		RoseProcess.DisableNoClip();
		InterfaceTimer.Stop();
		ApplicationContext.CombatCameraTimer.Stop();
		ApplicationContext.CameraYawTimer.Stop();
	}

	private void _init(object sender, RoutedEventArgs e)
	{
		InterfaceTimer.Tick += Interface_Tick;
		ApplicationContext.CombatCameraTimer.Tick += CombatCameraTimer_Tick;
		ApplicationContext.CameraYawTimer.Tick += CameraYawTimer_Tick;
		ApplicationContext.ZHackTimer.Tick += ZHackTimer_Tick;

		InterfaceTimer.Start();
		ApplicationContext.CombatCameraTimer.Start();
		ApplicationContext.CameraYawTimer.Start();
	}
	
	/// <summary>
	/// Navigates to another panel when a properly configured button
	/// is clicked. The button must have the 'Tag' attribute set
	/// and bound to the target Panel.
	/// </summary>
	private void NavigatePanel(object sender, RoutedEventArgs e)
	{
		if (sender is not Button { Tag: UIElement target } button) return;
		// Reset visibility & colors of all panels + navigation buttons
		GeneralOptionsPanel.Visibility = Visibility.Hidden;
		CombatOptionsPanel.Visibility = Visibility.Hidden;
		AutoClericPanel.Visibility = Visibility.Hidden;
		FoodOptionsPanel.Visibility = Visibility.Hidden;
		KeybindOptionsPanel.Visibility = Visibility.Hidden;
		LoggingOptionsPanel.Visibility = Visibility.Hidden;
		AutoCombatPanelButton.Background = Brushes.Black;
		AutoClericPanelButton.Background = Brushes.Black;
		FoodOptionsPanelButton.Background = Brushes.Black;
		GeneralPanelButton.Background = Brushes.Black;
		KeybindingsPanelButton.Background = Brushes.Black;
		LogsViewPanelButton.Background = Brushes.Black;
		// Show the newly selected panel and highlight the button to indicate it is selected
		target.Visibility = Visibility.Visible;
		button.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#468cc0")!;
	}

	/// <summary>
	/// Called when the hook button is clicked. Attempts to find the
	/// ROSE (trose.exe) application and access its memory.
	/// </summary>
	private void HookApplication(object sender, RoutedEventArgs e)
	{
		var brushConverter = new BrushConverter();
		Process? process = RoseProcess.GetProcess(ApplicationContext.UseSecondClient);
		
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

		if (settings.LastMonsterTableLocation != null)
		{
			var loaded = ApplicationContext.LoadMonsterTable(settings.LastMonsterTableLocation);
			if (!loaded) settings.LastMonsterTableLocation = null;
		}

		ApplicationContext.Settings = settings;
	}
	
    private void Interface_Tick(object? sender, EventArgs e)
    {
	    if (LoggingOptionsPanel.Visibility == Visibility.Visible)
        {
            _refreshLogs();
        }

        if (!ApplicationContext.Hooked)
        {
            return;
        }

        ApplicationContext.UiData.Update();
    }

    /// <summary> Refreshes the log </summary>
    private void _refreshLogs()
    {
        var logEntries = Logger.Entries.Where(e => (int)e.Level >= (int)Settings.SelectedLogLevel).ToList();
        
        var displayedLog = LogsViewPanel.SystemMsgLog.Text;
        if (displayedLog is not string)
        {
            LogsViewPanel.SystemMsgLog.Text = "";
        }

        if (logEntries.Count == 0)
        {
            LogsViewPanel.SystemMsgLog.Text = "";
            return;
        }

        var lines = logEntries.Select(entry =>
        {
            var date = entry.TimeStamp.ToString("hh:mm:ss tt");
            return $"({date}) {entry.Level}: {entry.Text}";
        }).ToArray();
        LogsViewPanel.SystemMsgLog.Text = string.Join(Environment.NewLine, lines);
    }

    private void CombatCameraTimer_Tick(object? sender, EventArgs e)
    {
        if (ApplicationContext.ActiveCharacter == null)
            return;

        if (Settings.GeneralOptions.LockCameraPitch)
        {
            ApplicationContext.ActiveCharacter.Camera.Pitch = Settings.GeneralOptions.LockedCameraPitchAmount;
        }
        if (Settings.GeneralOptions.LockCameraZoom)
        {
            ApplicationContext.ActiveCharacter.Camera.Zoom = Settings.GeneralOptions.LockedCameraZoomAmount;
        }
    }

    private void CameraYawTimer_Tick(object? sender, EventArgs e)
    {
		if (ApplicationContext.ActiveCharacter == null || !Settings.CombatOptions.CameraYawWaveEnabled) return;

        var waveform = (float)(Math.PI * Math.Sin(0.25 * _yawCounter));
        ApplicationContext.ActiveCharacter.Camera.Yaw = waveform;
        _yawCounter += _yawCounterIncrement;

        _yawMouseScrollCounter++;

        if (_yawMouseScrollCounter > _yawMouseScrollCounterMax)
        {
            _yawMouseScrollCounter = 0;
            ApplicationContext.ActiveCharacter.Camera.Zoom += 10f;
        }
    }

	private void ZHackTimer_Tick(object? sender, EventArgs e)
	{
		if (ApplicationContext.ActiveCharacter == null)
		{
			Settings.ZHackOptions.Enabled = false;
			ApplicationContext.ZHackTimer.Stop();
			Logger.Warn("ZHack was disabled because an active character could not be found");
			return;
		}

		if (ApplicationContext.ActiveCharacter.IsDead)
		{
			if (ApplicationContext.Settings.GeneralOptions.DeathAction != DeathActions.CANCEL_TIMERS) return;
			Trace.WriteLine("Canceled z-hack timer due to player death");
			MainWindow.Logger.Warn("Disabling ZHack due to player death");
			Settings.ZHackOptions.Enabled = false;
			ApplicationContext.ZHackTimer.Stop();
			return;
		}
		
		if (ApplicationContext.ActiveCharacter.IsOnMount)
		{
			return;
		}
		
		ApplicationContext.ActiveCharacter.PositionZ += Settings.ZHackOptions.Amount;
	}
}

public class UiData : PropertyNotifyingClass
{
	private readonly ApplicationContext _context;

	#region Enablement information

	public bool CanEnableAutoCombat => _canTimersBeEnabled();

	public bool CanEnableZHack => _canTimersBeEnabled();

	public bool CanEnableAutoFood => _canTimersBeEnabled();

	private bool _canTimersBeEnabled()
	{
		if (_context.ActiveCharacter == null) return false;
		return !_context.ActiveCharacter.IsDead ||
		       _context.Settings.GeneralOptions.DeathAction == DeathActions.PAUSE_TIMERS;
	}

	#endregion
	
	#region Character Data
	public string Name => _context.ActiveCharacter?.Name ?? "N/A";
	public int Level => _context.ActiveCharacter?.Level ?? -1;
	public int Xp => _context.ActiveCharacter?.Xp ?? -1;
	public int Zuly => _context.ActiveCharacter?.Zuly ?? -1;
	public string HpText => $"{_context.ActiveCharacter?.Hp ?? 0} / {_context.ActiveCharacter?.MaxHp ?? 0}";
	public string MpText => $"{_context.ActiveCharacter?.Mp ?? 0} / {_context.ActiveCharacter?.MaxMp ?? 0}";
	public string? CurrentTargetName => _context.ActiveCharacter?.TargetName ?? "N/A";
	public int LastTargetId => _context.ActiveCharacter?.LastTargetId ?? -1;
	public double PositionX => Math.Round(_context.ActiveCharacter?.PositionX ?? 0f, 2);
	public double PositionY => Math.Round(_context.ActiveCharacter?.PositionY ?? 0f, 2);
	public double PositionZ => Math.Round(_context.ActiveCharacter?.PositionZ ?? 0f, 2);
	public int MapId => -1;
	public float CameraZoom => _context.ActiveCharacter?.Camera.Zoom ?? 0f;
	public float CameraPitch => _context.ActiveCharacter?.Camera.Pitch ?? 0f;
	public float CameraYaw => _context.ActiveCharacter?.Camera.Yaw ?? 0f;
	
	#endregion

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