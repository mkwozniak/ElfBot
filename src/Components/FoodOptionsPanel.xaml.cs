using System.Windows;

namespace ElfBot.Components;

public partial class FoodOptionsPanel
{
	private ApplicationContext? ApplicationContext => TryFindResource("ApplicationContext") as ApplicationContext;

	public FoodOptionsPanel()
	{
		InitializeComponent();
	}

	private void EnableHpTimer(object sender, RoutedEventArgs e)
	{
		MainWindow.StopTimer(ApplicationContext.HpFoodTimer); // just in case it's already running
		MainWindow.StartTimer(ApplicationContext.HpFoodTimer,
			(int)(ApplicationContext.Settings.FoodOptions.CheckFrequency * 1000));
		MainWindow.Logger.Info("Enabled auto-HP food consumption", LogEntryTag.Food);
	}

	private void DisableHpTimer(object sender, RoutedEventArgs e)
	{
		MainWindow.StopTimer(ApplicationContext.HpFoodTimer);
		MainWindow.Logger.Info("Disabled auto-HP food consumption", LogEntryTag.Food);
	}

	private void EnableMpTimer(object sender, RoutedEventArgs e)
	{
		MainWindow.StopTimer(ApplicationContext.MpFoodTimer); // just in case it's already running
		MainWindow.StartTimer(ApplicationContext.MpFoodTimer,
			(int)(ApplicationContext.Settings.FoodOptions.CheckFrequency * 1000));
		MainWindow.Logger.Info("Enabled auto-MP food consumption", LogEntryTag.Food);
	}

	private void DisableMpTimer(object sender, RoutedEventArgs e)
	{
		MainWindow.StopTimer(ApplicationContext.MpFoodTimer);
		MainWindow.Logger.Info("Disabled auto-MP food consumption", LogEntryTag.Food);
	}
}