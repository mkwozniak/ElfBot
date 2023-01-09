using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ElfBot;

public class Settings : PropertyNotifyingClass
{
	private List<HotkeySlot> _keybindings = default!;
	private List<HotkeySlot> _shiftKeybindings = default!;
	public CombatOptions CombatOptions { get; } = new();
	public LootOptions LootOptions { get; } = new();
	public FoodOptions FoodOptions { get; } = new();
	public ZHackOptions ZHackOptions { get; } = new();

	public List<HotkeySlot> Keybindings
	{
		get => _keybindings;
		set
		{
			if (value.Count == 0) return; // likely from a config load
			_keybindings = value;
			NotifyPropertyChanged();
		}
	}

	public List<HotkeySlot> ShiftKeybindings
	{
		get => _shiftKeybindings;
		set
		{
			if (value.Count == 0) return; // likely from a config load
			_shiftKeybindings = value;
			NotifyPropertyChanged();
		}
	}

	[JsonIgnore] public Level SelectedLogLevel = Level.Info;

	/// <summary>
	/// Finds all keybindings for a specified keybind action.
	///
	/// The returned type specifies the key code to press as well as whether
	/// it is on the shift hotbar. 
	/// </summary>
	/// <param name="types">Types of actions to query for</param>
	/// <returns>List of matching keybinds, empty if nothing found</returns>
	public List<HotkeySlot> FindKeybindings(params KeybindAction[] types)
	{
		List<HotkeySlot> matching = new();
		matching.AddRange(Keybindings.FindAll(kb => types.Contains(kb.Action)));
		matching.AddRange(ShiftKeybindings.FindAll(kb => types.Contains(kb.Action)));
		return matching;
	}
}

public sealed class CombatOptions : PropertyNotifyingClass
{
	private bool _autoCombatEnabled;

	[JsonIgnore]
	public bool AutoCombatEnabled
	{
		get => _autoCombatEnabled;
		set
		{
			if (_autoCombatEnabled == value) return;
			_autoCombatEnabled = value;
			NotifyPropertyChanged();
		}
	}

	public float AttackTimeout { get; set; } = 60f;
	public bool ForceCameraZoom { get; set; }
	public bool ForceCameraOverhead { get; set; }
	public bool CameraYawWaveEnabled { get; set; } // moves camera in a circle
	public bool PriorityTargetScan { get; set; } = true;
	public int MaxPriorityChecks { get; set; } = 10;
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
public class HotkeySlot
{
	[JsonProperty] public string Key { get; init; } = default!;
	public int KeyCode => MainWindow.KeyMap[Key];

	[JsonProperty] public int Value { get; set; }
	public KeybindAction Action => (KeybindAction)Value;
	
	[JsonProperty] public bool IsShift { get; init; }
	
	/// <summary>
	/// The cooldown time, in seconds.
	/// </summary>
	[JsonProperty] public int Cooldown { get; set; }
}