namespace ElfBot
{
    using System.Windows;
    using Process = System.Diagnostics.Process;
    using Console = System.Console;
    using Environment = System.Environment;
    using MonsterList = System.Collections.Generic.List<string>;
    using MonsterHashTable = System.Collections.Generic.HashSet<string>;
    using TextBox = System.Windows.Controls.TextBox;

    using DateTime = System.DateTime;
    using StringComparison = System.StringComparison;
    using System;
    using WindowsInput;
    using System.Diagnostics;

    public sealed partial class MainWindow : Window
    {
        #region Form Init

        /// <summary> Form Constructor </summary>
        public MainWindow()
        {
            Closed += StopWindowBehavior;
            Loaded += PrepareElfBot;
            InitializeComponent();
        }

        #endregion

        #region System Methods

        /// <summary> Invoke when the window is closed </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopWindowBehavior(object? sender, EventArgs e)
        {
            StopTimer(CombatTimer);
            StopTimer(TargettingTimer);
            StopTimer(CheckTimer);
            StopTimer(LootingTimer);
            StopTimer(LootingEndTimer);
            StopTimer(InterfaceTimer);
            StopTimer(AttackTimeoutTimer);
            StopTimer(HpFoodTimer);
            StopTimer(MpFoodTimer);
            StopTimer(HpFoodKeyTimer);
            StopTimer(MpFoodKeyTimer);
            StopTimer(CombatCameraTimer);
            StopTimer(CameraYawTimer);
        }

        /// <summary> Gets a proc id by name. </summary>
        /// <param name="name"> The name of the process. </param>
        /// <returns> The id of the process if it exists, otherwise returns 0. </returns>
        private int GetProcIdFromName(string name) //new 1.0.2 function
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
                        LogDateMsg("Hooking to second ROSE client.", LogTypes.System);
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
        /// <returns>True if the process was successfully hooked.</returns>
        private bool TryOpenProcess()
        {
            int pID = GetProcIdFromName("trose");

            if (pID > 0)
            {
                Globals.TargetApplicationMemory.OpenProcess(pID);
                LogDateMsg("Process ID: " + pID.ToString() + " Hooked Successfully.", LogTypes.System);
                OnFinishedHooking?.Invoke();
                Globals.Hooked = true;
                return true;
            }

            HookBtn.Content = "Hook Failed :(";
            return false;
        }

        /// <summary> Logs a message to console and to form log. </summary>
        /// <param name="msg"> The message </param>
        private void LogDateMsg(string msg, LogTypes logType)
        {
            if (IgnoredLogTypes.Contains(logType))
                return;

            Trace.WriteLine(System.DateTime.Now.ToString() + ": " + msg);
            LogMsgToFormLog(msg);
        }

        #endregion

        #region Bot Methods

        /// <summary> Prepares the bot for starting </summary>
        public void PrepareElfBot(object sender, RoutedEventArgs e)
        {
			Trace.WriteLine("test");
			//Closed += StopWindowBehavior;

			_sim = new InputSimulator();
            _openMonsterTableDialog = new Microsoft.Win32.OpenFileDialog();
            _monsterTable = new MonsterHashTable();

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
            ListenToTimer(CombatCameraTimer, CombatCameraTimer_Tick);
            ListenToTimer(CameraYawTimer, CameraYawTimer_Tick);

            StartTimer(InterfaceTimer, _interfaceUpdateTime);
            StartTimer(CombatCameraTimer, _combatCameraTickTime);
            StartTimer(CameraYawTimer, _cameraYawTickTime);

            OnFinishedHooking += FinishHook;

            SystemMsgLog.Content = "";
            AutoCombatCheckBox.IsEnabled = false;
            HpFoodCheckBox.IsEnabled = false;
            MpFoodCheckBox.IsEnabled = false;
            CombatCameraCheckBox.IsEnabled = false;
            TimedCameraYawCheckBox.IsEnabled = false;

            CombatOptionsPanel.Visibility = Visibility.Visible;

            IgnoredLogTypes.Add(LogTypes.Camera);
            IgnoredLogTypes.Add(LogTypes.Food);
            IgnoredLogTypes.Add(LogTypes.Combat);

            _openMonsterTableDialog.Filter = "Text files(*.txt)| *.txt";

            // worker threads could be useful later
            // if (!worker.IsBusy)  {  //worker.RunWorkerAsync();  }
        }

