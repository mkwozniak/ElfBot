using System;
using System.Collections.Generic;
using Memory;

namespace ElfBot;

using System.Windows;
using Random = System.Random;
using KeyList = System.Collections.Generic.List<WindowsInput.Native.VirtualKeyCode>;
using TextBoxDict = System.Collections.Generic.Dictionary<string, System.Windows.Controls.TextBox>;
using CheckBoxDict = System.Collections.Generic.Dictionary<string, System.Windows.Controls.CheckBox>;
using ComboBoxDict = System.Collections.Generic.Dictionary<string, System.Windows.Controls.ComboBox>;
using InputSimulator = WindowsInput.InputSimulator;
using Timer = System.Windows.Threading.DispatcherTimer;

using VirtualKeyCode = WindowsInput.Native.VirtualKeyCode;
using System.Runtime.InteropServices;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public ApplicationContext? ApplicationContext => TryFindResource("ApplicationContext") as ApplicationContext;
	public Settings? Settings => ApplicationContext?.Settings;
	
	public static readonly Mem TargetApplicationMemory = new Mem();
	public static readonly Logger Logger = new Logger();

	[DllImport("ROSE_Input.dll")]
	private static extern void SendKey(int key);

	// Events
	private FinishedHooking? OnFinishHook;

    // Timers
    private Timer InterfaceTimer = new();
	private Timer HpFoodTimer = new();
	private Timer MpFoodTimer = new();
	private Timer HpFoodKeyTimer = new();
	private Timer MpFoodKeyTimer = new();
	private Timer CombatCameraTimer = new();
	private Timer CameraYawTimer = new();
	private Timer ZHackTimer = new();

	// References
	public static readonly InputSimulator Sim = new();
    public static readonly Random Ran = new();

	// Structures

	public static readonly Dictionary<string, VirtualKeyCode> KeyMap = new()
	{
		{ "1", VirtualKeyCode.VK_1 },
		{ "2", VirtualKeyCode.VK_2 },
		{ "3", VirtualKeyCode.VK_3 },
		{ "4", VirtualKeyCode.VK_4},
		{ "5", VirtualKeyCode.VK_5 },
		{ "6", VirtualKeyCode.VK_6 },
		{ "7", VirtualKeyCode.VK_7 },
		{ "8", VirtualKeyCode.VK_8 },
		{ "9", VirtualKeyCode.VK_9 },
		{ "0", VirtualKeyCode.VK_0 },
		{ "-", VirtualKeyCode.SUBTRACT },
		{ "=", VirtualKeyCode.OEM_PLUS }
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
    private int _rightClickCounter = 0;

	private const float CameraMaxZoom = 100f;
	private const float CameraMaxPitch = 1f;

	private double _yawCounter = 0;

}

public class ApplicationContext : PropertyNotifyingClass
{
	private Settings _settings = new()
	{
		Keybindings = new List<Keybinding>
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
		}
	};
	private AutoCombatStatus _combatState = AutoCombatStatus.Inactive;
	private bool _hooked;
	
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

	/// <summary>
	/// Checks whether the hooked ROSE process is in
	/// the foreground (selected/active window).
	/// </summary>
	/// <returns>true if the ROSE window is selected</returns>
	public bool IsHookedProcessInForeground()
	{
		var foreground = RoseProcess.getForegroundProcess();
		return foreground != null && foreground.Id == HookedProcessId;
	}
}