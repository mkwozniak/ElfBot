namespace ElfBot
{
	using Process = System.Diagnostics.Process;
	using Console = System.Console;
	using Environment = System.Environment;
	using Form = System.Windows.Forms.Form;
	using MaskedTextBox = System.Windows.Forms.MaskedTextBox;
	using MonsterList = System.Collections.Generic.List<string>;
	using MonsterHashTable = System.Collections.Generic.HashSet<string>;

	using DateTime = System.DateTime;
	using StringComparison = System.StringComparison;

	public sealed partial class MainForm : Form
	{
		#region System Methods

		/// <summary> Gets a proc id by name. </summary>
		/// <param name="name"> The name of the process. </param>
		/// <returns> The id of the process if it exists, otherwise returns 0. </returns>
		public int GetProcIdFromName(string name) //new 1.0.2 function
		{
			Process[] processlist = Process.GetProcesses();

			if (name.ToLower().Contains(".exe"))
				name = name.Replace(".exe", "");
			if (name.ToLower().Contains(".bin")) // test
				name = name.Replace(".bin", "");

			bool foundClient = false;
			int mainID = 0;

			foreach (Process theprocess in processlist)
			{
				//find (name).exe in the process list (use task manager to find the name)
				if (theprocess.ProcessName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
				{
					if (!foundClient)
					{
						foundClient = true;
						mainID = theprocess.Id;
						continue;
					}

					if (_dualClient && foundClient)
					{
						mainID = theprocess.Id;
						Globals.Logger.Debug($"Hooking to second ROSE client with PID {mainID}", LogEntryTag.System);
						return mainID;
					}
				}
			}

			if (foundClient)
			{
				return mainID;
			}

			return mainID; //if we fail to find it
		}

		/// <summary> Tries to open and hook to rose online process. </summary>
		/// <returns></returns>
		private bool TryOpenProcess()
		{
			int pID = GetProcIdFromName("trose");

			if (pID > 0)
			{
				Globals.TargetApplicationMemory.OpenProcess(pID);
				Globals.Logger.Info($"Successfully hooked ROSE process with PID {pID}", LogEntryTag.System);
				OnFinishedHooking.Invoke();
				Globals.Hooked = true;
				return true;
			}

			Globals.Logger.Warn($"Process PID {pID} was invalid and could not be hooked", LogEntryTag.System);
			ProcessHookLabel.Text = "Process Hook Failed :(";
			ProcessHookLabel.ForeColor = System.Drawing.Color.Red;
			return false;
		}

		#endregion

		#region Bot Methods

		/// <summary> Prepares the bot for starting </summary>
		public void PrepareElfBot()
		{
			_monsterTable = new MonsterHashTable();
			AutoCombatCheckBox.Enabled = false;

			CombatOptionsPanel.Visible = true;
			CombatOptionsPanel.Size = new System.Drawing.Size(306, 443);
			CombatOptionsBtn.ForeColor = System.Drawing.Color.Green;

			HpFoodCheckBox.Enabled = false;
			MpFoodCheckBox.Enabled = false;
			CombatCameraCheckBox.Enabled = false;
			TimedCameraYawCheckBox.Enabled = false;
			OnFinishedHooking += FinishHook;

			// worker threads could be useful later
			// if (!worker.IsBusy)  {  //worker.RunWorkerAsync();  }
		}

		/// <summary> Callback for when process has hooked. </summary>
		private void FinishHook()
		{
			ProcessHookLabel.Text = "Process Hooked!";
			ProcessHookLabel.ForeColor = System.Drawing.Color.LimeGreen;
			AutoCombatCheckBox.Enabled = true;
			HpFoodCheckBox.Enabled = true;
			MpFoodCheckBox.Enabled = true;
			CombatCameraCheckBox.Enabled = true;
			TimedCameraYawCheckBox.Enabled = true;
		}

		/// <summary> Rebuilds the monster list from the monster hash table </summary>
		private void RebuildMonsterList()
		{
			MonsterList monsterList = new MonsterList(_monsterTable);
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

		/// <summary> Loads a string array of monsters into the monster hash table then rebuilds the list </summary>
		/// <param name="monsters"> The list of monsters. </param>
		private void LoadToMonsterTable(string[] monsters)
		{
			_monsterTable.Clear();
			for (int i = 0; i < monsters.Length; i++)
			{
				if (monsters[i].Length > 0)
				{
					Globals.Logger.Info($"Added monster {monsters[i]} to monster table", LogEntryTag.Combat);
					_monsterTable.Add(monsters[i]);
				}
			}
			RebuildMonsterList();
		}

		/// <summary> Checks if the last targetted enemy has been killed by checking XP gain. </summary>
		private void CheckTargetKilled()
		{
			// if current xp is greater than our xp while targetting
			if (_currentXP > _xpBeforeKill)
			{
				_targetDefeatedMsg = Addresses.TargetDefeatedMessage.GetValue();
				Globals.Logger.Debug($"Defeated target and received message \"{_targetDefeatedMsg}\"",
					LogEntryTag.Combat);
				StopTimer(AttackTimeoutTimer);
				_pressedTargetting = false;

				if (combatLootCheckbox.Checked)
				{
					// enemy has died, loot now and start the loot timer
					_combatState = CombatStates.Looting;
					StopTimer(CombatTimer);
					// start the looting timer for hotkey
					StartTimer(LootingTimer, (int)(_actionDelay * 1000));
					// start the timer to end that 
					StartTimer(LootingEndTimer, (int)(_lootForSeconds * 1000));
					return;
				}

				// combat looting disabled, go back to targetting
				StopTimer(CombatTimer);
				SwitchToTargetting(true);
			}
		}

		/// <summary> Checks if the bot should be pressing to attack a target </summary>
		private void CheckShouldAttackTarget()
		{
			// if target uid is not 0 and there are combat keys and no defeat message 
			if (_currentTargetUID != 0 && _activeCombatKeys.Count > 0 && _targetDefeatedMsg.Length == 0)
			{
				// choose random attack and press key
				int ranSkill = _ran.Next(0, _activeCombatKeys.Count);
				_sim.Keyboard.KeyPress(_activeCombatKeys[ranSkill]);

				Globals.Logger.Debug($"Attack tick: {_activeCombatKeys[ranSkill]}", LogEntryTag.Combat);

				if (!AttackTimeoutTimer.Enabled)
				{
					// start timeout timer incase this target gets the bot stuck
					StartTimer(AttackTimeoutTimer, (int)(_retargetTimeout * 1000));
				}
			}
		}

		/// <summary> Switches the bot state to targetting mode </summary>
		/// <param name="resetUID"></param>
		private void SwitchToTargetting(bool resetUID = false)
		{
			if (resetUID)
			{
				_currentTargetUID = -1;
			}

			_currentTarget = "";
			_pressedTargetting = false;
			_combatState = CombatStates.Targetting;
			StartTimer(TargettingTimer, (int)(_actionDelay * 1000));
		}

		#endregion

		#region Form Methods

		/// <summary> Hides all panels visibility and controls. </summary>
		private void HideAllPanels()
		{
			CombatOptionsPanel.Visible = false;
			LootOptionsPanel.Visible = false;
			FoodOptionsPanel.Visible = false;
			MonsterTablePanel.Visible = false;
			CombatOptionsBtn.ForeColor = System.Drawing.Color.Black;
			LootOptionsBtn.ForeColor = System.Drawing.Color.Black;
			FoodOptionsBtn.ForeColor = System.Drawing.Color.Black;
			MonsterTableBtn.ForeColor = System.Drawing.Color.Black;
		}

		/// <summary> Tries to parse a float in an input box and stores the value in the referenced float. </summary>
		/// <param name="box"> The box to try to parse the float. </param>
		/// <param name="write"> The float to store the parsed value. </param>
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
	}
}