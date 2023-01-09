using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace ElfBot.Components;

public partial class MonsterTablePanel
{
	private ApplicationContext? ApplicationContext => TryFindResource("ApplicationContext") as ApplicationContext;

	public MonsterTablePanel()
	{
		InitializeComponent();
	}

	/// <summary> Loads a monster table </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void LoadMonsterTable(object sender, RoutedEventArgs e)
	{
		var dialog = new OpenFileDialog
		{
			Filter = "Plain Text (*.txt)|*.txt"
		};
		if (dialog.ShowDialog() is not true) return;

		try
		{
			using var reader = new StreamReader(dialog.FileName);
			var contents = reader.ReadToEnd();
			var monsters = contents.Split(',');

			ApplicationContext.MonsterTable.Clear();
			ApplicationContext.MonsterTable.UnionWith(monsters);

			MonsterTableText.Content = monsters.Length == 0 ? "Empty" : string.Join("\n", monsters);
		}
		catch (System.Security.SecurityException ex)
		{
			MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
			                $"Details:\n\n{ex.StackTrace}");
		}
	}
}