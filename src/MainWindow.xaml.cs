namespace ElfBot;

using System.Windows;
using Random = System.Random;
using MonsterHashTable = System.Collections.Generic.HashSet<string>;
using KeyList = System.Collections.Generic.List<WindowsInput.Native.VirtualKeyCode>;
using TextBoxDict = System.Collections.Generic.Dictionary<string, System.Windows.Controls.TextBox>;
using CheckBoxDict = System.Collections.Generic.Dictionary<string, System.Windows.Controls.CheckBox>;
using ComboBoxDict = System.Collections.Generic.Dictionary<string, System.Windows.Controls.ComboBox>;
using InputSimulator = WindowsInput.InputSimulator;
using Timer = System.Windows.Threading.DispatcherTimer;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

using VirtualKeyCode = WindowsInput.Native.VirtualKeyCode;
using System;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	// Events
	private FinishedHooking? OnFinishHook;

    // Timers
	private Timer CombatTimer = new();
	private Timer TargettingTimer = new();
	private Timer CheckTimer = new();
	private Timer LootingTimer = new();
	private Timer LootingEndTimer = new();
	private Timer InterfaceTimer = new();
	private Timer AttackTimeoutTimer = new();
	private Timer HpFoodTimer = new();
	private Timer MpFoodTimer = new();
	private Timer HpFoodKeyTimer = new();
	private Timer MpFoodKeyTimer = new();
	private Timer CombatCameraTimer = new();
	private Timer CameraYawTimer = new();
	private Timer ZHackTimer = new();

	// References
	private InputSimulator? _sim;
    private Random _ran = new();
    private Config? _config;
	private OpenFileDialog? _openMonsterTableDialog;

	// Structures
	private MonsterHashTable? _monsterTable;
	private KeyList _activeCombatKeys = new KeyList();
	private KeyList _activeHPKeys = new KeyList();
	private KeyList _activeMPKeys = new KeyList();
	private KeyList _activeLootKeys = new KeyList();
	private KeyList _activeBuffKeys = new KeyList();
	private TextBoxDict _textBoxes = new TextBoxDict();
	private CheckBoxDict _checkBoxes = new CheckBoxDict();
	private ComboBoxDict _comboBoxes = new ComboBoxDict();

	private KeyList _keyMap = new KeyList()
	{
		{ VirtualKeyCode.VK_1 },
		{ VirtualKeyCode.VK_2 },
		{ VirtualKeyCode.VK_3 },
		{ VirtualKeyCode.VK_4 },
		{ VirtualKeyCode.VK_5 },
		{ VirtualKeyCode.VK_6 },
		{ VirtualKeyCode.VK_7 },
		{ VirtualKeyCode.VK_8 },
		{ VirtualKeyCode.VK_9 },
		{ VirtualKeyCode.VK_0 },
	};

	// Values
	private CombatStates _combatState;

	private bool _dualClient = true;
    private bool _pressedTargetting = false;
    private bool _eatHPFood = true;
    private bool _eatMPFood = true;
    private bool _combatCamera = false;
    private bool _timedCameraYaw = false;
    private bool _combatLoot = false;
    private bool _autoHP = false;
    private bool _autoMP = false;

    private int _currentTargetUID = 0;
    private int _currentXP = 0;
    private int _playerMaxMP = 0;
    private int _playerMP = 0;
    private int _playerMaxHP = 0;
    private int _playerHP = 0;
    private int _xpBeforeKill = -1;
    private int _interfaceUpdateTime = 60;
    private int _combatCameraTickTime = 500;
	private int _cameraYawTickTime = 50;
    private int _rightClickCounter = 0;

	private const float CameraMaxZoom = 100f;
	private const float CameraMaxPitch = 1f;

	private float _combatLootSeconds = 3f;
    private float _actionDelay = 0.5f;
    private float _combatKeyDelay = 1f;
    private float _foodDelay = 1f;
    private float _hpKeyDelay = 7f;
    private float _mpKeyDelay = 7f;
    private float _retargetTimeout = 15f;
    private float _lastXPos = 0f;
    private float _lastYPos = 0f;
    private float _currentFoodHPThreshold = 50f;
    private float _currentFoodMPThreshold = 50f;
	private float _zHackDelay = 5f;
	private float _zHackDelayAmount = 7f;

    private double _yawCounter = 0;

    private string _currentTarget = "";
    private string _targetDefeatedMsg = "";
}
