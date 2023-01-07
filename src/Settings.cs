﻿using System.Collections.Generic;
using Newtonsoft.Json;
using WindowsInput.Native;

namespace ElfBot;

public class Settings : PropertyNotifyingClass
{
	private List<Keybinding> _keybindings = default!;
	public CombatOptions CombatOptions { get; } = new();
	public LootOptions LootOptions { get; } = new();
	public FoodOptions FoodOptions { get; } = new();
	public ZHackOptions ZHackOptions { get; } = new();

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

public sealed class CombatOptions
{
	[JsonIgnore]
	public bool AutoCombatEnabled { get; set; }
	public float CombatKeyDelay { get; set; } = 1f;
	public float AttackTimeout { get; set; } = 60f;
	public bool ForceCameraZoom { get; set; }
	public bool ForceCameraOverhead { get; set; }
	public bool CameraYawWaveEnabled { get; set; } // moves camera in a circle
}

public sealed class LootOptions
{
	public float Duration { get; set; } = 1f;
	public bool LootAfterCombatEnabled { get; set; }
}

public sealed class FoodOptions
{
	public bool AutoHpEnabled { get; set; }
	public float AutoHpThresholdPercent { get; set; } = 50f;
	public bool AutoMpEnabled { get; set; }
	public float AutoMpThresholdPercent { get; set; } = 50f;
	public float CheckFrequency { get; set; } = 1f;
	public float Cooldown { get; set; } = 7f; 
}

public sealed class ZHackOptions
{
	public bool Enabled { get; set; }
	public float Frequency { get; set; } = 5f; 
	public float Amount { get; set; } = 7f;
}

[JsonObject(MemberSerialization.OptIn)]
public class Keybinding
{
	[JsonProperty] public string Key { get; init; } = default!;
	public VirtualKeyCode KeyCode => MainWindow.KeyMap[Key];

	[JsonProperty] public int Value { get; set; }
	public KeybindType Type => (KeybindType)Value;
}