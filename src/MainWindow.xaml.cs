namespace ElfBot
{
    using System.Windows;
    using Random = System.Random;
    using LogSet = System.Collections.Generic.HashSet<LogTypes>;
    using KeyDict = System.Collections.Generic.Dictionary<string, WindowsInput.Native.VirtualKeyCode>;
    using MonsterHashTable = System.Collections.Generic.HashSet<string>;
    using KeyList = System.Collections.Generic.List<WindowsInput.Native.VirtualKeyCode>;
    using InputSimulator = WindowsInput.InputSimulator;
    using Timer = System.Windows.Threading.DispatcherTimer;

    using VirtualKeyCode = WindowsInput.Native.VirtualKeyCode;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
        public static ApplicationExiting? OnApplicationExit;
        private FinishedHooking? OnFinishedHooking;

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

        private KeyList _activeCombatKeys = new KeyList();
        private KeyList _activeHPKeys = new KeyList();
        private KeyList _activeMPKeys = new KeyList();
        private MonsterHashTable? _monsterTable;

        private CombatStates _combatState;
        private InputSimulator? _sim;
        private Random _ran = new Random();

        private bool _dualClient = true;
        private bool _pressedTargetting = false;
        private bool _eatHPFood = true;
        private bool _eatMPFood = true;
        private bool _combatCamera = false;
        private bool _timedCameraYaw = false;

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

        private float _lootForSeconds = 3f;
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

        private double _yawCounter = 0;

        private string _currentTarget = "";
        private string _targetDefeatedMsg = "";
        private string _currentSystemLog = "";
        private const float CameraMaxZoom = 100f;
        private const float CameraMaxPitch = 1f;

        private Timer CombatTimer = new Timer();
        private Timer TargettingTimer = new Timer();
        private Timer CheckTimer = new Timer();
        private Timer LootingTimer = new Timer();
        private Timer LootingEndTimer = new Timer();
        private Timer InterfaceTimer = new Timer();
        private Timer AttackTimeoutTimer = new Timer();
        private Timer HpFoodTimer = new Timer();
        private Timer MpFoodTimer = new Timer();
        private Timer HpFoodKeyTimer = new Timer();
        private Timer MpFoodKeyTimer = new Timer();
        private Timer CombatCameraTimer = new Timer();
        private Timer CameraYawTimer = new Timer();

        private OpenFileDialog? _openMonsterTableDialog;
	}
}
