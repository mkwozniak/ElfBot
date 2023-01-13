using System;
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
		ApplicationContext.ZHackTimer.Stop();
		ApplicationContext.ZHackTimer.Interval = TimeSpan.FromSeconds(ApplicationContext.Settings.ZHackOptions.Frequency);
		ApplicationContext.ZHackTimer.Start();
		MainWindow.Logger.Info("Enabled Timed ZHack", LogEntryTag.Combat);
	}

	private void DisableZHack(object sender, RoutedEventArgs e)
	{
		ApplicationContext.ZHackTimer.Stop();
		MainWindow.Logger.Info("Disabled Timed ZHack", LogEntryTag.Combat);
	}
}