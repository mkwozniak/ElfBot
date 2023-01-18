using System.Windows;

namespace ElfBot.Components;

public partial class AutoClericPanel
{
	private ApplicationContext ApplicationContext => (TryFindResource("ApplicationContext") as ApplicationContext)!;

	public AutoClericPanel()
	{
		InitializeComponent();
	}

	private void StartAutoCleric(object sender, RoutedEventArgs e)
	{
		ApplicationContext.AutoCleric.Start();
	}

	private void StopAutoCleric(object sender, RoutedEventArgs e)
	{
		ApplicationContext.AutoCleric.Stop();
	}
}