using System.Windows;

namespace ElfBot.Components;

public partial class FoodOptionsPanel
{
	private ApplicationContext ApplicationContext => (TryFindResource("ApplicationContext") as ApplicationContext)!;
	public FoodOptionsPanel()
	{
		InitializeComponent();
	}

	/// <summary>
	/// Enables auto food if either auto-hp or auto-mp are selected.
	/// </summary>
	private void EnableAutoFood(object sender, RoutedEventArgs e)
	{
		if (!ApplicationContext.AutoFood.IsStarted())
		{
			ApplicationContext.AutoFood.Start();	
		}
	}

	/// <summary>
	/// Checks to see if the auto-food timer can be disabled.
	///
	/// Since auto-food is under 1 main loop, it is disabled only if auto-hp
	/// and auto-mp are disabled.
	/// </summary>
	private void CheckDisableAutoFood(object sender, RoutedEventArgs e)
	{
		var opts = ApplicationContext.Settings.FoodOptions;
		if (!opts.AutoHpEnabled && !opts.AutoMpEnabled)
		{
			ApplicationContext.AutoFood.Stop();
		}
	}
}