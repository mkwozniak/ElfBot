using System;

namespace ElfBot;

using System.Windows;

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
	    StopTimer(InterfaceTimer);
        StopTimer(ApplicationContext.HpFoodTimer);
        StopTimer(ApplicationContext.MpFoodTimer);
        StopTimer(ApplicationContext.HpFoodKeyTimer);
        StopTimer(ApplicationContext.MpFoodKeyTimer);
        StopTimer(ApplicationContext.CombatCameraTimer);
        StopTimer(ApplicationContext.CameraYawTimer);
    }

    #endregion

	#region Bot Methods

	/// <summary> Prepares the bot for starting </summary>
	public void PrepareElfBot(object sender, RoutedEventArgs e)
    {

	    // listen auto combat send key event
		ApplicationContext.AutoCombat.OnSendKey += SendKey; 

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
	    ListenToTimer(InterfaceTimer, Interface_Tick);
		ListenToTimer(ApplicationContext.HpFoodTimer, HpFoodTimer_Tick);
		ListenToTimer(ApplicationContext.MpFoodTimer, MpFoodTimer_Tick);
		ListenToTimer(ApplicationContext.HpFoodKeyTimer, HpFoodKeyTimer_Tick);
		ListenToTimer(ApplicationContext.MpFoodKeyTimer, MpFoodKeyTimer_Tick);
		ListenToTimer(ApplicationContext.CombatCameraTimer, CombatCameraTimer_Tick);
		ListenToTimer(ApplicationContext.CameraYawTimer, CameraYawTimer_Tick);
        ListenToTimer(ApplicationContext.ZHackTimer, ZHackTimer_Tick);

		StartTimer(InterfaceTimer, _interfaceUpdateTime);
		StartTimer(ApplicationContext.CombatCameraTimer, _combatCameraTickTime);
		StartTimer(ApplicationContext.CameraYawTimer, _cameraYawTickTime);
	}

    /// <summary> Prepares values for default interface view </summary>
    private void PrepareElfbotInterface()
    {
	    CombatOptionsPanel.Visibility = Visibility.Visible;
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

    #endregion
}