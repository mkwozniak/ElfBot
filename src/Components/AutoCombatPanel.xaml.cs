using System.Windows;

namespace ElfBot.Components;

public partial class AutoCombatPanel
{
	private ApplicationContext? ApplicationContext => TryFindResource("ApplicationContext") as ApplicationContext;

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
}