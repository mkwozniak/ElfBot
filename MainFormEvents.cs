namespace ElfBot
{
	using Form = System.Windows.Forms.Form;
	using EventArgs = System.EventArgs;
	using DialogResult = System.Windows.Forms.DialogResult;
	using MessageBox = System.Windows.Forms.MessageBox;
	using Mem = Memory.Mem;
	using InputSimulator = WindowsInput.InputSimulator;


	public sealed partial class MainForm : Form
	{
		#region Form Init

		/// <summary> Form Constructor </summary>
		public MainForm()
		{
			InitializeComponent();
		}

		/// <summary> Form Load event </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_Load(object sender, EventArgs e)
		{
			_mem = new Mem();
			_sim = new InputSimulator();

			ListenToTimer(CombatTimer, Attacking_Tick);
			ListenToTimer(TargettingTimer, Targetting_Tick);
			ListenToTimer(CheckTimer, CheckingTarget_Tick);
			ListenToTimer(LootingTimer, Loot_Tick);
			ListenToTimer(LootingEndTimer, LootEnd_Tick);
			ListenToTimer(InterfaceTimer, Interface_Tick);
			ListenToTimer(AttackTimeoutTimer, RetargetTimeout_Tick);
			ListenToTimer(HpFoodTimer, HpFoodTimer_Tick);
			ListenToTimer(MpFoodTimer, MpFoodTimer_Tick);
			ListenToTimer(HpFoodKeyTimer, HpFoodKeyTimer_Tick);
			ListenToTimer(MpFoodKeyTimer, MpFoodKeyTimer_Tick);
			ListenToTimer(CameraTimer, CameraTimer_Tick);
			ListenToTimer(CameraYawTimer, CameraYawTimer_Tick);

			StartTimer(InterfaceTimer, _interfaceUpdateTime);
			StartTimer(CameraTimer, _cameraTickTime);
			StartTimer(CameraYawTimer, _cameraYawTickTime);

			PrepareElfBot();
		}

		#endregion

		#region Button Events

		/// <summary> Tries to hook process </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HookBtn_Click(object sender, EventArgs e)
		{
			if (!TryOpenProcess()) { return; }  // TODO: add error to fail process open
		}

		/// <summary> Updates combat keys </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UpdateCombatKeysBtn_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < combatKeys.Items.Count; i++)
			{
				bool isChecked = combatKeys.GetItemChecked(i);
				string key = combatKeys.GetItemText(combatKeys.Items[i]);
				if (isChecked)
				{
					if (!_activeCombatKeys.Contains(_keyMap[key]))
					{
						LogDateMsg("Added Active Key: " + _keyMap[key].ToString(), LogTypes.System);
						_activeCombatKeys.Add(_keyMap[key]);
					}
					continue;
				}
				_activeCombatKeys.Remove(_keyMap[key]);
			}
		}

		/// <summary> Updates food keys </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UpdateFoodKeysBtn_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < HpKeys.Items.Count; i++)
			{
				bool isChecked = HpKeys.GetItemChecked(i);
				string key = HpKeys.GetItemText(HpKeys.Items[i]);
				if (isChecked)
				{
					if (!_activeHPKeys.Contains(_keyMap[key]))
					{
						LogDateMsg("Added Active HP Key: " + _keyMap[key].ToString(), LogTypes.System);
						_activeHPKeys.Add(_keyMap[key]);
					}
					continue;
				}
				_activeHPKeys.Remove(_keyMap[key]);
			}

			for (int i = 0; i < MpKeys.Items.Count; i++)
			{
				bool isChecked = MpKeys.GetItemChecked(i);
				string key = MpKeys.GetItemText(MpKeys.Items[i]);
				if (isChecked)
				{
					if (!_activeMPKeys.Contains(_keyMap[key]))
					{
						LogDateMsg("Added Active MP Key: " + _keyMap[key].ToString(), LogTypes.System);
						_activeMPKeys.Add(_keyMap[key]);
					}
					continue;
				}
				_activeMPKeys.Remove(_keyMap[key]);
			}
		}

		/// <summary> Loads a monster table  </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LoadTableButton_Click(object sender, EventArgs e)
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

		/// <summary> Loads more panel buttons </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MorePanelsBtn_Click(object sender, EventArgs e)
		{
			if (_currentPanelsPage == 0)
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

		/// <summary> Opens the combat options menu </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CombatOptionsButton_Click(object sender, EventArgs e)
		{
			HideAllPanels();
			CombatOptionsPanel.Visible = CombatOptionsPanel.Visible ? false : true;
			CombatOptionsPanel.Size = CombatOptionsPanel.Visible ? new System.Drawing.Size(306, 443) : new System.Drawing.Size(306, 0);
			CombatOptionsBtn.ForeColor = CombatOptionsPanel.Visible ? System.Drawing.Color.Green : System.Drawing.Color.Black;
		}

		/// <summary> Options the loot options menu </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LootOptionsBtn_Click(object sender, EventArgs e)
		{
			HideAllPanels();
			LootOptionsPanel.Visible = LootOptionsPanel.Visible ? false : true;
			LootOptionsPanel.Size = LootOptionsPanel.Visible ? new System.Drawing.Size(103, 443) : new System.Drawing.Size(103, 0);
			LootOptionsBtn.ForeColor = LootOptionsPanel.Visible ? System.Drawing.Color.Green : System.Drawing.Color.Black;
		}

		/// <summary> Opens the food options menu </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FoodOptionsBtn_Click(object sender, EventArgs e)
		{
			HideAllPanels();
			FoodOptionsPanel.Visible = FoodOptionsPanel.Visible ? false : true;
			FoodOptionsPanel.Size = FoodOptionsPanel.Visible ? new System.Drawing.Size(306, 443) : new System.Drawing.Size(309, 0);
			FoodOptionsBtn.ForeColor = FoodOptionsPanel.Visible ? System.Drawing.Color.Green : System.Drawing.Color.Black;
		}

		#endregion

		#region InputBox Events

		// All inputbox inputchanged events

		private void LootTimeInputBox_InputChanged(object sender, EventArgs e)
		{
			TryFloatFromInputBox(lootTimeInputBox, ref _lootForSeconds);
		}

		private void ActionDelayInputBox_InputChanged(object sender, EventArgs e)
		{
			TryFloatFromInputBox(actionDelayInputBox, ref _actionDelay);
		}

		private void RetargetTimeoutInputBox_InputChanged(object sender, EventArgs e)
		{
			TryFloatFromInputBox(retargetTimeoutInputBox, ref _retargetTimeout);
		}

		private void CombatKeyDelayInputBox_InputChanged(object sender, EventArgs e)
		{
			TryFloatFromInputBox(combatKeyDelayInputBox, ref _combatKeyDelay);
		}

		private void FoodDelayInputBox_InputChanged(object sender, EventArgs e)
		{
			TryFloatFromInputBox(FoodDelayInputBox, ref _foodDelay);
		}

		private void EatKeyDelayInputBox_InputChanged(object sender, EventArgs e)
		{
			TryFloatFromInputBox(EatKeyDelayInputBox, ref _hpKeyDelay);
			TryFloatFromInputBox(EatKeyDelayInputBox, ref _mpKeyDelay);
		}

		private void CameraYawDelayInputBox_InputChanged(object sender, EventArgs e)
		{
			TryFloatFromInputBox(CameraYawDelayInputBox, ref _timedCameraYawDelay);
		}

		#endregion

		#region Checkbox Events

		/// <summary> Enables/disables auto combat </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AutoCombatCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (!_hooked)
				return;

			if (!AutoCombatCheckBox.Checked)
			{
				StopAllCombatRelatedTimers();
				AutoCombatState.Text = "INACTIVE";
				LogDateMsg("Disabled AutoCombat", LogTypes.System);
				return;
			}

			if (_monsterTable.Count == 0)
			{
				LogDateMsg("Error: Empty Monstertable", LogTypes.System);
				AutoCombatCheckBox.Checked = false;
				return;
			}

			LogDateMsg("Enabled AutoCombat", LogTypes.System);
			HpFoodCheckBox.Enabled = true;
			MpFoodCheckBox.Enabled = true;
			_xpBeforeKill = -1;
			SwitchToTargetting();
		}

		/// <summary> Enables/disables auto hp food </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HpFoodCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (!_hooked)
				return;

			if (HpFoodCheckBox.Checked)
			{
				LogDateMsg("Enabled Auto Food HP", LogTypes.System);
				StartTimer(HpFoodTimer, (int)(_foodDelay * 1000));
				return;
			}

			LogDateMsg("Disabled Auto Food HP", LogTypes.System);
			StopTimer(HpFoodTimer);
		}

		/// <summary> Enables/disables auto mp food </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MpFoodCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (!_hooked)
				return;

			if (MpFoodCheckBox.Checked)
			{
				LogDateMsg("Enabled Auto Food MP", LogTypes.System);
				StartTimer(MpFoodTimer, (int)(_foodDelay * 1000));
				return;
			}

			LogDateMsg("Disabled Auto Food MP", LogTypes.System);
			StopTimer(MpFoodTimer);
		}

		/// <summary> Enables/disables force max camera zoom </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ForceMaxZoomCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (ForceMaxZoomCheckBox.Checked)
			{
				LogDateMsg("Enabled Force Max Camera Zoom", LogTypes.System);
				_forceCameraMaxZoom = true;
				return;
			}

			LogDateMsg("Disabled Force Max Camera Zoom", LogTypes.System);
			_forceCameraMaxZoom = false;
		}

		/// <summary> Enables/disables force top down camera </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ForceTopdownCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (ForceTopdownCheckBox.Checked)
			{
				LogDateMsg("Enabled Force Camera Topdown", LogTypes.System);
				_forceCameraTopDown = true;
				return;
			}

			LogDateMsg("Disabled Force Camera Topdown", LogTypes.System);
			_forceCameraTopDown = false;
		}

		/// <summary> Enables/disables second client mode </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SecondClientCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (SecondClientCheckBox.Checked)
			{
				LogDateMsg("Enabled 2nd Client Mode.", LogTypes.System);
				_dualClient = true;
				return;
			}

			LogDateMsg("Disabled 2nd Client Mode.", LogTypes.System);
			_dualClient = false;
		}

		/// <summary> Enables/disables timed camera yaw </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TimedCameraYawCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (TimedCameraYawCheckBox.Checked)
			{
				LogDateMsg("Enabled Timed Camera Yaw.", LogTypes.System);
				_timedCameraYaw = true;
				return;
			}

			LogDateMsg("Disabled Timed Camera Yaw.", LogTypes.System);
			_timedCameraYaw = false;
		}

		/// <summary> Enables/disables combat logging </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LogCombatCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			if (LogCombatCheckBox.Checked)
			{
				IgnoredLogTypes.Add(LogTypes.Combat);
				return;
			}
			IgnoredLogTypes.Remove(LogTypes.Combat);
		}

		/// <summary> Enables/disables food logging </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LogFoodCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			if (LogFoodCheckBox.Checked)
			{
				IgnoredLogTypes.Add(LogTypes.Food);
				return;
			}
			IgnoredLogTypes.Remove(LogTypes.Food);
		}

		/// <summary> Enables/disables camera logging </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LogCameraCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			if (LogCameraCheckBox.Checked)
			{
				IgnoredLogTypes.Add(LogTypes.Camera);
				return;
			}
			IgnoredLogTypes.Remove(LogTypes.Camera);
		}

		#endregion

	}
}