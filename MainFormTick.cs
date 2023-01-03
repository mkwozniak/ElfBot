namespace ElfBot
{
	using System;
	using System.Windows.Forms;
	using WindowsInput.Native;
	using WindowsInput;

	public sealed partial class MainForm : Form
	{

		private void targetting_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();

			// unassign timer event
			StopTimer(targettingTimer);

			if (_currentTargetUID != -1)
			{
				_currentTarget = _mem.ReadString(Addresses["CurrentTarget"]); // make sure we are on the target we want.
				_currentTargetUID = _mem.ReadInt(Addresses["TargetUID"]);
			}

			// if current target isnt in monstertable or there is no unique target id
			if ((!MonsterTable.Contains(_currentTarget) || _currentTargetUID == 0) && !_pressedTargetting)
			{
				// press targetting key
				_sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
				_pressedTargetting = true;

				LogDateMsg("Target Tab Press Tick");

				// go into checking target mode to make sure the tab target was OK

				// update XP values
				_currentXP = _mem.ReadInt(Addresses["CurrentXP"]);
				_xpBeforeKill = _currentXP;

				// update labels
				CurrentXPLabel.Text = "Current XP: " + _currentXP.ToString();
				CurrentXPLabel.Text = "XP Before Kill: " + _xpBeforeKill.ToString();
				StartTimer(checkTimer, (int)(_actionDelay * 1000));
				_combatState = CombatStates.CheckingTarget;

				return;
			}

			SwitchToTargetting();
		}

		private void checkingTarget_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();

			// get target memory
			_currentTarget = _mem.ReadString(Addresses["CurrentTarget"]);
			_currentTargetUID = _mem.ReadInt(Addresses["TargetUID"]);

			LogDateMsg("Checking Target Tick");

			StopTimer(checkTimer);

			// if current target is in monster table
			if (MonsterTable.Contains(_currentTarget) && _currentTargetUID != 0)
			{
				StopTimer(attackTimeoutTimer); // stop timeout timer

				// go into attack state
				StopTimer(combatTimer);

				// keep track of last position before going into attack mode 
				_lastXPos = _mem.ReadFloat(Addresses["PlayerXPos"]);
				_lastYPos = _mem.ReadFloat(Addresses["PlayerYPos"]);

				_targetDefeatedMsg = "";
				_combatState = CombatStates.Attacking;

				if(!combatTimer.Enabled)
				{
					StartTimer(combatTimer, (int)(_combatKeyDelay * 1000));
				}

				return;
			}

			SwitchToTargetting();
		}

		private void attacking_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();

			_currentXP = _mem.ReadInt(Addresses["CurrentXP"]); // get current xp
			_currentTarget = _mem.ReadString(Addresses["CurrentTarget"]); // make sure we are on the target we want.
			_currentTargetUID = _mem.ReadInt(Addresses["TargetUID"]);
			_playerMaxMP = _mem.ReadInt(Addresses["PlayerMaxMP"]);

			if (MonsterTable.Contains(_currentTarget))
			{
				CheckTargetKilled();

				if (_currentTargetUID != 0 && ActiveCombatKeys.Count > 0 && _targetDefeatedMsg.Length == 0)
				{
					int ranSkill = _ran.Next(0, ActiveCombatKeys.Count);
					LogDateMsg("Attack Tick: " + ActiveCombatKeys[ranSkill].ToString());
					_sim.Keyboard.KeyPress(ActiveCombatKeys[ranSkill]); // attack press

					if (!attackTimeoutTimer.Enabled)
					{
						StartTimer(attackTimeoutTimer, (int)(_retargetTimeout * 1000));
					}
				}
			}

			// no proper target while attacking
			if (_currentTargetUID == 0)
			{
				// back to targetting
				StopTimer(combatTimer);
				SwitchToTargetting();
			}

			// if the current XP is less than the xp before the last kill, then the char leveled up
			if (_currentXP < _xpBeforeKill)
			{
				LogDateMsg("Leveled Up. Resetting State.");

				// reset the xp before kill to -1
				_xpBeforeKill = -1;
				StopAllTimers();

				// back to targetting
				SwitchToTargetting(true);
			}
		}

		private void interface_Tick(object sender, EventArgs e)
		{
			if(!_hooked)
			{
				return;
			}

			TargetLabel.Text = _currentTarget;
			TargetUIDLabel.Text = _currentTargetUID.ToString();
			AutoCombatState.Text = _combatState.ToString();
			CurrentXPLabel.Text = _currentXP.ToString();
			PlayerPosLabel.Text = "X: " + _lastXPos + "Y: " + _lastYPos;
			maxManaLabel.Text = "Max Mana: " + _playerMaxMP.ToString();
		}

		private void loot_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();
			LogDateMsg("Looting Tick");
			_sim.Keyboard.KeyPress(VirtualKeyCode.VK_4);
		}

		private void lootEnd_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();
			StopTimer(lootingEndTimer);
			StopTimer(lootingTimer);
			LogDateMsg("End Loot Tick");

			// go back to targetting
			_currentTarget = "";
			_pressedTargetting = false;
			_currentTargetUID = -1;
			_combatState = CombatStates.Targetting;
			SwitchToTargetting(true);
			StartTimer(targettingTimer, (int)(_actionDelay * 1000));
		}

		private void hpFoodTimer_Tick(object sender, EventArgs e)
		{
			if (ActiveHPKeys.Count == 0)
				return;

			_playerHP = _mem.ReadInt(Addresses["PlayerHP"]);
			_playerMaxHP = _mem.ReadInt(Addresses["PlayerMaxHP"]);

			if (_playerHP == 0 || _playerMaxHP == 0)
				return;

			float hpPercent = ((float)(_playerHP) / (float)(_playerMaxHP));
			string desiredHP = hpComboBox.Text;

			LogDateMsg("Checking Food Tick HP: " + _playerHP.ToString() + "/" + _playerMaxHP.ToString() 
				+ "(" + ((int)(hpPercent * 100f)).ToString() + "%)");

			if(hpPercent < Percentages[desiredHP] && _eatHPFood)
			{
				int ranFood = _ran.Next(0, ActiveHPKeys.Count);
				LogDateMsg("Eat HP Food: " + ActiveHPKeys[ranFood].ToString());
				_sim.Keyboard.KeyPress(ActiveHPKeys[ranFood]); // food press
				_eatHPFood = false;
				// start the delay timer to press the key again
				StartTimer(hpFoodKeyTimer, (int)(_hpKeyDelay * 1000));
			}
		}

		private void mpFoodTimer_Tick(object sender, EventArgs e)
		{
			if (ActiveMPKeys.Count == 0)
				return;

			_playerMP = _mem.ReadInt(Addresses["PlayerMP"]);
			_playerMaxMP = _mem.ReadInt(Addresses["PlayerMaxMP"]);

			if (_playerMP == 0 || _playerMaxMP == 0)
				return;

			float mpPercent = ((float)(_playerMP) / (float)(_playerMaxMP));

			string desiredMP = mpComboBox.Text;

			LogDateMsg("Checking Food Tick MP: " + _playerMP.ToString() + "/" + _playerMaxMP.ToString()
				+ "(" + ((int)(mpPercent * 100f)).ToString() + "%)");

			if (mpPercent < Percentages[desiredMP] && _eatMPFood)
			{
				int ranFood = _ran.Next(0, ActiveMPKeys.Count);
				LogDateMsg("Eat MP Food: " + ActiveMPKeys[ranFood].ToString());
				_sim.Keyboard.KeyPress(ActiveMPKeys[ranFood]); // food press
				// start the delay timer to press the key again
				StartTimer(mpFoodKeyTimer, (int)(_mpKeyDelay * 1000));
				_eatMPFood = false;
			}
		}

		private void hpFoodKeyTimer_Tick(object sender, EventArgs e)
		{
			_eatHPFood = true;
			StopTimer(hpFoodKeyTimer);
		}

		private void mpFoodKeyTimer_Tick(object sender, EventArgs e)
		{
			_eatMPFood = true;
			StopTimer(mpFoodKeyTimer);
		}

		private void retargetTimeout_Tick(object sender, EventArgs e)
		{
			_currentXP = _mem.ReadInt(Addresses["CurrentXP"]); // get current xp
			float x = _mem.ReadFloat(Addresses["PlayerXPos"]);
			float y = _mem.ReadFloat(Addresses["PlayerYPos"]);

			StopTimer(attackTimeoutTimer);

			if ((_targetDefeatedMsg.Length == 0 && _currentXP == _xpBeforeKill) && (x == _lastXPos && y == _lastYPos))
			{
				LogDateMsg("Attack Timeout Tick");
				StopAllTimers();
				SwitchToTargetting(true);
			}
		}
	}
}