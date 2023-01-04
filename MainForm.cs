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
	using System.Diagnostics;

	public sealed partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		#region Members

		private AddressDict Addresses = new AddressDict()
		{
			{ "PlayerName" , "trose.exe+10C1918" },
			{ "PlayerLevel" , "trose.exe+10BE100,0x3AD8" },
			{ "Zuly" , "trose.exe+10BE100,0x3D38" },
			{ "MapID" , "trose.exe+10C4AE4" },
			{ "CurrentTarget" , "trose.exe+10D8C10" },
			{ "CurrentXP" , "trose.exe+10BE100,0x3AD4" },
			{ "TargetUID" , "trose.exe+10C0458,0x8"},
			{ "PlayerXPos" , "trose.exe+010BE100,0x258,0x370,0xA0,0x380,0x1B8"},
			{ "PlayerYPos" , "trose.exe+010BE100,0x258,0x370,0xA0,0x380,0x1BC"},
			{ "PlayerZPos", "trose.exe+010BE100,0x258,0x370,0xA0,0x380,0x1C0" },
			{ "PlayerHP" , "trose.exe+10BE100,0x3acc"},
			{ "PlayerMaxHP" , "trose.exe+10BE100,0x4600"},
			{ "PlayerMP" , "trose.exe+10BE100,0x3AD0"},
			{ "PlayerMaxMP" , "trose.exe+10BE100,0x4604"},
			{ "TargetDefeatedMsg" , "trose.exe+10C5950"},
			{ "CameraZoom", "trose.exe+010D2520,0xD70,0x6C4" },
			{ "CameraPitch", "trose.exe+010D2520,0xD70,0x6C0" },
			{ "CameraYaw", "trose.exe+010D2520,0xD70,0x6BC" },
		};

		private PercentageDict Percentages = new PercentageDict()
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

		private KeyDict KeyMap = new KeyDict()
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

		private VirtualKeyCode ShiftKey = VirtualKeyCode.SHIFT;
		private KeyList ActiveCombatKeys = new KeyList();
		private KeyList ActiveHPKeys = new KeyList();
		private KeyList ActiveMPKeys = new KeyList();
		private MonsterHashTable MonsterTable;

		private CombatStates _combatState;
		private Mem _mem;
		private InputSimulator _sim;
		private System.Random _ran = new System.Random();

		private bool _hooked = false;
		private bool _dualClient = true;
		private bool _pressedTargetting = false;
		private bool _eatHPFood = true;
		private bool _eatMPFood = true;
		private bool _forceCameraMaxZoom = false;
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
		private int _cameraTickTime = 500;
		private int _panelButtonHeight = 26;

		private float _lootForSeconds = 4f;
		private float _actionDelay = 0.5f;
		private float _combatKeyDelay = 1f;
		private float _foodDelay = 1f;
		private float _hpKeyDelay = 10f;
		private float _mpKeyDelay = 10f;
		private float _retargetTimeout = 15f;
		private float _timedCameraYawDelay = 8f;
		private float _lastXPos = 0f;
		private float _lastYPos = 0f;

		private string _currentTarget = "";
		private string _targetDefeatedMsg = "";
		private string _cameraMaxZoom = "100";
		private string _cameraMaxPitch = "1";
		private string[] _cameraYawRotations = new string[] { "1.5", "-1.5", "0", "3" };

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
			ListenToTimer(cameraTimer, cameraTimer_Tick);
			ListenToTimer(CameraYawTimer, CameraYawTimer_Tick);

			StartTimer(interfaceTimer, _interfaceUpdateTime);
			StartTimer(cameraTimer, _cameraTickTime);

			MonsterTable = new MonsterHashTable();
			AutoCombatBox.Enabled = false;
			SystemMsgLog.Clear();

			CombatOptionsPanel.Visible = true;
			CombatOptionsPanel.Size = new System.Drawing.Size(306, 443);
			CombatOptionsBtn.ForeColor = System.Drawing.Color.Green;

			// worker threads could be useful later
			// if (!worker.IsBusy)  {  //worker.RunWorkerAsync();  }
		}

		/// <summary>
		/// Get the process ID number by process name.
		/// </summary>
		/// <param name="name">Example: "eqgame". Use task manager to find the name. Do not include .exe</param>
		/// <returns></returns>
		public int GetProcIdFromName(string name) //new 1.0.2 function
		{
			Process[] processlist = Process.GetProcesses();

			if (name.ToLower().Contains(".exe"))
				name = name.Replace(".exe", "");
			if (name.ToLower().Contains(".bin")) // test
				name = name.Replace(".bin", "");

			bool foundClient = false;
			int mainID = 0;

			foreach (System.Diagnostics.Process theprocess in processlist)
			{
				//find (name).exe in the process list (use task manager to find the name)
				if (theprocess.ProcessName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
				{
					if(!foundClient)
					{
						foundClient = true;
						mainID = theprocess.Id;
						continue;
					}

					if(_dualClient && foundClient)
					{
						mainID = theprocess.Id;
						LogDateMsg("Hooking to second ROSE client.");
						return mainID;
					}
				}					
			}

			if(foundClient)
			{
				return mainID;
			}

			return mainID; //if we fail to find it
		}

		private bool TryOpenProcess()
		{
			int pID = GetProcIdFromName("trose");

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

		private void CheckShouldAttackTarget()
		{
			if (_currentTargetUID != 0 && ActiveCombatKeys.Count > 0 && _targetDefeatedMsg.Length == 0)
			{			
				int ranSkill = _ran.Next(0, ActiveCombatKeys.Count);
				_sim.Keyboard.KeyPress(ActiveCombatKeys[ranSkill]); // attack press

				LogDateMsg("Attack Tick: " + ActiveCombatKeys[ranSkill].ToString());

				if (!attackTimeoutTimer.Enabled)
				{
					StartTimer(attackTimeoutTimer, (int)(_retargetTimeout * 1000));
				}
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
			CameraOptionsPanel.Visible = false;
			CombatOptionsBtn.ForeColor = System.Drawing.Color.Black;
			LootOptionsBtn.ForeColor = System.Drawing.Color.Black;
			FoodOptionsBtn.ForeColor = System.Drawing.Color.Black;
			CameraOptionsBtn.ForeColor = System.Drawing.Color.Black;
		}

		private void TryFloatFromInputBox(MaskedTextBox box, ref float write)
		{
			float result = 0f;

			if (!float.TryParse(box.Text, out result))
			{
				box.Text = write.ToString();
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
		}

		private void actionDelayInputBox_InputChanged(object sender, EventArgs e)
		{
			TryFloatFromInputBox(actionDelayInputBox, ref _actionDelay);
		}

		private void retargetTimeoutInputBox_InputChanged(object sender, EventArgs e)
		{
			TryFloatFromInputBox(retargetTimeoutInputBox, ref _retargetTimeout);
		}

		private void combatKeyDelayInputBox_InputChanged(object sender, EventArgs e)
		{
			TryFloatFromInputBox(combatKeyDelayInputBox, ref _combatKeyDelay);
		}

		private void foodDelayInputBox_InputChanged(object sender, EventArgs e)
		{
			TryFloatFromInputBox(foodDelayInputBox, ref _foodDelay);
		}

		private void eatKeyDelayInputBox_InputChanged(object sender, EventArgs e)
		{
			TryFloatFromInputBox(eatKeyDelayInputBox, ref _hpKeyDelay);
			TryFloatFromInputBox(eatKeyDelayInputBox, ref _mpKeyDelay);
		}
		private void CameraYawDelayInputbox_InputChanged(object sender, EventArgs e)
		{
			float result = 0f;

			if (float.TryParse(CameraYawDelayInputbox.Text, out result))
			{
				StopTimer(CameraYawTimer);
				_timedCameraYawDelay = result;
				StartTimer(CameraYawTimer, (int)(_timedCameraYawDelay * 1000));
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
			CombatOptionsBtn.ForeColor = CombatOptionsPanel.Visible ? System.Drawing.Color.Green : System.Drawing.Color.Black;
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

		private void ForceMaxZoomCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (ForceMaxZoomCheckbox.Checked)
			{
				LogDateMsg("Enabled Force Max Camera Zoom");
				_forceCameraMaxZoom = true;
				return;
			}

			LogDateMsg("Disabled Force Max Camera Zoom");
			_forceCameraMaxZoom = false;
		}

		private void ForceTopdownCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (ForceTopdownCheckbox.Checked)
			{
				LogDateMsg("Enabled Force Camera Topdown");
				_forceCameraTopDown = true;
				return;
			}

			//StopTimer(cameraTimer)
			LogDateMsg("Disabled Force Camera Topdown");
			_forceCameraTopDown = false;
		}

		private void MorePanelsBtn_Click(object sender, EventArgs e)
		{
			if(_currentPanelsPage == 0)
			{
				HideAllPanels();

				System.Drawing.Size btnSize = CombatOptionsBtn.Size;
				btnSize.Height = 0;

				System.Drawing.Size newBtnSize = CombatOptionsBtn.Size;
				newBtnSize.Height = _panelButtonHeight;

				CombatOptionsBtn.Size = btnSize;
				LootOptionsBtn.Size = btnSize;
				FoodOptionsBtn.Size = btnSize;
				CameraOptionsBtn.Size = newBtnSize;

				CameraOptionsPanel.Visible = true;
				CameraOptionsPanel.Size = new System.Drawing.Size(103, 443);
				CameraOptionsBtn.ForeColor = System.Drawing.Color.Green;

				_currentPanelsPage = 1;
				return;
			}

			if (_currentPanelsPage == 1)
			{
				HideAllPanels();

				System.Drawing.Size btnSize = CombatOptionsBtn.Size;
				btnSize.Height = 0;

				System.Drawing.Size newBtnSize = CombatOptionsBtn.Size;
				newBtnSize.Height = _panelButtonHeight;

				CombatOptionsBtn.Size = newBtnSize;
				LootOptionsBtn.Size = newBtnSize;
				FoodOptionsBtn.Size = newBtnSize;
				CameraOptionsBtn.Size = btnSize;

				CombatOptionsPanel.Visible = true;
				CombatOptionsPanel.Size = new System.Drawing.Size(306, 443);
				CombatOptionsBtn.ForeColor = System.Drawing.Color.Green;

				_currentPanelsPage = 0;
				return;
			}
		}

		private void SecondClientCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if(SecondClientCheckbox.Checked)
			{
				LogDateMsg("Enabled 2nd Client Mode.");
				_dualClient = true;
				return;
			}

			LogDateMsg("Disabled 2nd Client Mode.");
			_dualClient = false;
		}

		private void TimedCameraYawCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			StopTimer(CameraYawTimer);

			if (TimedCameraYawCheckbox.Checked)
			{
				LogDateMsg("Enabled Timed Camera Yaw.");
				StartTimer(CameraYawTimer, (int)(_timedCameraYawDelay * 1000));
				_timedCameraYaw = true;
				return;
			}

			LogDateMsg("Disabled Timed Camera Yaw.");
			StopTimer(CameraYawTimer);
			_timedCameraYaw = false;
		}

        private void PlayerPosLabel_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void PlayerNameLabel_Click(object sender, EventArgs e)
        {

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }
    }
}
