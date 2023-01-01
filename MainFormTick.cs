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
			if(_combatState == CombatStates.Attacking && !combatTimer.Enabled)
			{
				ListenToTimer(combatTimer, (int)(_actionDelay * 1000), attacking_Tick);
			}

			if(_combatState == CombatStates.Targetting && !targettingTimer.Enabled)
			{
				ListenToTimer(targettingTimer, (int)(_actionDelay * 1000), targetting_Tick);
			}

			if (_combatState == CombatStates.CheckingTarget && !checkTimer.Enabled)
			{
				ListenToTimer(checkTimer, (int)(_actionDelay * 1000), checkingTarget_Tick);
			}

			if (_combatState == CombatStates.Looting)
			{
				AutoCombatState.Text = "Looting";
				sim.Keyboard.KeyPress(VirtualKeyCode.VK_4);
			}

			TargetLabel.Text = _currentTarget;
			TargetUIDLabel.Text = _currentTargetUID.ToString();
			AutoCombatState.Text = _combatState.ToString();
		}

		private void targetting_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();

			// unassign timer event
			targettingTimer.Tick -= targetting_Tick;
			targettingTimer.Enabled = false;

			// if current target isnt in monstertable or there is no unique target id
			if (!MonsterTable.Contains(_currentTarget) || _currentTargetUID == 0)
			{
				// press targetting key
				sim.Keyboard.KeyPress(VirtualKeyCode.TAB);

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

			// unassign timer event
			checkTimer.Tick -= checkingTarget_Tick;
			checkTimer.Enabled = false;

			// get target memory
			_currentTarget = m.ReadString(Addresses["CurrentTarget"]);
			_currentTargetUID = m.ReadInt(Addresses["TargetUID"]);

			// if current target is in monster table
			if (MonsterTable.Contains(_currentTarget))
			{
				// go into attack state
				Console.WriteLine("Found Target");
				_combatState = CombatStates.Attacking;
				return;
			}

			_combatState = CombatStates.Targetting;
		}

		private void attacking_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();

			// unassign timer event
			combatTimer.Tick -= attacking_Tick;
			combatTimer.Enabled = false;

			_currentXP = m.ReadInt(Addresses["CurrentXP"]); // get current xp
			_currentTarget = m.ReadString(Addresses["CurrentTarget"]); // make sure we are on the target we want.
			_currentTargetUID = m.ReadInt(Addresses["TargetUID"]);

			if (MonsterTable.Contains(_currentTarget))
			{
				// if current xp is greater than our xp while targetting
				if (_currentXP > _xpBeforeKill)
				{
					if(combatLootCheckbox.Checked)
					{
						// enemy has died, loot now and start the loot timer
						_combatState = CombatStates.Looting;
						lootTimer.Interval = (int)(_lootForSeconds * 1000);
						lootTimer.Tick += new EventHandler(lootEnd_Tick);
						lootTimer.Enabled = true;
					}
					else
					{
						_currentTarget = "";
						_combatState = CombatStates.Targetting;
					}
				}

				if (_currentTargetUID != 0)
				{
					sim.Keyboard.KeyPress(VirtualKeyCode.VK_1); // attack press
				}
			}

			// no proper target while attacking
			if (_currentTargetUID == 0)
			{
				// back to targetting
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

		private void lootEnd_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();

			// unassign timer event
			lootTimer.Tick -= new EventHandler(lootEnd_Tick);
			lootTimer.Enabled = false;

			// go back to targetting
			_currentTarget = "";
			_combatState = CombatStates.Targetting;
		}

		private void AutoCombatBox_CheckedChanged(object sender, EventArgs e)
		{
			if (!AutoCombatBox.Checked && mainTimer.Enabled)
			{
				mainTimer.Tick -= mainTimer_Tick;
				mainTimer.Enabled = false;
				AutoCombatLabel.Text = "DISABLED";
				_currentTarget = "None";
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
			mainTimer.Interval = 1000;
			mainTimer.Tick += new EventHandler(mainTimer_Tick);
			mainTimer.Enabled = true;
			_xpBeforeKill = -1;
			_combatState = CombatStates.Targetting;
		}
	}
}