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
				// update labels
				XPBeforeKillLabel.Text = "XP Before Kill: " + _currentXP.ToString();
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
				CheckShouldAttackTarget();
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

			// Primary window
			AutoCombatState.Text = _combatState.ToString();

			// Character information
			String name = _mem.ReadString(Addresses["PlayerName"]);
			int level = _mem.Read2Byte(Addresses["PlayerLevel"]);
			_currentXP = _mem.ReadInt(Addresses["CurrentXP"]);
			int zuly = _mem.ReadInt(Addresses["Zuly"]);
			PlayerNameLabel.Text = "Name: " + name.ToString();
			PlayerLevelLabel.Text = "Level: " + level.ToString();
			CurrentXPLabel.Text = "XP: " + $"{_currentXP:n0}";
			PlayerZulyLabel.Text = "Zuly: " + $"{zuly:n0}";

			// Location information
			float x = _mem.ReadFloat(Addresses["PlayerXPos"]);
			float y = _mem.ReadFloat(Addresses["PlayerYPos"]);
			float z = _mem.ReadFloat(Addresses["PlayerZPos"]);
			int mapId = _mem.ReadInt(Addresses["MapID"]);
			PlayerPosXLabel.Text = "X: " + x.ToString();
			PlayerPosYLabel.Text = "Y: " + y.ToString();
			PlayerPosZLabel.Text = "Z: " + z.ToString();
			PlayerMapIdLabel.Text = "Map ID: " + mapId.ToString();

			// Status information
			int hp = _mem.ReadInt(Addresses["PlayerHP"]);
			int maxHp = _mem.ReadInt(Addresses["PlayerMaxHP"]);
			int mp = _mem.ReadInt(Addresses["PlayerMP"]);
			int maxMp = _mem.ReadInt(Addresses["PlayerMaxMP"]);
			PlayerHPLabel.Text = "HP: " + hp.ToString() + " / " + maxHp.ToString();
			PlayerMPLabel.Text = "MP: " + mp.ToString() + " / " + maxMp.ToString();

			// Misc information
			_currentTarget = _mem.ReadString(Addresses["CurrentTarget"]); // make sure we are on the target we want.
			_currentTargetUID = _mem.ReadInt(Addresses["TargetUID"]);
			TargetLabel.Text = "Target: " + _currentTarget;
			TargetUIDLabel.Text = "Target UID: " + _currentTargetUID.ToString();
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

			if ((_targetDefeatedMsg.Length == 0 && _currentXP == _xpBeforeKill) || (x == _lastXPos && y == _lastYPos))
			{
				LogDateMsg("Attack Timeout Tick");
				StopAllTimers();
				SwitchToTargetting(true);
			}
		}

		private void cameraTimer_Tick(object sender, EventArgs e)
		{
			if (!_hooked)
				return;

			CameraZoomLabel.Text = "Camera Zoom: " + _mem.ReadFloat(Addresses["CameraZoom"]).ToString();
			CameraPitchLabel.Text = "Camera Pitch: " + _mem.ReadFloat(Addresses["CameraPitch"]).ToString();
			CameraYawLabel.Text = "Camera Yaw: " + _mem.ReadFloat(Addresses["CameraYaw"]).ToString();

			if (_forceCameraMaxZoom)
			{
				LogDateMsg("Force Zoom Camera Tick");
				_mem.WriteMemory(Addresses["CameraZoom"], "float", _cameraMaxZoom);
			}

			if (_forceCameraTopDown)
			{
				LogDateMsg("Force Topdown Camera Tick");
				_mem.WriteMemory(Addresses["CameraPitch"], "float", _cameraMaxPitch);
			}
		}

		private void CameraYawTimer_Tick(object sender, EventArgs e)
		{
			if (!_hooked || !_timedCameraYaw)
				return;

			LogDateMsg("Timed Camera Yaw Tick");
			_sim.Mouse.VerticalScroll(-1);
			int ranRot = _ran.Next(0, 2);
			_mem.WriteMemory(Addresses["CameraYaw"], "float", _cameraYawRotations[ranRot]);
		}
	}
}