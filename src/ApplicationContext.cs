using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace ElfBot;

/// <summary>
/// Delegate method to call once the ROSE process unhooks.
/// </summary>
public delegate void OnRoseUnhook();

public class ApplicationContext : PropertyNotifyingClass
{
	private Settings _settings = new()
	{
		Keybindings = new List<HotkeySlot>
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
		},
		ShiftKeybindings = new List<HotkeySlot>
		{
			new() { Key = "1", Value = 0, IsShift = true },
			new() { Key = "2", Value = 0, IsShift = true },
			new() { Key = "3", Value = 0, IsShift = true },
			new() { Key = "4", Value = 0, IsShift = true },
			new() { Key = "5", Value = 0, IsShift = true },
			new() { Key = "6", Value = 0, IsShift = true },
			new() { Key = "7", Value = 0, IsShift = true },
			new() { Key = "8", Value = 0, IsShift = true },
			new() { Key = "9", Value = 0, IsShift = true },
			new() { Key = "0", Value = 0, IsShift = true },
			new() { Key = "-", Value = 0, IsShift = true },
			new() { Key = "=", Value = 0, IsShift = true }
		}
	};

	private bool _hooked;
	private Character _hookedCharacterMemRef = new();
	public OnRoseUnhook? RoseUnhookDelegate { get; set; }

	public DispatcherTimer CombatCameraTimer = new()
	{
		Interval = TimeSpan.FromMilliseconds(500)
	};

	public DispatcherTimer CameraYawTimer = new()
	{
		Interval = TimeSpan.FromMilliseconds(50)
	};
	public DispatcherTimer ZHackTimer = new();

	public readonly HashSet<string> MonsterTable = new();

	public ApplicationContext()
	{
		AutoCombat = new AutoCombat(this);
		AutoFood = new AutoFood(this);
		UiData = new UiData(this);
	}

	public Settings Settings
	{
		get => _settings;
		set
		{
			_settings = value;
			NotifyPropertyChanged();
		}
	}

	public Character? ActiveCharacter => Hooked && _hookedCharacterMemRef.IsValid() ? _hookedCharacterMemRef : null;

	public UiData UiData { get; }

	public AutoCombat AutoCombat { get; }
	
	public AutoFood AutoFood { get; }

	public bool Hooked
	{
		get
		{
			if (RoseProcess.HookedProcess == null) return false;
			// ReSharper disable once InvertIf
			if (RoseProcess.HookedProcess.HasExited)
			{
				RoseProcess.HookedProcess = null;
				Hooked = false;
				return false;
			}
			return true;
		}
		set
		{
			if (_hooked == value) return;
			_hooked = value;
			if (!value && RoseUnhookDelegate != null) 
				RoseUnhookDelegate.Invoke();
			NotifyPropertyChanged();
		}
	}

	public bool UseSecondClient { get; set; }
}