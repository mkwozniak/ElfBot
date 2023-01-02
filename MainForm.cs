namespace ElfBot
{
	using System;
	using System.Linq;
	using System.ComponentModel;
	using System.Windows.Forms;
	using Memory;
	using WindowsInput;
	using VirtualKeyCode = WindowsInput.Native.VirtualKeyCode;

	using AddressDict = System.Collections.Generic.Dictionary<string, string>;
	using OptionDict = System.Collections.Generic.Dictionary<string, bool>;
	using KeyDict = System.Collections.Generic.Dictionary<string, WindowsInput.Native.VirtualKeyCode>;
	using MonsterHashTable = System.Collections.Generic.HashSet<string>;
	using MonsterList = System.Collections.Generic.List<string>;
	using KeyList = System.Collections.Generic.List<WindowsInput.Native.VirtualKeyCode>;

	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		#region Members

		public static AddressDict Addresses = new AddressDict()
		{
			{ "CurrentTarget" , "trose.exe+10D8C10" },
			{ "CurrentXP" , "trose.exe+10BCA14" },
			{ "TargetUID" , "trose.exe+10C0458,0x8"},
		};

		public static OptionDict BotOptions = new OptionDict()
		{
			{ "AutoCombat" , false },
			{ "AutoCombatLoot" , false },
		};

		public static VirtualKeyCode ShiftKey = VirtualKeyCode.SHIFT;

		public static KeyDict CombatKeys = new KeyDict()
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

		public static KeyList ActiveCombatKeys = new KeyList();

		public static MonsterHashTable MonsterTable;

		private CombatStates _combatState;
		private Mem m;
		private InputSimulator sim;
		private System.Random _ran = new System.Random();

		private int _currentTargetUID = -1;
		private int _currentXP = 0;
		private int _xpBeforeKill = -1;
		private float _lootForSeconds = 4f;
		private float _actionDelay = 1f;
		private float _combatKeyDelay = 1f;
		private float _retargetTimeout = 15f;
		private string _currentTarget = "";
		private bool _pressedTargetting = false;
		private bool _pressedCombatKey = false;
		private bool _attacking = false;
		private int _lastTargetKilled = 0;

		#endregion

		#region Init Methods

		private void Form1_Load(object sender, EventArgs e)
		{
			m = new Mem();
			sim = new InputSimulator();

			ListenToTimer(combatTimer, attacking_Tick);
			ListenToTimer(targettingTimer, targetting_Tick);
			ListenToTimer(checkTimer, checkingTarget_Tick);
			ListenToTimer(lootingTimer, loot_Tick);
			ListenToTimer(lootTimer, lootEnd_Tick);
			ListenToTimer(interfaceTimer, interface_Tick);
			ListenToTimer(attackTimeoutTimer, retargetTimeout_Tick);

			MonsterTable = new MonsterHashTable();
			_combatState = CombatStates.Targetting;
			AutoCombatBox.Enabled = false;

			// worker threads could be useful later
			// if (!worker.IsBusy)  {  //worker.RunWorkerAsync();  }
		}

		private bool TryOpenProcess()
		{
			int pID = m.GetProcIdFromName("trose");

			if (pID > 0)
			{
				m.OpenProcess(pID);
				ProcessHookLabel.Text = "Process Hooked!";
				ProcessHookLabel.ForeColor = System.Drawing.Color.LimeGreen;

				AutoCombatBox.Enabled = true;

				return true;
			}

			ProcessHookLabel.Text = "Process Hook Failed :(";
			ProcessHookLabel.ForeColor = System.Drawing.Color.Red;
			return false;
		}

		private void ListenToTimer(Timer timer, EventHandler del)
		{
			timer.Tick += new EventHandler(del);
		}

		private void StartTimer(Timer timer, int msDelay)
		{
			timer.Interval = msDelay;
			timer.Enabled = true;
			timer.Start();
		}

		private void StopTimer(Timer timer, EventHandler del)
		{
			timer.Tick -= del;
			timer.Stop();
			timer.Enabled = false;
		}

		private void StopTimer(Timer timer)
		{
			timer.Stop();
			timer.Enabled = false;
		}

		#endregion

		#region Methods

		private void LogDateMsg(string msg)
		{
			Console.WriteLine(System.DateTime.Now.ToString() + ": " + msg);
		}

		private void RebuildMonsterTable()
		{
			MonsterList monsterList = new MonsterList(MonsterTable);
			monsterTableText.Text = "";

			for (int i = 0; i < monsterList.Count; i++)
			{
				monsterTableText.Text += monsterList[i] + "\n";
			}

			if (monsterTableText.Text.Length == 0)
			{
				monsterTableText.Text = "Empty";
			}
		}

		private void CheckTargetKilled()
		{
			// if current xp is greater than our xp while targetting
			if (_currentXP > _xpBeforeKill)
			{
				if (combatLootCheckbox.Checked)
				{
					// enemy has died, loot now and start the loot timer
					_combatState = CombatStates.Looting;
					_attacking = false;
					StartTimer(lootTimer, (int)(_lootForSeconds * 1000));
				}
				else
				{
					_currentTarget = "";
					_pressedTargetting = false;
					_attacking = false;
					_combatState = CombatStates.Targetting;
				}

				_pressedTargetting = false;
				_lastTargetKilled = _currentTargetUID;
			}
		}

		#endregion

		#region Form Events

		private void AutoCombatBox_CheckedChanged(object sender, EventArgs e)
		{
			if (!AutoCombatBox.Checked && mainTimer.Enabled)
			{
				mainTimer.Tick -= mainTimer_Tick;
				mainTimer.Stop();
				mainTimer.Enabled = false;
				StopTimer(attackTimeoutTimer);
				AutoCombatLabel.Text = "DISABLED";
				Console.WriteLine("Disabled AutoCombat");
				_currentTarget = "None";
				_pressedTargetting = false;
				_combatState = CombatStates.Targetting;
				return;
			}

			if (MonsterTable.Count == 0)
			{
				ErrorLabel.Text = "Error: Empty Monstertable.";
				AutoCombatBox.Checked = false;
				return;
			}

			AutoCombatLabel.Text = "ENABLED";
			Console.WriteLine("Enabled AutoCombat");
			mainTimer.Interval = 1000;
			mainTimer.Tick += mainTimer_Tick;
			mainTimer.Enabled = true;
			mainTimer.Start();
			_xpBeforeKill = -1;
			_combatState = CombatStates.Targetting;
		}

		private void monsterAddBtn_Click(object sender, EventArgs e)
		{
			string inputTxt = monsterInputBox.Text;
			if (MonsterTable.Contains(inputTxt))
			{
				monsterInputBox.Text = "";
				return;
			}

			if(MonsterTable.Count == 0)
			{
				monsterTableText.Text = "";
			}

			MonsterTable.Add(inputTxt);
			monsterInputBox.Text = "";
			RebuildMonsterTable();
		}

		private void monsterRemoveBtn_Click(object sender, EventArgs e)
		{
			string inputTxt = monsterInputBox.Text;
			monsterInputBox.Text = "";
			if (MonsterTable.Contains(inputTxt))
			{
				MonsterTable.Remove(inputTxt);
				RebuildMonsterTable();
			}
		}

		private void hookButton_Click(object sender, EventArgs e)
		{
			if (!TryOpenProcess()) { return; }  // TODO: add error to fail process open
		}

		private void lootTimeInputBox_InputChanged(object sender, EventArgs e)
		{
			float result = 0f;

			if(float.TryParse(lootTimeInputBox.Text, out result))
			{
				_lootForSeconds = result;
			}
		}

		private void actionDelayInputBox_InputChanged(object sender, EventArgs e)
		{
			float result = 0f;

			if (float.TryParse(actionDelayInputBox.Text, out result))
			{
				_actionDelay = result;
			}
		}

		private void retargetTimeoutInputBox_InputChanged(object sender, EventArgs e)
		{
			float result = 0f;

			if (float.TryParse(retargetTimeoutInputBox.Text, out result))
			{
				_retargetTimeout = result;
			}
		}


		private void combatKeyDelayInputBox_InputChanged(object sender, EventArgs e)
		{
			float result = 0f;

			if (float.TryParse(combatKeyDelayInputBox.Text, out result))
			{
				_combatKeyDelay = result;
			}
		}


		private void updateCombatKeysBtn_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < combatKeys.Items.Count; i++)
			{
				bool isChecked = combatKeys.GetItemChecked(i);
				string key = combatKeys.GetItemText(combatKeys.Items[i]);
				if (isChecked)
				{
					if (!ActiveCombatKeys.Contains(CombatKeys[key]))
					{
						Console.WriteLine("Added Active Key: " + CombatKeys[key].ToString());
						ActiveCombatKeys.Add(CombatKeys[key]);
					}
					continue;
				}

				Console.WriteLine("Removed Active Key: " + CombatKeys[key].ToString());
				ActiveCombatKeys.Remove(CombatKeys[key]);
			}
		}

		#endregion
	}
}
