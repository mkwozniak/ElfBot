using System.Diagnostics;
using System;
using System.Windows;

namespace ElfBot.Components;

public partial class AutoCombatPanel
{
	private ApplicationContext? ApplicationContext => TryFindResource("ApplicationContext") as ApplicationContext;

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
		if (!ApplicationContext.Hooked)
			return;

		int bytesWritten = 0;
		RoseProcess.WriteBytes(RoseProcess.CurrentProcessHandle,
			(IntPtr)0x7FF7454B4D70, new byte[] { 0xc3, 0x90 }, ref bytesWritten);

		Trace.WriteLine("Enabled NoClip with bytes written: " + bytesWritten);

		/*
		if(Addresses.NoClipOn.Write())
		{
			MainWindow.Logger.Debug("Successfully enabled NoClip.");
			return;
		}

		MainWindow.Logger.Debug("Could not enable NoClip.");
		*/
	}

	private void DisableNoClip(object sender, RoutedEventArgs e)
	{
		if (!ApplicationContext.Hooked)
			return;

		int bytesWritten = 0;
		RoseProcess.WriteBytes(RoseProcess.CurrentProcessHandle,
		(IntPtr)0x7FF7454B4D70, new byte[] { 0x40, 0x57 }, ref bytesWritten);

		Trace.WriteLine("Disabled NoClip with bytes written: " + bytesWritten);

		/*
		if (Addresses.NoClipOff.Write())
		{
			MainWindow.Logger.Debug("Successfully disabled NoClip.");
			return;
		}

		MainWindow.Logger.Debug("Could not disable NoClip.");
		*/
	}
}