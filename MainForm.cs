// TODO: shift key support + auto buff + zhack

/* 
 * tested working example for shift combo key
 * _sim.Keyboard.KeyDown(VirtualKeyCode.SHIFT); // hold shift
 * _sim.Keyboard.KeyPress(VirtualKeyCode.VK_1); // press hotkey
 * _sim.Keyboard.KeyUp(VirtualKeyCode.SHIFT); // let go of shift
*/

using System;

namespace ElfBot
{
	using Random = System.Random;
	using Form = System.Windows.Forms.Form;
	using PercentageDict = System.Collections.Generic.Dictionary<string, float>;
	using KeyDict = System.Collections.Generic.Dictionary<string, WindowsInput.Native.VirtualKeyCode>;
	using MonsterHashTable = System.Collections.Generic.HashSet<string>;
	using KeyList = System.Collections.Generic.List<WindowsInput.Native.VirtualKeyCode>;
	using InputSimulator = WindowsInput.InputSimulator;
	using VirtualKeyCode = WindowsInput.Native.VirtualKeyCode;
	using Size = System.Drawing.Size;

	public sealed partial class MainForm : Form
	{
		private FinishedHooking OnFinishedHooking;
		
		private PercentageDict _percentages = new PercentageDict()
		{
			{ "10%" , 0.1f },
			{ "20%" , 0.2f },
			{ "30%" , 0.3f },
			{ "40%" , 0.4f },
			{ "50%" , 0.5f },
			{ "60%" , 0.6f },
			{ "70%" , 0.7f },
			{ "80%" , 0.8f },
			{ "90%" , 0.9f },
		};

		private KeyDict _keyMap = new KeyDict()
		{
			{ "0" , VirtualKeyCode.VK_0 },
			{ "1" , VirtualKeyCode.VK_1 },
			{ "2" , VirtualKeyCode.VK_2 },
			{ "3" , VirtualKeyCode.VK_3 },
			{ "4" , VirtualKeyCode.VK_4 },
			{ "5" , VirtualKeyCode.VK_5 },
			{ "6" , VirtualKeyCode.VK_6 },
			{ "7" , VirtualKeyCode.VK_7 },
			{ "8" , VirtualKeyCode.VK_8 },
			{ "9" , VirtualKeyCode.VK_9 },
		};

		private KeyList _activeCombatKeys = new KeyList();
		private KeyList _activeHPKeys = new KeyList();
		private KeyList _activeMPKeys = new KeyList();
		private MonsterHashTable _monsterTable;

		private CombatStates _combatState;
		private InputSimulator _sim;
		private Random _ran = new Random();

		private bool _dualClient = true;
		private bool _pressedTargetting = false;
		private bool _eatHPFood = true;
		private bool _eatMPFood = true;
		private bool _combatCamera = false;
		private bool _forceCameraTopDown = false;
		private bool _timedCameraYaw = false;

		private int _currentPanelsPage = 0;
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
		private int _panelButtonHeight = 26;
		private int _rightClickCounter = 0;

		private float _lootForSeconds = 4f;
		private float _actionDelay = 0.5f;
		private float _combatKeyDelay = 1f;
		private float _foodDelay = 1f;
		private float _hpKeyDelay = 10f;
		private float _mpKeyDelay = 10f;
		private float _retargetTimeout = 15f;
		private float _lastXPos = 0f;
		private float _lastYPos = 0f;

		private double _yawCounter = 0;

		private string _currentTarget = "";
		private string _targetDefeatedMsg = "";
		private const float CameraMaxZoom = 100f;
		private const float CameraMaxPitch = 1f;

		private Size _size_monsterPanel = new Size(151, 443);
		private Size _size_combatPanel = new Size(306, 443);
		private Size _size_lootPanel = new Size(103, 443);
		private Size _size_foodPanel = new Size(306, 443);
	}
}
