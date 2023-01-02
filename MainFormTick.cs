namespace ElfBot
{
	using System;
	using System.Windows.Forms;
	using WindowsInput.Native;
	using WindowsInput;

	public partial class MainForm : Form
	{
		private void mainTimer_Tick(object sender, EventArgs e)
		{
			LogDateMsg("Main Timer Tick");
			
			if (_combatState == CombatStates.Attacking && !combatTimer.Enabled)
			{
				_pressedCombatKey = false;
				StartTimer(combatTimer, (int)(_combatKeyDelay * 1000));
				StartTimer(attackTimeoutTimer, (int)(_retargetTimeout * 1000));
			}

			if (_combatState == CombatStates.Targetting && !targettingTimer.Enabled)
			{
				StartTimer(targettingTimer, (int)(_actionDelay * 1000));
			}

			if (_combatState == CombatStates.CheckingTarget && !checkTimer.Enabled)
			{
				StartTimer(checkTimer, (int)(_actionDelay * 1000));
			}

			// TODO: Get item count memory to skip looting phase if 0
			if (_combatState == CombatStates.Looting && !lootingTimer.Enabled)
			{
				StartTimer(lootingTimer, (int)(_actionDelay * 1000));
			}

			if (!interfaceTimer.Enabled)
			{
				StartTimer(interfaceTimer, 60);
			}
		}

		private void targetting_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();

			// unassign timer event
			StopTimer(targettingTimer);

			// if current target isnt in monstertable or there is no unique target id
			if ((!MonsterTable.Contains(_currentTarget) || _currentTargetUID == 0) && !_pressedTargetting)
			{
				StopTimer(attackTimeoutTimer);

				// press targetting key
				sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
				_pressedTargetting = true;

				LogDateMsg("Target Tab Press");

				// go into checking target mode to make sure the tab target was OK
				_combatState = CombatStates.CheckingTarget;

				// update XP values
				_currentXP = m.ReadInt(Addresses["CurrentXP"]);
				_xpBeforeKill = _currentXP;

				// update labels
				CurrentXPLabel.Text = "Current XP: " + _currentXP.ToString();
				CurrentXPLabel.Text = "XP Before Kill: " + _xpBeforeKill.ToString();

				return;
			}

			_combatState = CombatStates.CheckingTarget;
		}

		private void checkingTarget_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();

			// get target memory
			_currentTarget = m.ReadString(Addresses["CurrentTarget"]);
			_currentTargetUID = m.ReadInt(Addresses["TargetUID"]);

			LogDateMsg("Checking Target ...");

			StopTimer(checkTimer);

			// if current target is in monster table
			if (MonsterTable.Contains(_currentTarget) && _currentTargetUID != 0)
			{
				// go into attack state
				_pressedCombatKey = false;
				// unassign timer event
				StopTimer(combatTimer);

				_combatState = CombatStates.Attacking;
				return;
			}

			_currentTarget = "";
			_pressedTargetting = false;
			_combatState = CombatStates.Targetting;

		}

		private void attacking_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();

			_currentXP = m.ReadInt(Addresses["CurrentXP"]); // get current xp
			_currentTarget = m.ReadString(Addresses["CurrentTarget"]); // make sure we are on the target we want.
			_currentTargetUID = m.ReadInt(Addresses["TargetUID"]);

			StopTimer(combatTimer);
			StartTimer(attackTimeoutTimer, (int)(_retargetTimeout * 1000));

			if (MonsterTable.Contains(_currentTarget))
			{
				LogDateMsg("Found Target.");

				CheckTargetKilled();

				if (_currentTargetUID != 0 && ActiveCombatKeys.Count > 0 && !_pressedCombatKey)
				{
					int ranSkill = _ran.Next(0, ActiveCombatKeys.Count);
					Console.WriteLine("Using Skill: " + ActiveCombatKeys[ranSkill].ToString());
					sim.Keyboard.KeyPress(ActiveCombatKeys[ranSkill]); // attack press
					_pressedCombatKey = true;
				}
			}

			// no proper target while attacking
			if (_currentTargetUID == 0)
			{
				// back to targetting
				_currentTarget = "";
				_pressedTargetting = false;
				_combatState = CombatStates.Targetting;
			}

			// if the current XP is less than the xp before the last kill, then the char leveled up
			if (_currentXP < _xpBeforeKill)
			{
				// reset the xp before kill to -1
				_xpBeforeKill = -1;
				// back to targetting
				_combatState = CombatStates.Targetting;
			}
		}

		private void interface_Tick(object sender, EventArgs e)
		{
			StopTimer(interfaceTimer);

			TargetLabel.Text = _currentTarget;
			TargetUIDLabel.Text = _currentTargetUID.ToString();
			AutoCombatState.Text = _combatState.ToString();
		}

		private void loot_Tick(object sender, EventArgs e)
		{
			StopTimer(lootingTimer);

			LogDateMsg("Looting Tick");
			AutoCombatState.Text = "Looting";
			sim.Keyboard.KeyPress(VirtualKeyCode.VK_4);
		}

		private void lootEnd_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();

			StopTimer(lootTimer);

			LogDateMsg("End Loot Tick");
			// go back to targetting
			_currentTarget = "";
			_pressedTargetting = false;
			_combatState = CombatStates.Targetting;
		}

		private void retargetTimeout_Tick(object sender, EventArgs e)
		{
			if (_combatState == CombatStates.Attacking)
			{
				LogDateMsg("Attack Timeout Tick");

				// go back to targetting
				_currentTarget = "";
				_pressedTargetting = false;
				_combatState = CombatStates.Targetting;
			}

			StopTimer(attackTimeoutTimer);
		}
	}
}