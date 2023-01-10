using System;
using System.Collections.Generic;

namespace ElfBot.Util;

public sealed class Cooldown
{
	private readonly DateTime _value;

	public Cooldown(DateTime value)
	{
		_value = value;
	}

	public Cooldown(TimeSpan duration) : this(DateTime.Now.Add(duration))
	{
	}

	/// <summary>
	/// Returns true if the current state is on a cooldown.
	/// </summary>
	/// <returns>cooldown state</returns>
	public bool isOnCooldown()
	{
		return !_isDateInPast(_value);
	}

	private static bool _isDateInPast(DateTime? time)
	{
		var now = DateTime.Now;
		return time != null && now.CompareTo(time) > 0;
	}
}

public abstract class CooldownTracker<T>
{
	protected readonly List<(T, Cooldown)> Cooldowns = new(); // List of cooldown tracking

	/// <summary>
	/// Marks an item as being on cooldown for a specified duration.
	/// </summary>
	/// <param name="item">value to track a cooldown for</param>
	/// <param name="duration">cooldown duration</param>
	public void SetCooldown(T item, TimeSpan duration)
	{
		var currentCooldown = GetCooldown(item);
		if (currentCooldown != null)
		{
			Cooldowns.Remove(currentCooldown.Value);
		}

		Cooldowns.Add((item, new Cooldown(duration)));
	}

	/// <summary>
	/// Returns true if an item is currently on cooldown.
	/// </summary>
	/// <param name="item">item to check</param>
	/// <returns>hotkey cooldown status</returns>
	public bool isOnCooldown(T item)
	{
		var cooldown = GetCooldown(item);
		return cooldown != null && cooldown.Value.Item2.isOnCooldown();
	}

	/// <summary>
	/// Clears all tracked cooldowns.
	/// </summary>
	public void Clear()
	{
		Cooldowns.Clear();
	}

	protected abstract (T, Cooldown)? GetCooldown(T slot);
}

/// <summary>
/// Tracks hotkey cooldowns.
/// </summary>
public sealed class HotkeyCooldownTracker : CooldownTracker<HotkeySlot>
{
	protected override (HotkeySlot, Cooldown)? GetCooldown(HotkeySlot slot)
	{
		var index = Cooldowns.FindIndex(i => i.Item1.Key == slot.Key
		                                     && i.Item1.IsShift == slot.IsShift);
		return index < 0 ? null : Cooldowns[index];
	}
}