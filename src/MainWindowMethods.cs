namespace ElfBot;

using System.Windows;
using WindowsInput;
using MonsterList = System.Collections.Generic.List<string>;
using MonsterHashTable = System.Collections.Generic.HashSet<string>;
using EventArgs = System.EventArgs;

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

    /// <summary> Tries to open and hook to rose online process. </summary>
    /// <returns>True if the process was successfully hooked.</returns>
    private bool TryOpenProcess()
    {
        int pID = RoseProcess.GetProcIdFromName("trose", ApplicationContext.UseSecondClient);

        if (pID > 0)
        {
            Globals.TargetApplicationMemory.OpenProcess(pID);
            Globals.Logger.Info($"Successfully hooked ROSE process with PID {pID}", LogEntryTag.System);
            OnFinishHook?.Invoke();
            ApplicationContext.Hooked = true;
            return true;
        }
        
        Globals.Logger.Warn($"Process PID {pID} was invalid and could not be hooked", LogEntryTag.System);
        HookBtn.Content = "Hook Failed :(";
        return false;
    }

	#endregion

	#region Bot Methods

	/// <summary> Prepares the bot for starting </summary>
	public void PrepareElfBot(object sender, RoutedEventArgs e)
    {
		_sim = new InputSimulator();

        _openMonsterTableDialog = new Microsoft.Win32.OpenFileDialog();
		_openMonsterTableDialog.Filter = "Text files(*.txt)| *.txt";

		_monsterTable = new MonsterHashTable();

        // listen to hook event
        OnFinishHook += FinishHook;

        // prepare the initial interface view
        PrepareElfbotInterface();

		// prepare timers
		PrepareElfbotTimers();

		// worker threads could be useful later
		// if (!worker.IsBusy)  {  //worker.RunWorkerAsync();  }
	}

    /// <summary> Prepares all essential timers then starts background timers </summary>
    private void PrepareElfbotTimers()
    {
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
        ListenToTimer(ZHackTimer, ZHackTimer_Tick);

		StartTimer(InterfaceTimer, _interfaceUpdateTime);
		StartTimer(CombatCameraTimer, _combatCameraTickTime);
		StartTimer(CameraYawTimer, _cameraYawTickTime);
	}

    /// <summary> Prepares values for default interface view </summary>
    private void PrepareElfbotInterface()
    {
		SystemMsgLog.Content = "";
		CombatOptionsPanel.Visibility = Visibility.Visible;
	}

    /// <summary> Callback for when process has hooked. </summary>
	private void FinishHook()
    {
        HookBtn.Content = "Process Hooked!";
    }

    /// <summary> Hides all panels visibility and controls. </summary>
    private void HideAllPanels()
    {
	    CombatOptionsPanel.Visibility = Visibility.Hidden;
	    FoodOptionsPanel.Visibility = Visibility.Hidden;
	    MonsterTablePanel.Visibility = Visibility.Hidden;
	    KeybindOptionsPanel.Visibility = Visibility.Hidden;
	    LoggingOptionsPanel.Visibility = Visibility.Hidden;
	    ZHackOptionsPanel.Visibility = Visibility.Hidden;
    }

    /// <summary> Rebuilds the monster list from the monster hash table </summary>
	private void RebuildMonsterList()
    {
        if (_monsterTable == null)
            return;

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
		if (_monsterTable == null)
			return;

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
            Globals.Logger.Debug($"Defeated target and received message \"{_targetDefeatedMsg}\"", LogEntryTag.Combat);
            StopTimer(AttackTimeoutTimer);
            _pressedTargetting = false;

            if (CombatLootCheckbox.IsChecked == true)
            {
                // enemy has died, loot now and start the loot timer
                _combatState = CombatStates.Looting;
                StopTimer(CombatTimer);
                // start the looting timer for hotkey
                StartTimer(LootingTimer, (int)(Settings.CombatOptions.ActionTimerDelay * 1000));
                // start the timer to end that 
                StartTimer(LootingEndTimer, (int)(Settings.LootOptions.Duration * 1000));
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
	    var activeCombatKeys = Settings.Keybindings
		    .FindAll(kb => kb.Type is KeybindType.Attack or KeybindType.Skill)
		    .ToArray();

	    if (activeCombatKeys.Length == 0)
	    {
		    Globals.Logger.Warn("Tried to attack, but no keys are set");
		    return;
	    }
	    
        // if target uid is not 0 and there are combat keys and no defeat message 
        if (_currentTargetUID != 0 && _targetDefeatedMsg.Length == 0)
        {
            // choose random attack and press key
            int ranSkill = _ran.Next(0, activeCombatKeys.Length);
            _sim?.Keyboard.KeyPress(activeCombatKeys[ranSkill].KeyCode);

            Globals.Logger.Debug($"Attack tick: {activeCombatKeys[ranSkill]}", LogEntryTag.Combat);

            if (!AttackTimeoutTimer.IsEnabled)
            {
                // start timeout timer incase this target gets the bot stuck
                StartTimer(AttackTimeoutTimer, (int)(Settings.CombatOptions.RetargetTimeout * 1000));
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
        StartTimer(TargettingTimer, (int)(Settings.CombatOptions.ActionTimerDelay * 1000));
    }

    #endregion
}