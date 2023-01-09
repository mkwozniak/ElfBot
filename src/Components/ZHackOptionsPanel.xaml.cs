using System.Windows;

namespace ElfBot.Components;

public partial class ZHackOptionsPanel
{
	private ApplicationContext? ApplicationContext => TryFindResource("ApplicationContext") as ApplicationContext;

	public ZHackOptionsPanel()
	{
		InitializeComponent();
	}

	private void EnableZHack(object sender, RoutedEventArgs e)
	{
		MainWindow.StopTimer(ApplicationContext.ZHackTimer);
		MainWindow.StartTimer(ApplicationContext.ZHackTimer,
			(int)(ApplicationContext.Settings.ZHackOptions.Frequency * 1000));
		MainWindow.Logger.Info("Enabled Timed ZHack", LogEntryTag.Combat);
	}

	private void DisableZHack(object sender, RoutedEventArgs e)
	{
		MainWindow.StopTimer(ApplicationContext.ZHackTimer);
		MainWindow.Logger.Info("Disabled Timed ZHack", LogEntryTag.Combat);
	}
}