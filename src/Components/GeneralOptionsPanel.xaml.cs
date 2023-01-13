using System;
using System.Windows;
using System.Windows.Controls;

namespace ElfBot.Components;

public partial class GeneralOptionsPanel 
{
	private ApplicationContext ApplicationContext => (TryFindResource("ApplicationContext") as ApplicationContext)!;

	public GeneralOptionsPanel()
	{
		InitializeComponent();
	}

	private void EnableNoClip(object sender, RoutedEventArgs e)
	{
		if (!ApplicationContext.Hooked) return;
		if (!RoseProcess.EnableNoClip())
		{
			MainWindow.Logger.Warn("Failed to enable no-clip, could not write to memory");
			ApplicationContext.Settings.CombatOptions.NoClip = false;
			return;
		}

		MainWindow.Logger.Info("Enabled no-clip");
	}

	private void DisableNoClip(object sender, RoutedEventArgs e)
	{
		if (!ApplicationContext.Hooked) return;
		if (!RoseProcess.DisableNoClip())
		{
			MainWindow.Logger.Warn("Failed to disable no-clip, could not write to memory");
			ApplicationContext.Settings.CombatOptions.NoClip = true;
			return;
		}

		MainWindow.Logger.Info("Disabled no-clip");
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