namespace ElfBot
{
	using System;
	using System.Windows.Forms;
	using WindowsInput.Native;

	public sealed partial class MainForm : Form
	{
		private void targetting_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();

			// unassign timer event
			StopTimer(targettingTimer);

			if (_currentTargetUID != -1)
			{
				_currentTarget = Addresses.Target.GetValue(); // make sure we are on the target we want.
				_currentTargetUID = Addresses.TargetId.GetValue();
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
				XPBeforeKillLabel.Text = $@"XP Before Kill: {_currentXP}";
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
			_currentTarget = Addresses.Target.GetValue();
			_currentTargetUID = Addresses.TargetId.GetValue();

			LogDateMsg("Checking Target Tick");

			StopTimer(checkTimer);

			// if current target is in monster table
			if (MonsterTable.Contains(_currentTarget) && _currentTargetUID != 0)
			{
				StopTimer(attackTimeoutTimer); // stop timeout timer

				// go into attack state
				StopTimer(combatTimer);

				// keep track of last position before going into attack mode 
				_lastXPos = Addresses.PositionX.GetValue();
				_lastYPos = Addresses.PositionY.GetValue();

				_targetDefeatedMsg = "";
				_combatState = CombatStates.Attacking;

				if (!combatTimer.Enabled)
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

			_currentXP = Addresses.Xp.GetValue(); // get current xp
			_currentTarget = Addresses.Target.GetValue(); // make sure we are on the target we want.
			_currentTargetUID = Addresses.TargetId.GetValue();
			_playerMaxMP = Addresses.MaxMp.GetValue();

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
			if (!Globals.Hooked)
			{
				return;
			}

			// Primary window
			AutoCombatState.Text = _combatState.ToString();

			// Character information
			String name = Addresses.CharacterName.GetValue();
			int level = Addresses.Level.GetValue();
			_currentXP = Addresses.Xp.GetValue();
			int zuly = Addresses.Zuly.GetValue();
			PlayerNameLabel.Text = $@"Name: {name}";
			PlayerLevelLabel.Text = $@"Level: {level}";
			CurrentXPLabel.Text = $@"XP: {_currentXP:n0}";
			PlayerZulyLabel.Text = $@"Zuly: {zuly:n0}";

			// Location information
			float x = Addresses.PositionX.GetValue();
			float y = Addresses.PositionY.GetValue();
			float z = Addresses.PositionZ.GetValue();
			int mapId = Addresses.MapId.GetValue();
			PlayerPosXLabel.Text = $@"X: {x}";
			PlayerPosYLabel.Text = $@"Y: {y}";
			PlayerPosZLabel.Text = $@"Z: {z}";
			PlayerMapIdLabel.Text = $@"Map ID: {mapId}";

			// Status information
			int hp = Addresses.Hp.GetValue();
			int maxHp = Addresses.MaxHp.GetValue();
			int mp = Addresses.Mp.GetValue();
			int maxMp = Addresses.MaxMp.GetValue();
			PlayerHPLabel.Text = $@"HP: {hp} / {maxHp}";
			PlayerMPLabel.Text = $@"MP: {mp} / {maxMp}";

			// Misc information
			_currentTarget = Addresses.Target.GetValue();
			_currentTargetUID = Addresses.TargetId.GetValue();
			TargetLabel.Text = $@"Target: {_currentTarget}";
			TargetUIDLabel.Text = $@"Target UID: {_currentTargetUID}";
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

			_playerHP = Addresses.Hp.GetValue();
			_playerMaxHP = Addresses.MaxHp.GetValue();

			if (_playerHP == 0 || _playerMaxHP == 0)
				return;

			float hpPercent = ((float)(_playerHP) / (float)(_playerMaxHP));
			string desiredHP = hpComboBox.Text;

			LogDateMsg($"Checking Food Tick HP: {_playerHP}/{_playerMaxHP}({(int)(hpPercent * 100f)}%)");

			if (hpPercent < Percentages[desiredHP] && _eatHPFood)
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

			_playerHP = Addresses.Hp.GetValue();
			_playerMaxHP = Addresses.MaxHp.GetValue();

			if (_playerMP == 0 || _playerMaxMP == 0)
				return;

			float mpPercent = ((float)(_playerMP) / (float)(_playerMaxMP));

			string desiredMP = mpComboBox.Text;

			LogDateMsg($"Checking Food Tick MP: {_playerMP}/{_playerMaxMP}({(int)(mpPercent * 100f)}%)");

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
			_currentXP = Addresses.Xp.GetValue();
			float x = Addresses.PositionX.GetValue();
			float y = Addresses.PositionY.GetValue();

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
			if (!Globals.Hooked)
				return;

			CameraZoomLabel.Text = $@"Zoom: {Addresses.CameraZoom.GetValue()}";
			CameraPitchLabel.Text = $@"Pitch: {Addresses.CameraPitch.GetValue()}";
			CameraYawLabel.Text = $@"Yaw: {Addresses.CameraYaw.GetValue()}";

			if (_forceCameraMaxZoom)
			{
				LogDateMsg("Force Zoom Camera Tick");
				Addresses.CameraZoom.writeValue(CameraMaxZoom);
			}

			if (_forceCameraTopDown)
			{
				LogDateMsg("Force Topdown Camera Tick");
				Addresses.CameraPitch.writeValue(CameraMaxPitch);
			}
		}

		private void CameraYawTimer_Tick(object sender, EventArgs e)
		{
			if (!Globals.Hooked || !_timedCameraYaw)
				return;

			LogDateMsg("Timed Camera Yaw Tick");
			_sim.Mouse.VerticalScroll(-1);
			int ranRot = _ran.Next(0, 2);
			Addresses.CameraYaw.writeValue(_cameraYawRotations[ranRot]);
		}
	}
}