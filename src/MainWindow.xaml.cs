using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Memory;

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

	// Events
	private FinishedHooking? OnFinishHook;

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