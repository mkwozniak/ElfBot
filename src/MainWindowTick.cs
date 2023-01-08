using System;

namespace ElfBot;

using System.Windows;
using System.Linq;
using Timer = System.Windows.Threading.DispatcherTimer;
using Trace = System.Diagnostics.Trace;

public sealed partial class MainWindow : Window
{
    #region Timer Methods

    private void ListenToTimer(Timer timer, EventHandler del)
    {
        timer.Tick += new EventHandler(del);
    }

    private void StartTimer(Timer timer, int msDelay)
    {
        timer.Interval = new System.TimeSpan(0, 0, 0, 0, msDelay);
        timer.Start();
    }

    private void StopTimer(Timer timer)
    {
        timer.Stop();
    }

    #endregion

    #region Tick Methods

    /// <summary> Timer tick for bot interface values to update </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Interface_Tick(object? sender, EventArgs e)
    {
        // Handle logging view
        if (LoggingOptionsPanel.Visibility == Visibility.Visible)
        {
            RefreshLogs();
        }

        if (!ApplicationContext.Hooked)
        {
            return;
        }

        ApplicationContext.CharacterData.Update();
    }

    /// <summary> Refreshes the log </summary>
    private void RefreshLogs()
    {
        Level level = (Level) Enum.Parse(typeof(Level), LogLevelSelection.Text, true);
        var logEntries = Logger.Entries.Where(e => e.Level >= level).ToList();
        
        var displayedLog = SystemMsgLog.Content;
        if (displayedLog is not string)
        {
            SystemMsgLog.Content = "";
        }

        if (logEntries.Count == 0)
        {
            SystemMsgLog.Content = "";
            return;
        }

        var lines = logEntries.Select(entry =>
        {
            var date = entry.TimeStamp.ToString("hh:mm:ss tt");
            return $"({date}) {entry.Level}: {entry.Text}";
        }).ToArray();
        SystemMsgLog.Content = string.Join(Environment.NewLine, lines);
    }

    /// <summary> Timer tick for eating hp food </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HpFoodTimer_Tick(object? sender, EventArgs e)
    {
		if (Sim == null || !ApplicationContext.Hooked)
			return;

        var activeHpKeys = Settings.Keybindings
            .FindAll(kb => kb.Type is KeybindType.HpFood or KeybindType.HpInstant)
            .ToArray();
        if (activeHpKeys.Length == 0)
        {
	        Trace.WriteLine("No active HP keys!");
            Logger.Warn("No active HP keys are set", LogEntryTag.System);
	        return;
        }

        _playerHP = Addresses.Hp.GetValue();
        _playerMaxHP = Addresses.MaxHp.GetValue();

        if (_playerHP == 0 || _playerMaxHP == 0)
            return;

        float hpPercent = ((float)(_playerHP) / (float)(_playerMaxHP));

		Trace.WriteLine("Check HP Food Tick");
        Logger.Debug("Checking health...", LogEntryTag.Food);
        
        if (hpPercent < (Settings.FoodOptions.AutoHpThresholdPercent / 100) && _eatHPFood)
        {
            int ranFood = Ran.Next(0, activeHpKeys.Length);
				Trace.WriteLine("Eat HP Food Tick");
            Logger.Debug($"Health is low, eating food at slot {ranFood}", LogEntryTag.Food);
            SendKey(activeHpKeys[ranFood].KeyCode);
            _eatHPFood = false;
            // start the delay timer to press the key again
            StartTimer(HpFoodKeyTimer, (int)(Settings.FoodOptions.Cooldown * 1000));
        }
    }

    /// <summary> Timer tick for eating mp food </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MpFoodTimer_Tick(object? sender, EventArgs e)
    {
		if (Sim == null || !ApplicationContext.Hooked)
			return;

        var activeMpKeys = Settings.Keybindings
            .FindAll(kb => kb.Type is KeybindType.MpFood or KeybindType.MpInstant)
            .ToArray();

        if (activeMpKeys.Length == 0)
        {
			Trace.WriteLine("No active MP keys!");
            Logger.Warn("No active MP keys are set", LogEntryTag.System);
			return;
		}

        _playerMP = Addresses.Mp.GetValue();
        _playerMaxMP = Addresses.MaxMp.GetValue();

        if (_playerMP == 0 || _playerMaxMP == 0)
            return;

        float mpPercent = ((float)(_playerMP) / (float)(_playerMaxMP));

		Trace.WriteLine("Check MP Food Tick");
        Logger.Debug("Checking mana...", LogEntryTag.Food);

        if (mpPercent < (Settings.FoodOptions.AutoMpThresholdPercent / 100) && _eatMPFood)
        {
            int ranFood = Ran.Next(0, activeMpKeys.Length);
			Trace.WriteLine("Eat MP Food Tick");
            Logger.Debug($"Mana is low, eating food at slot {ranFood}", LogEntryTag.Food);
            SendKey(activeMpKeys[ranFood].KeyCode);
            // start the delay timer to press the key again
            StartTimer(MpFoodKeyTimer, (int)(Settings.FoodOptions.Cooldown * 1000));
            _eatMPFood = false;
        }
    }

    /// <summary> Timer tick to reset hp food key </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HpFoodKeyTimer_Tick(object? sender, EventArgs e)
    {
        _eatHPFood = true;
        StopTimer(HpFoodKeyTimer);
    }

    /// <summary> Timer tick to reset mp food key </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MpFoodKeyTimer_Tick(object? sender, EventArgs e)
    {
        _eatMPFood = true;
        StopTimer(MpFoodKeyTimer);
    }

    /// <summary> Timer tick for combat camera </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CombatCameraTimer_Tick(object? sender, EventArgs e)
    {
        if (!ApplicationContext.Hooked)
            return;

        if (Settings.CombatOptions.ForceCameraOverhead)
        {
            Addresses.CameraPitch.writeValue(CameraMaxPitch);
        }
        if (Settings.CombatOptions.ForceCameraZoom)
        {
            Addresses.CameraZoom.writeValue(CameraMaxZoom);
        }
    }

    /// <summary> Timer tick for camera yaw spin </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CameraYawTimer_Tick(object? sender, EventArgs e)
    {
		if (!ApplicationContext.Hooked || !Settings.CombatOptions.CameraYawWaveEnabled || Sim == null)
            return;

        float waveform = (float)(Math.PI * Math.Sin(0.25 * _yawCounter));
        Addresses.CameraYaw.writeValue(waveform);
        _yawCounter += 0.05;

        _rightClickCounter++;

        if (_rightClickCounter > 50)
        {
            _rightClickCounter = 0;
            Sim.Mouse.VerticalScroll(-1);
        }
    }

	/// <summary> Timer tick for timed zhack </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void ZHackTimer_Tick(object? sender, EventArgs e)
	{
		if (!ApplicationContext.Hooked)
			return;

		Logger.Debug("ZHack Timer Tick", LogEntryTag.System);
        //_sim.Keyboard.KeyPress(VirtualKeyCode.VK_W);
		Addresses.PositionZ.writeValue(Settings.ZHackOptions.Amount);
	}

    #endregion
}