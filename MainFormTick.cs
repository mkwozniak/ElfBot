namespace ElfBot
{
	using System;
	using System.Windows.Forms;
	using WindowsInput.Native;
	using WindowsInput;

	public partial class MainForm : Form
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
				_lastXPos = _mem.ReadFloat(Addresses["PlayerXPos"]);
				_lastYPos = _mem.ReadFloat(Addresses["PlayerYPos"]);
				_targetDefeatedMsg = "";
				_pressedCombatKey = false;
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
			_playerMaxMana = _mem.ReadInt(Addresses["PlayerMaxMana"]);

			if (MonsterTable.Contains(_currentTarget))
			{
				CheckTargetKilled();

				if (_currentTargetUID != 0 && ActiveCombatKeys.Count > 0 && _targetDefeatedMsg.Length == 0)
				{
					int ranSkill = _ran.Next(0, ActiveCombatKeys.Count);
					LogDateMsg("Attack Tick: " + ActiveCombatKeys[ranSkill].ToString());
					_sim.Keyboard.KeyPress(ActiveCombatKeys[ranSkill]); // attack press
					_pressedCombatKey = true;

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
			StopTimer(interfaceTimer);

			TargetLabel.Text = _currentTarget;
			TargetUIDLabel.Text = _currentTargetUID.ToString();
			AutoCombatState.Text = _combatState.ToString();
			PlayerPosLabel.Text = "X: " + _lastXPos + "Y: " + _lastYPos;
			maxManaLabel.Text = "Max Mana: " + _playerMaxMana.ToString();
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
			StartTimer(targettingTimer, (int)(_actionDelay * 1000));
		}

		private void retargetTimeout_Tick(object sender, EventArgs e)
		{
			_currentXP = _mem.ReadInt(Addresses["CurrentXP"]); // get current xp
			StopTimer(attackTimeoutTimer);

			if (_targetDefeatedMsg.Length == 0 && _currentXP == _xpBeforeKill)
			{
				LogDateMsg("Attack Timeout Tick");
				StopAllTimers();
				SwitchToTargetting(true);
			}
		}
	}
}