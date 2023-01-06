using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using WindowsInput.Native;

namespace ElfBot;

public abstract class PropertyNotifyingClass : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}

public class ApplicationContext : PropertyNotifyingClass
{
	private Settings _settings = new()
	{
		Keybindings = new List<Keybinding>
		{
			new() { Key = "1", Value = 0 },
			new() { Key = "2", Value = 0 },
			new() { Key = "3", Value = 0 },
			new() { Key = "4", Value = 0 },
			new() { Key = "5", Value = 0 },
			new() { Key = "6", Value = 0 },
			new() { Key = "7", Value = 0 },
			new() { Key = "8", Value = 0 },
			new() { Key = "9", Value = 0 },
			new() { Key = "0", Value = 0 },
			new() { Key = "-", Value = 0 },
			new() { Key = "=", Value = 0 }
		}
	};

	public Settings Settings
	{
		get => _settings;
		set
		{
			_settings = value;
			NotifyPropertyChanged();
		}
	}
}

public class Settings : PropertyNotifyingClass
{
	private List<Keybinding> _keybindings;

	public List<Keybinding> Keybindings
	{
		get => _keybindings;
		set
		{
			_keybindings = value;
			NotifyPropertyChanged();
		}
	}
}

[JsonObject(MemberSerialization.OptIn)]
public class Keybinding
{
	[JsonProperty] public string Key { get; init; } = default!;
	public VirtualKeyCode KeyCode => MainWindow.KeyMap[Key];

	[JsonProperty] public int Value { get; set; }
	public KeybindType Type => (KeybindType)Value;
}