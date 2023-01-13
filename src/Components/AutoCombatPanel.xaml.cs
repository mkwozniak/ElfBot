using System.Windows;

namespace ElfBot.Components;

public partial class AutoCombatPanel
{
	private ApplicationContext ApplicationContext => (TryFindResource("ApplicationContext") as ApplicationContext)!;

	public AutoCombatPanel()
	{
		InitializeComponent();
	}

	private void StartAutoCombat(object sender, RoutedEventArgs e)
	{
		ApplicationContext.AutoCombat.Start();
	}

	private void StopAutoCombat(object sender, RoutedEventArgs e)
	{
		ApplicationContext.AutoCombat.Stop();
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
}