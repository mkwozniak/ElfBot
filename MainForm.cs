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
	using PercentageDict = System.Collections.Generic.Dictionary<string, float>;
	using OptionDict = System.Collections.Generic.Dictionary<string, bool>;
	using KeyDict = System.Collections.Generic.Dictionary<string, WindowsInput.Native.VirtualKeyCode>;
	using MonsterHashTable = System.Collections.Generic.HashSet<string>;
	using MonsterList = System.Collections.Generic.List<string>;
	using KeyList = System.Collections.Generic.List<WindowsInput.Native.VirtualKeyCode>;
	using MsgList = System.Collections.Generic.List<string>;

	public sealed partial class MainForm : Form
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
			{ "PlayerXPos" , "trose.exe+010BE100,0x258,0x370,0xA0,0x380,0x1B8"},
			{ "PlayerYPos" , "trose.exe+010BE100,0x258,0x370,0xA0,0x380,0x1BC"},
			{ "PlayerZPos", "trose.exe+010BE100,0x258,0x370,0xA0,0x380,0x1C0" },
			{ "PlayerHP" , "trose.exe+10BE100,0x3acc"},
			{ "PlayerMaxHP" , "trose.exe+10BE100,0x4600"},
			{ "PlayerMP" , "trose.exe+10BE100,0x3AD0"},
			{ "PlayerMaxMP" , "trose.exe+10BE100,0x4604"},
			{ "TargetDefeatedMsg" , "trose.exe+10C5950"},
		};

		public static PercentageDict Percentages = new PercentageDict()
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

		public static KeyDict KeyMap = new KeyDict()
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

		public static VirtualKeyCode ShiftKey = VirtualKeyCode.SHIFT;
		public static KeyList ActiveCombatKeys = new KeyList();
		public static KeyList ActiveHPKeys = new KeyList();
		public static KeyList ActiveMPKeys = new KeyList();
		public static MonsterHashTable MonsterTable;

		private CombatStates _combatState;
		private Mem _mem;
		private InputSimulator _sim;
		private System.Random _ran = new System.Random();

		private int _currentTargetUID = 0;
		private int _currentXP = 0;
		private int _playerMaxMP = 0;
		private int _playerMP = 0;
		private int _playerMaxHP = 0;
		private int _playerHP = 0;
		private int _xpBeforeKill = -1;
		private int _interfaceUpdateTime = 60;
		private float _lootForSeconds = 4f;
		private float _actionDelay = 0.5f;
		private float _combatKeyDelay = 1f;
		private float _foodDelay = 1f;
		private float _hpKeyDelay = 10f;
		private float _mpKeyDelay = 10f;
		private float _retargetTimeout = 15f;
		private string _currentTarget = "";
		private string _targetDefeatedMsg = "";
		private bool _hooked = false;
		private bool _pressedTargetting = false;
		private bool _eatHPFood = true;
		private bool _eatMPFood = true;
		private float _lastXPos = 0f;
		private float _lastYPos = 0f;

		#endregion

		#region Init Methods

		private void Form1_Load(object sender, EventArgs e)
		{
			_mem = new Mem();
			_sim = new InputSimulator();

			ListenToTimer(combatTimer, attacking_Tick);
			ListenToTimer(targettingTimer, targetting_Tick);
			ListenToTimer(checkTimer, checkingTarget_Tick);
			ListenToTimer(lootingTimer, loot_Tick);
			ListenToTimer(lootingEndTimer, lootEnd_Tick);
			ListenToTimer(interfaceTimer, interface_Tick);
			ListenToTimer(attackTimeoutTimer, retargetTimeout_Tick);
			ListenToTimer(hpFoodTimer, hpFoodTimer_Tick);
			ListenToTimer(mpFoodTimer, mpFoodTimer_Tick);
			ListenToTimer(hpFoodKeyTimer, hpFoodKeyTimer_Tick);
			ListenToTimer(mpFoodKeyTimer, mpFoodKeyTimer_Tick);

			StartTimer(interfaceTimer, _interfaceUpdateTime);

			MonsterTable = new MonsterHashTable();
			AutoCombatBox.Enabled = false;
			SystemMsgLog.Clear();

			CombatOptionsPanel.Visible = CombatOptionsPanel.Visible ? false : true;
			CombatOptionsPanel.Size = CombatOptionsPanel.Size.Height == 0 ? new System.Drawing.Size(583, 443) : new System.Drawing.Size(583, 0);
			CombatOptionsButton.ForeColor = CombatOptionsPanel.Visible ? System.Drawing.Color.Green : System.Drawing.Color.Black;

			// worker threads could be useful later
			// if (!worker.IsBusy)  {  //worker.RunWorkerAsync();  }
		}

		private bool TryOpenProcess()
		{
			int pID = _mem.GetProcIdFromName("trose");

			if (pID > 0)
			{
				_mem.OpenProcess(pID);
				ProcessHookLabel.Text = "Process Hooked!";
				ProcessHookLabel.ForeColor = System.Drawing.Color.LimeGreen;
				AutoCombatBox.Enabled = true;
				_hooked = true;
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

		private void StopTimer(Timer timer)
		{
			timer.Stop();
			timer.Enabled = false;
		}

		private void StopAllTimers()
		{
			StopTimer(combatTimer);
			StopTimer(targettingTimer);
			StopTimer(checkTimer);
			StopTimer(lootingEndTimer);
			StopTimer(lootingTimer);
			StopTimer(attackTimeoutTimer);
			StopTimer(hpFoodTimer);
			StopTimer(mpFoodTimer);
			StopTimer(hpFoodKeyTimer);
			StopTimer(mpFoodKeyTimer);
		}

		#endregion

		#region Methods

		private void LogDateMsg(string msg)
		{
			Console.WriteLine(System.DateTime.Now.ToString() + ": " + msg);
			LogMsgToFormLog(msg);
		}

		private void LogMsgToFormLog(string msg)
		{
			if (SystemMsgLog.Text.Length > 2048)
			{
				SystemMsgLog.Clear();
			}

			string dateFormat = DateTime.Now.ToString("hh:mm:ss tt");

			string str = dateFormat + ":" + Environment.NewLine + msg;
			SystemMsgLog.AppendText(str);
			SystemMsgLog.AppendText(Environment.NewLine);
			SystemMsgLog.AppendText(Environment.NewLine);
		}
	

		private void RebuildMonsterList()
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

		private void LoadToMonsterTable(string[] monsters)
		{
			MonsterTable.Clear();
			for (int i = 0; i < monsters.Length; i++)
			{
				if (monsters[i].Length > 0)
				{
					LogDateMsg("Added monster to table from file: " + monsters[i]);
					MonsterTable.Add(monsters[i]);
				}
			}
			RebuildMonsterList();
		}

		private void CheckTargetKilled()
		{
			// if current xp is greater than our xp while targetting
			if (_currentXP > _xpBeforeKill)
			{
				_targetDefeatedMsg = _mem.ReadString(Addresses["TargetDefeatedMsg"]);
				LogDateMsg("Target Defeat: " + _targetDefeatedMsg);
				StopTimer(attackTimeoutTimer);

				if (combatLootCheckbox.Checked)
				{
					// enemy has died, loot now and start the loot timer
					_combatState = CombatStates.Looting;
					StopTimer(combatTimer);
					StartTimer(lootingTimer, (int)(_actionDelay * 1000)); // start the looting timer for hotkey
					StartTimer(lootingEndTimer, (int)(_lootForSeconds * 1000)); // start the timer to end that 
				}
				else
				{
					StopTimer(combatTimer);
					SwitchToTargetting(true);
				}

				_pressedTargetting = false;
			}
		}

		private void SwitchToTargetting(bool resetUID = false)
		{
			if(resetUID)
			{
				_currentTargetUID = -1;
			}

			_currentTarget = "";
			_pressedTargetting = false;
			_combatState = CombatStates.Targetting;
			StartTimer(targettingTimer, (int)(_actionDelay * 1000));
		}

		private void HideAllPanels()
		{
			CombatOptionsPanel.Visible = false;
			LootOptionsPanel.Visible = false;
			FoodOptionsPanel.Visible = false;
			CombatOptionsButton.ForeColor = System.Drawing.Color.Black;
			LootOptionsBtn.ForeColor = System.Drawing.Color.Black;
			FoodOptionsBtn.ForeColor = System.Drawing.Color.Black;
		}

		private void TryFloatFromInputBox(MaskedTextBox box, ref float write)
		{
			float result = 0f;

			if (!float.TryParse(box.Text, out result))
			{
				return;
			}

			write = result;
		}

		#endregion

		#region Form Events

		private void AutoCombatBox_CheckedChanged(object sender, EventArgs e)
		{
			if (!_hooked)
				return;

			if (!AutoCombatBox.Checked)
			{
				StopAllTimers();
				AutoCombatState.Text = "INACTIVE";
				LogDateMsg("Disabled AutoCombat");
				// SwitchToTargetting();
				return;
			}

			if (MonsterTable.Count == 0)
			{
				LogDateMsg("Error: Empty Monstertable");
				AutoCombatBox.Checked = false;
				return;
			}

			AutoCombatState.Text = "STARTING";
			LogDateMsg("Enabled AutoCombat");
			_xpBeforeKill = -1;
			SwitchToTargetting();
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
			RebuildMonsterList();
		}

		private void monsterRemoveBtn_Click(object sender, EventArgs e)
		{
			string inputTxt = monsterInputBox.Text;
			monsterInputBox.Text = "";
			if (MonsterTable.Contains(inputTxt))
			{
				MonsterTable.Remove(inputTxt);
				RebuildMonsterList();
			}
		}

		private void hookButton_Click(object sender, EventArgs e)
		{
			if (!TryOpenProcess()) { return; }  // TODO: add error to fail process open
		}

		private void lootTimeInputBox_InputChanged(object sender, EventArgs e)
		{
			TryFloatFromInputBox(lootTimeInputBox, ref _lootForSeconds);
			/*
			float result = 0f;

			if(float.TryParse(lootTimeInputBox.Text, out result))
			{
				_lootForSeconds = result;
			}
			*/
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

		private void foodDelayInputBox_InputChanged(object sender, EventArgs e)
		{
			float result = 0f;

			if (float.TryParse(foodDelayInputBox.Text, out result))
			{
				_foodDelay = result;
			}
		}

		private void eatKeyDelayInputBox_InputChanged(object sender, EventArgs e)
		{
			float result = 0f;

			if (float.TryParse(eatKeyDelayInputBox.Text, out result))
			{
				_hpKeyDelay = result;
				_mpKeyDelay = result;
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
					if (!ActiveCombatKeys.Contains(KeyMap[key]))
					{
						LogDateMsg("Added Active Key: " + KeyMap[key].ToString());
						ActiveCombatKeys.Add(KeyMap[key]);
					}
					continue;
				}

				// LogDateMsg("Removed Active Key: " + CombatKeys[key].ToString());
				ActiveCombatKeys.Remove(KeyMap[key]);
			}
		}

		private void updateFoodKeysBtn_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < hpKeys.Items.Count; i++)
			{
				bool isChecked = hpKeys.GetItemChecked(i);
				string key = hpKeys.GetItemText(hpKeys.Items[i]);
				if (isChecked)
				{
					if (!ActiveHPKeys.Contains(KeyMap[key]))
					{
						LogDateMsg("Added Active HP Key: " + KeyMap[key].ToString());
						ActiveHPKeys.Add(KeyMap[key]);
					}
					continue;
				}
				ActiveHPKeys.Remove(KeyMap[key]);
			}

			for (int i = 0; i < mpKeys.Items.Count; i++)
			{
				bool isChecked = mpKeys.GetItemChecked(i);
				string key = mpKeys.GetItemText(mpKeys.Items[i]);
				if (isChecked)
				{
					if (!ActiveMPKeys.Contains(KeyMap[key]))
					{
						LogDateMsg("Added Active MP Key: " + KeyMap[key].ToString());
						ActiveMPKeys.Add(KeyMap[key]);
					}
					continue;
				}
				ActiveMPKeys.Remove(KeyMap[key]);
			}
		}

		private void loadTableButton_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				try
				{
					var filePath = openFileDialog1.FileName;
					var sr = new System.IO.StreamReader(openFileDialog1.FileName);
					string contents = sr.ReadToEnd();
					string[] monsters = contents.Split(',');
					if (monsters.Length > 0)
					{
						LoadToMonsterTable(monsters);
					}
				}
				catch (System.Security.SecurityException ex)
				{
					MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
					$"Details:\n\n{ex.StackTrace}");
				}
			}
		}

		private void CombatOptionsButton_Click(object sender, EventArgs e)
		{
			HideAllPanels();
			CombatOptionsPanel.Visible = CombatOptionsPanel.Visible ? false : true;
			CombatOptionsPanel.Size = CombatOptionsPanel.Visible ? new System.Drawing.Size(306, 443) : new System.Drawing.Size(306, 0);
			CombatOptionsButton.ForeColor = CombatOptionsPanel.Visible ? System.Drawing.Color.Green : System.Drawing.Color.Black;
		}

		private void LootOptionsBtn_Click(object sender, EventArgs e)
		{
			HideAllPanels();
			LootOptionsPanel.Visible = LootOptionsPanel.Visible ? false : true;
			LootOptionsPanel.Size = LootOptionsPanel.Visible ? new System.Drawing.Size(103, 443) : new System.Drawing.Size(103, 0);
			LootOptionsBtn.ForeColor = LootOptionsPanel.Visible ? System.Drawing.Color.Green : System.Drawing.Color.Black;
		}

		private void FoodOptionsBtn_Click(object sender, EventArgs e)
		{
			HideAllPanels();
			FoodOptionsPanel.Visible = FoodOptionsPanel.Visible ? false : true;
			FoodOptionsPanel.Size = FoodOptionsPanel.Visible ? new System.Drawing.Size(306, 443) : new System.Drawing.Size(309, 0);
			FoodOptionsBtn.ForeColor = FoodOptionsPanel.Visible ? System.Drawing.Color.Green : System.Drawing.Color.Black;
		}

		private void hpFoodCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (!_hooked)
				return;

			if (hpFoodCheckbox.Checked)
			{
				LogDateMsg("Enabled Auto Food HP");
				StartTimer(hpFoodTimer, (int)(_foodDelay * 1000));
				return;
			}

			LogDateMsg("Disabled Auto Food HP");
			StopTimer(hpFoodTimer);
		}

		private void mpFoodCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (!_hooked)
				return;

			if (mpFoodCheckbox.Checked)
			{
				LogDateMsg("Enabled Auto Food MP");
				StartTimer(mpFoodTimer, (int)(_foodDelay * 1000));
				return;
			}

			LogDateMsg("Disabled Auto Food MP");
			StopTimer(mpFoodTimer);
		}

		#endregion

		private void button2_Click(object sender, EventArgs e)
		{
			//_mem.WriteMemory(Addresses["PlayerZPos"], "float", (_mem.ReadFloat(Addresses["PlayerZPos"]) + 2).ToString());
			Console.WriteLine(_mem.ReadFloat(Addresses["PlayerZPos"]).ToString());
			_mem.WriteMemory(Addresses["PlayerZPos"], "float", "5");
		}
	}
}
