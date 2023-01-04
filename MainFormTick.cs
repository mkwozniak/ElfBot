namespace ElfBot
{
	using EventArgs = System.EventArgs;
	using EventHandler = System.EventHandler;
	using Form = System.Windows.Forms.Form;
	using VirtualKeyCode = WindowsInput.Native.VirtualKeyCode;
	using Timer = System.Windows.Forms.Timer;
	using Math = System.Math;

	public sealed partial class MainForm : Form
	{
		#region Timer Methods
		private void ListenToTimer(Timer timer, EventHandler del)
		{
			timer.Tick += new EventHandler(del);
		}

		private void StartTimer(Timer timer, int msDelay)
		{
			timer.Interval = msDelay;
			timer.Start();
		}

		private void StopTimer(Timer timer)
		{
			timer.Stop();
			timer.Enabled = false;
		}

		private void StopAllCombatRelatedTimers()
		{
			StopTimer(CombatTimer);
			StopTimer(TargettingTimer);
			StopTimer(CheckTimer);
			StopTimer(LootingEndTimer);
			StopTimer(LootingTimer);
			StopTimer(AttackTimeoutTimer);
		}

		#endregion

		#region Timer Tick Methods

		/// <summary>
		/// Timer tick for when bot is targetting
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Targetting_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();

			StopTimer(TargettingTimer);

			if (_currentTargetUID != -1)
			{
				_currentTarget = Addresses.Target.GetValue(); // make sure we are on the target we want.
				_currentTargetUID = Addresses.TargetId.GetValue();
			}

			// if current target isnt in monstertable or there is no unique target id
			if ((!_monsterTable.Contains(_currentTarget) || _currentTargetUID == 0) && !_pressedTargetting)
			{
				// press targetting key
				_sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
				_pressedTargetting = true;

				LogDateMsg("Target Tab Press Tick", LogTypes.Combat);

				_currentXP = Addresses.Xp.GetValue();
				_xpBeforeKill = _currentXP;

				// go into checking target mode to make sure the tab target was OK
				// update labels
				XPBeforeKillLabel.Text = $@"XP Before Kill: {_currentXP}";
				StartTimer(CheckTimer, (int)(_actionDelay * 1000));
				_combatState = CombatStates.CheckingTarget;
				return;
			}

			SwitchToTargetting();
		}

		/// <summary>
		/// Timer tick for when bot is checking its target
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CheckingTarget_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();

			// get target memory
			_currentTarget = Addresses.Target.GetValue();
			_currentTargetUID = Addresses.TargetId.GetValue();

			LogDateMsg("Checking Target Tick", LogTypes.Combat);

			StopTimer(CheckTimer);

			// if current target is in monster table
			if (_monsterTable.Contains(_currentTarget) && _currentTargetUID != 0)
			{
				StopTimer(AttackTimeoutTimer); // stop timeout timer

				// go into attack state
				StopTimer(CombatTimer);

				// keep track of last position before going into attack mode 
				_lastXPos = Addresses.PositionX.GetValue();
				_lastYPos = Addresses.PositionY.GetValue();

				_targetDefeatedMsg = "";
				_combatState = CombatStates.Attacking;

				if(!CombatTimer.Enabled)
				{
					StartTimer(CombatTimer, (int)(_combatKeyDelay * 1000));
				}

				return;
			}

			SwitchToTargetting();
		}

		/// <summary>
		/// Timer tick for when bot is attacking its target
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Attacking_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();

			_currentXP = Addresses.Xp.GetValue(); // get current xp
			_currentTarget = Addresses.Target.GetValue(); // make sure we are on the target we want.
			_currentTargetUID = Addresses.TargetId.GetValue();
			_playerMaxMP = Addresses.MaxMp.GetValue();

			if (_monsterTable.Contains(_currentTarget))
			{
				CheckTargetKilled();
				CheckShouldAttackTarget();
			}

			// no proper target while attacking
			if (_currentTargetUID == 0)
			{
				// back to targetting
				StopTimer(CombatTimer);
				SwitchToTargetting();
			}

			// if the current XP is less than the xp before the last kill, then the char leveled up
			if (_currentXP < _xpBeforeKill)
			{
				LogDateMsg("Leveled Up. Resetting State.", LogTypes.System);

				// reset the xp before kill to -1
				_xpBeforeKill = -1;
				StopAllCombatRelatedTimers();

				// back to targetting
				SwitchToTargetting(true);
			}
		}

		/// <summary>
		/// Timer tick for bot interface values to update
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Interface_Tick(object sender, EventArgs e)
		{
			if (!Globals.Hooked)
			{
				return;
			}

			// Primary window
			AutoCombatState.Text = _combatState.ToString();

			// Character information
			string name = Addresses.CharacterName.GetValue();
			int level = Addresses.Level.GetValue();
			int xp = Addresses.Xp.GetValue();
			int zuly = Addresses.Zuly.GetValue();
			PlayerNameLabel.Text = $@"Name: {name}";
			PlayerLevelLabel.Text = $@"Level: {level}";
			CurrentXPLabel.Text = $@"XP: {xp:n0}";
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
			TargetLabel.Text = $@"Target: {(string.IsNullOrEmpty(_currentTarget) ? "N/A" : _currentTarget)}";
			TargetUIDLabel.Text = $@"Target UID: {_currentTargetUID}";
		}

		/// <summary>
		/// Timer tick for looting
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Loot_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();
			LogDateMsg("Looting Tick", LogTypes.Combat);
			_sim.Keyboard.KeyPress(VirtualKeyCode.VK_4);
		}

		/// <summary>
		/// Timer tick for looting finished
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LootEnd_Tick(object sender, EventArgs e)
		{
			AutoCombatState.Text = _combatState.ToString();
			StopTimer(LootingEndTimer);
			StopTimer(LootingTimer);
			LogDateMsg("End Loot Tick", LogTypes.Combat);

			SwitchToTargetting(true);
		}

		/// <summary>
		/// Timer tick for eating hp food
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HpFoodTimer_Tick(object sender, EventArgs e)
		{
			if (_activeHPKeys.Count == 0)
				return;

			_playerHP = Addresses.Hp.GetValue();
			_playerMaxHP = Addresses.MaxHp.GetValue();

			if (_playerHP == 0 || _playerMaxHP == 0)
				return;

			float hpPercent = ((float)(_playerHP) / (float)(_playerMaxHP));
			string desiredHP = hpComboBox.Text;

			LogDateMsg($"Checking Food Tick HP: {_playerHP}/{_playerMaxHP}({(int)(hpPercent * 100f)}%)", LogTypes.Food);

			if (hpPercent < _percentages[desiredHP] && _eatHPFood)
			{
				int ranFood = _ran.Next(0, _activeHPKeys.Count);
				LogDateMsg("Eat HP Food: " + _activeHPKeys[ranFood].ToString(), LogTypes.Food);
				_sim.Keyboard.KeyPress(_activeHPKeys[ranFood]); // food press
				_eatHPFood = false;
				// start the delay timer to press the key again
				StartTimer(HpFoodKeyTimer, (int)(_hpKeyDelay * 1000));
			}
		}

		/// <summary>
		/// Timer tick for eating mp food
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MpFoodTimer_Tick(object sender, EventArgs e)
		{
			if (_activeMPKeys.Count == 0)
				return;

			_playerHP = Addresses.Hp.GetValue();
			_playerMaxHP = Addresses.MaxHp.GetValue();

			if (_playerMP == 0 || _playerMaxMP == 0)
				return;

			float mpPercent = ((float)(_playerMP) / (float)(_playerMaxMP));

			string desiredMP = mpComboBox.Text;

			LogDateMsg($"Checking Food Tick MP: {_playerMP}/{_playerMaxMP}({(int)(mpPercent * 100f)}%)", LogTypes.Food);

			if (mpPercent < _percentages[desiredMP] && _eatMPFood)
			{
				int ranFood = _ran.Next(0, _activeMPKeys.Count);
				LogDateMsg("Eat MP Food: " + _activeMPKeys[ranFood].ToString(), LogTypes.Food);
				_sim.Keyboard.KeyPress(_activeMPKeys[ranFood]); // food press
				// start the delay timer to press the key again
				StartTimer(MpFoodKeyTimer, (int)(_mpKeyDelay * 1000));
				_eatMPFood = false;
			}
		}

		/// <summary>
		/// Timer tick to reset hp food key
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HpFoodKeyTimer_Tick(object sender, EventArgs e)
		{
			_eatHPFood = true;
			StopTimer(HpFoodKeyTimer);
		}

		/// <summary>
		/// Timer tick to reset mp food key
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MpFoodKeyTimer_Tick(object sender, EventArgs e)
		{
			_eatMPFood = true;
			StopTimer(MpFoodKeyTimer);
		}

		/// <summary>
		/// Timer tick to timeout retarget
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RetargetTimeout_Tick(object sender, EventArgs e)
		{
			_currentXP = Addresses.Xp.GetValue();
			float x = Addresses.PositionX.GetValue();
			float y = Addresses.PositionY.GetValue();

			StopTimer(AttackTimeoutTimer);

			if ((_targetDefeatedMsg.Length == 0 && _currentXP == _xpBeforeKill) || (x == _lastXPos && y == _lastYPos))
			{
				LogDateMsg("Attack Timeout Tick", LogTypes.Combat);
				StopAllCombatRelatedTimers();
				SwitchToTargetting(true);
			}
		}

		/// <summary>
		/// Timer tick for camera locks
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CameraTimer_Tick(object sender, EventArgs e)
		{
			if (!Globals.Hooked)
				return;

			CameraZoomLabel.Text = $@"Zoom: {Addresses.CameraZoom.GetValue()}";
			CameraPitchLabel.Text = $@"Pitch: {Addresses.CameraPitch.GetValue()}";
			CameraYawLabel.Text = $@"Yaw: {Addresses.CameraYaw.GetValue()}";

			if (_forceCameraMaxZoom)
			{
				//LogDateMsg("Force Zoom Camera Tick", LogTypes.Camera);
				Addresses.CameraZoom.writeValue(CameraMaxZoom);
			}

			if (_forceCameraTopDown)
			{
				//LogDateMsg("Force Topdown Camera Tick", LogTypes.Camera);
				Addresses.CameraPitch.writeValue(CameraMaxPitch);
			}
		}

		private void CameraYawTimer_Tick(object sender, EventArgs e)
		{
			if (!Globals.Hooked || !_timedCameraYaw)
				return;

			float waveform = (float) ((Math.PI / 1.1) * Math.Sin(0.25 * _yawCounter));
			Addresses.CameraYaw.writeValue(waveform);
			_yawCounter += 0.05;

			_rightClickCounter++;

			if (_rightClickCounter > 50)
			{
				System.Console.WriteLine("Right Clicking ... ");
				_rightClickCounter = 0;
				_sim.Mouse.VerticalScroll(-1);
			}
		}

		#endregion
	}
}