        /// <summary> Callback for when process has hooked. </summary>
        private void FinishHook()
        {
            HookBtn.Content = "Process Hooked!";
            AutoCombatCheckBox.IsEnabled = true;
            HpFoodCheckBox.IsEnabled = true;
            MpFoodCheckBox.IsEnabled = true;
            CombatCameraCheckBox.IsEnabled = true;
            TimedCameraYawCheckBox.IsEnabled = true;
        }

        /// <summary> Rebuilds the monster list from the monster hash table </summary>
        private void RebuildMonsterList()
        {
            MonsterList monsterList = new MonsterList(_monsterTable);
            monsterTableText.Content = "";

            for (int i = 0; i < monsterList.Count; i++)
            {
                monsterTableText.Content += monsterList[i] + "\n";
            }

            if (monsterList.Count == 0)
            {
                monsterTableText.Content = "Empty";
            }
        }

        /// <summary> Loads a string array of monsters into the monster hash table then rebuilds the list </summary>
        /// <param name="monsters"> The list of monsters. </param>
        private void LoadToMonsterTable(string[] monsters)
        {
            _monsterTable?.Clear();
            for (int i = 0; i < monsters.Length; i++)
            {
                if (monsters[i].Length > 0)
                {
                    LogDateMsg("Added monster to table from file: " + monsters[i], LogTypes.System);
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
                LogDateMsg("Target Defeat: " + _targetDefeatedMsg, LogTypes.Combat);
                StopTimer(AttackTimeoutTimer);
                _pressedTargetting = false;

                if (combatLootCheckbox.IsChecked == true)
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
                _sim?.Keyboard.KeyPress(_activeCombatKeys[ranSkill]);

                LogDateMsg("Attack Tick: " + _activeCombatKeys[ranSkill].ToString(), LogTypes.Combat);

                if (!AttackTimeoutTimer.IsEnabled)
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

        /// <summary> Logs a message to the form log. </summary>
        /// <param name="msg"> The message </param>
        private void LogMsgToFormLog(string msg)
        {
            if (_currentSystemLog.Length > 2048)
            {
                SystemMsgLog.Content = "";
            }

            string dateFormat = DateTime.Now.ToString("hh:mm:ss tt");

            string str = dateFormat + ":" + Environment.NewLine + msg;
            SystemMsgLog.Content += (str);
            SystemMsgLog.Content += (Environment.NewLine);
            SystemMsgLog.Content += (Environment.NewLine);
        }

        /// <summary> Hides all panels visibility and controls. </summary>
        private void HideAllPanels()
        {
            CombatOptionsPanel.Visibility = Visibility.Hidden;
            LootOptionsPanel.Visibility = Visibility.Hidden;
            FoodOptionsPanel.Visibility = Visibility.Hidden;
            MonsterTablePanel.Visibility = Visibility.Hidden;
            KeybindOptionsPanel.Visibility = Visibility.Hidden;
            LoggingOptionsPanel.Visibility = Visibility.Hidden;
        }

        /// <summary> Tries to parse a float in an input box and stores the value in the referenced float. </summary>
        /// <param name="box"> The box to try to parse the float. </param>
        /// <param name="write"> The float to store the parsed value. </param>
        private void TryFloatFromInputBox(TextBox box, ref float write, bool isPercentage = false)
        {
            float result = 0f;

            if (!float.TryParse(box.Text, out result))
            {
                box.Text = write.ToString();
                return;
            }

            if(isPercentage && (result <= 0 || result > 100))
            {
                return;
            }

            write = result;
        }

        #endregion
    }
}