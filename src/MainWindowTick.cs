using System;

namespace ElfBot;

using System.Windows;
using System.Linq;
using Timer = System.Windows.Threading.DispatcherTimer;

public sealed partial class MainWindow : Window
{
    #region Timer Methods

    public static void ListenToTimer(Timer timer, EventHandler del)
    {
        timer.Tick += new EventHandler(del);
    }

    public static void StartTimer(Timer timer, int msDelay)
    {
        timer.Interval = new System.TimeSpan(0, 0, 0, 0, msDelay);
        timer.Start();
    }

    public static void StopTimer(Timer timer)
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
        var logEntries = Logger.Entries.Where(e => (int)e.Level >= (int)Settings.SelectedLogLevel).ToList();
        
        var displayedLog = LogsViewPanel.SystemMsgLog.Text;
        if (displayedLog is not string)
        {
            LogsViewPanel.SystemMsgLog.Text = "";
        }

        if (logEntries.Count == 0)
        {
            LogsViewPanel.SystemMsgLog.Text = "";
            return;
        }

        var lines = logEntries.Select(entry =>
        {
            var date = entry.TimeStamp.ToString("hh:mm:ss tt");
            return $"({date}) {entry.Level}: {entry.Text}";
        }).ToArray();
        LogsViewPanel.SystemMsgLog.Text = string.Join(Environment.NewLine, lines);
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
		if (!ApplicationContext.Hooked || !Settings.CombatOptions.CameraYawWaveEnabled)
            return;

        float waveform = (float)(Math.PI * Math.Sin(0.25 * _yawCounter));
        Addresses.CameraYaw.writeValue(waveform);
        _yawCounter += _yawCounterIncrement;

        _yawMouseScrollCounter++;

        if (_yawMouseScrollCounter > _yawMouseScrollCounterMax)
        {
            _yawMouseScrollCounter = 0;
            Addresses.CameraZoom.writeValue(Addresses.CameraZoom.GetValue() + 10f);
            // Sim.Mouse.VerticalScroll(-1);
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
		Addresses.PositionZ.writeValue(Addresses.PositionZ.GetValue() + Settings.ZHackOptions.Amount);
	}

    #endregion
}