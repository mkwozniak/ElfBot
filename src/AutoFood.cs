using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using ElfBot.Util;
#pragma warning disable CS4014

namespace ElfBot;

/// <summary>
/// Auto-food master state. Handles timing of food consumption when
/// HP or MP falls too low (as per configuration).
/// </summary>
public sealed class AutoFood
{
	public static readonly Random Random = new();

	private readonly ApplicationContext _context;
	private readonly HotkeyCooldownTracker _cooldownTracker = new();

	private readonly DispatcherTimer _autoFoodTimer = new()
	{
		Interval = TimeSpan.FromMilliseconds(250)
	};

	private FoodOptions FoodOptions => _context.Settings.FoodOptions;

	public AutoFood(ApplicationContext context)
	{
		_context = context;
		_autoFoodTimer.Tick += Tick;
	}

	/// <summary>
	/// Starts the automation of auto-food.
	/// </summary>
	public void Start()
	{
		Trace.WriteLine("Enabling auto-food");
		_cooldownTracker.Clear();
		_autoFoodTimer.Start();
	}

	/// <summary>
	/// Returns whether the auto-food loop is running.
	/// </summary>
	/// <returns>auto-food enablement state</returns>
	public bool IsStarted()
	{
		return _autoFoodTimer.IsEnabled;
	}

	/// <summary>
	/// Stops the automation of auto-food.
	/// </summary>
	public void Stop()
	{
		Trace.WriteLine("Auto-food disabled");
		_cooldownTracker.Clear();
		FoodOptions.AutoHpEnabled = false;
		FoodOptions.AutoMpEnabled = false;
		_autoFoodTimer.Stop();
	}

	/// <summary>
	/// Master heartbeat for checking player HP and MP and applying food items.
	/// </summary>
	private void Tick(object? sender, EventArgs e)
	{
		if (_context.ActiveCharacter == null)
		{
			Trace.WriteLine("Canceled auto-food due to missing player");
			MainWindow.Logger.Warn("Auto-food disabled, please ensure that your character is signed in");
			Stop();
			return;
		}

		if (_context.ActiveCharacter.IsDead)
		{
			if (_context.Settings.GeneralOptions.DeathAction != DeathActions.CANCEL_TIMERS) return;
			Trace.WriteLine("Canceled auto-food due to player death");
			MainWindow.Logger.Warn("Disabling auto-food due to player death");
			Stop();
			return;
		}

		try
		{
			if (FoodOptions.AutoHpEnabled)
			{
				_autoHealth();
			}

			if (FoodOptions.AutoMpEnabled)
			{
				_autoMana();
			}
		}
		catch (Exception ex)
		{
			MainWindow.Logger.Error($"An exception occurred when attempting to process auto-food");
			MainWindow.Logger.Error($"Disabling auto-food");
			MainWindow.Logger.Error(ex.Message);
			if (ex.StackTrace != null)
			{
				MainWindow.Logger.Error(ex.StackTrace);
			}

			Stop();
		}
	}

	private void _autoHealth()
	{
		var hp = _context.ActiveCharacter!.Hp;
		var maxHp = _context.ActiveCharacter.MaxHp;

		if (hp == 0 || maxHp == 0)
		{
			return;
		}

		if (_shouldTrigger(hp, maxHp, FoodOptions.HpSlowFoodThresholdPercent))
		{
			_runAction(KeybindAction.HpFood);
		}

		if (_shouldTrigger(hp, maxHp, FoodOptions.HpInstantFoodThresholdPercent))
		{
			_runAction(KeybindAction.HpInstant);
		}
	}

	private void _autoMana()
	{
		var mp = _context.ActiveCharacter!.Mp;
		var maxMp = _context.ActiveCharacter.MaxMp;

		if (mp == 0 || maxMp == 0)
		{
			return;
		}

		if (_shouldTrigger(mp, maxMp, FoodOptions.MpSlowFoodThresholdPercent))
		{
			_runAction(KeybindAction.MpFood);
		}

		if (_shouldTrigger(mp, maxMp, FoodOptions.MpInstantFoodThresholdPercent))
		{
			_runAction(KeybindAction.MpInstant);
		}
	}

	private bool _runAction(KeybindAction action)
	{
		var activeActionKeys = _context.Settings.FindKeybindings(action);
		var hotkey = _findAvailableFoodHotkey(activeActionKeys);

		if (hotkey == null) return false;

		RoseProcess.SendKeypress(hotkey.KeyCode, hotkey.IsShift);
		_cooldownTracker.SetCooldown(hotkey, TimeSpan.FromSeconds(hotkey.Cooldown + 0.1f));
		Trace.WriteLine($"Consuming food for action {action} in slot {hotkey.Key} " +
		                $"by pressing keycode {hotkey.KeyCode}.");
		return true;
	}

	private HotkeySlot? _findAvailableFoodHotkey(List<HotkeySlot> slots)
	{
		if (slots.Count == 0) return null;

		var notOnCooldown = slots.FindAll(hk => !_cooldownTracker.isOnCooldown(hk));
		if (notOnCooldown.Count == 0) return null;

		// Select a random slot to use
		var randomKeyIndex = Random.Next(0, notOnCooldown.Count);
		return notOnCooldown[randomKeyIndex];
	}

	private static bool _shouldTrigger(int value, int maxValue, float threshold)
	{
		var percent = value / (float)maxValue;
		var thresholdPercent = threshold / 100;
		return percent <= thresholdPercent;
	}
}