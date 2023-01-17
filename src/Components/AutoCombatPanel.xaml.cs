using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

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

	/// <summary>Saves the monster table to file</summary>
	private void SaveMonsterTable(object sender, RoutedEventArgs e)
	{
		var dialog = new SaveFileDialog()
		{
			Filter = "Plain Text (*.txt)|*.txt",
			FileName = "MonsterTable.txt"
		};

		if (dialog.ShowDialog() == true)
		{
			var sb = new StringBuilder();
			foreach (var entry in ApplicationContext.MonsterTable)
			{
				if (entry.Priority) sb.Append('*');
				sb.Append(entry.Name).AppendLine();
			}

			File.WriteAllText(dialog.FileName, sb.ToString());
		}
		else
		{
			MainWindow.Logger.Warn("Failed to show save monster table dialog");
		}
	}

	/// <summary>Loads a monster table</summary>
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
			var monsters = contents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

			foreach (var line in monsters)
			{
				var split = line.Split(','); // Backwards compatibility with single-line files
				foreach (var monster in split)
				{
					var name = monster.Trim();
					if (name.Length == 0) continue;
					var priority = false;
					if (monster.StartsWith('*'))
					{
						priority = true;
						name = monster.TrimStart('*');
					}

					var item = ApplicationContext.MonsterTable.SingleOrDefault(v => v.Name == name);
					if (item != null) continue;

					ApplicationContext.MonsterTable.Add(new MonsterTableEntry()
					{
						Name = name,
						Priority = priority
					});
				}
			}
		}
		catch (System.Security.SecurityException ex)
		{
			MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
			                $"Details:\n\n{ex.StackTrace}");
		}
	}

	private void DeleteMonsterTableEntry(object sender, RoutedEventArgs e)
	{
		var button = sender as Button;
		var item = ApplicationContext.MonsterTable.SingleOrDefault(v => v.Name == (string)button!.Tag);
		if (item != null)
		{
			ApplicationContext.MonsterTable.Remove(item);
		}
	}

	private void AddNewMonsterTableEntry(object sender, RoutedEventArgs e)
	{
		var name = NewMonsterTableEntryText.Text.Trim();
		var item = ApplicationContext.MonsterTable.SingleOrDefault(v => v.Name == name);
		if (item == null)
		{
			ApplicationContext.MonsterTable.Add(new MonsterTableEntry()
			{
				Name = name,
				Priority = false
			});
		}
		NewMonsterTableEntryText.Clear();
	}
}