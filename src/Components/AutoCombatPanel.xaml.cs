using System;
using System.Windows;

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

	private void EnableNoClip(object sender, RoutedEventArgs e)
	{
		if (!ApplicationContext.Hooked) return;

		if (!_writeNoClip(new byte[] { 0xC3, 0x90 }))
		{
			MainWindow.Logger.Warn("Failed to enable no-clip, could not write to memory");
			ApplicationContext.Settings.CombatOptions.NoClip = false;
			return;
		}

		MainWindow.Logger.Info("Enable no-clip");
	}

	private void DisableNoClip(object sender, RoutedEventArgs e)
	{
		if (!ApplicationContext.Hooked) return;

		if (!_writeNoClip(new byte[] { 0x40, 0x57 }))
		{
			MainWindow.Logger.Warn("Failed to disable no-clip, could not write to memory");
			ApplicationContext.Settings.CombatOptions.NoClip = true;
			return;
		}

		MainWindow.Logger.Info("Disabled no-clip");
	}

	private bool _writeNoClip(byte[] bytes)
	{
		if (RoseProcess.HookedProcess == null
		    || RoseProcess.HookedProcess.MainModule == null) return false;
		if (bytes.Length != 2)
		{
			throw new ArgumentException("Bytes array must have a length of 2");
		}

		var baseAddress = RoseProcess.HookedProcess.MainModule.BaseAddress;
		var withOffset = baseAddress + StaticOffsets.NoClipFunction;
		var bytesWritten = 0;
		try
		{
			RoseProcess.WriteProcessMemory(RoseProcess.HookedProcess.Handle, withOffset,
				bytes, bytes.Length, ref bytesWritten);
			return bytesWritten > 0;
		}
		catch (Exception ex)
		{
			MainWindow.Logger.Error($"An exception occurred when attempting write no clip bytes {bytes}");
			MainWindow.Logger.Error(ex.Message);
			if (ex.StackTrace != null)
			{
				MainWindow.Logger.Error(ex.StackTrace);
			}

			return false;
		}
	}
}