using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ElfBot.Util;

namespace ElfBot;

public static class RoseProcess
{
	public static Process? HookedProcess { get; set; }

	[DllImport("kernel32.dll")]
	public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
		byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

	/// <summary>
	/// Gets the ROSE process object.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="dualClient"></param>
	/// <returns></returns>
	public static Process? GetProcess(bool dualClient = false)
	{
		var processes = Process.GetProcessesByName("trose");
		if (!dualClient) return processes.Length > 0 ? processes[0] : null;
		return processes.Length > 1 ? processes[1] : null;
	}

	/// <summary>
	/// Sends a key press to the ROSE application.
	/// </summary>
	/// <param name="code">key code to send</param>
	/// <param name="shift">whether to use the key press with shift</param>
	/// <returns>whether the keypress was sent successfully</returns>
	public static bool SendKeypress(Messaging.VKeys code, bool shift = false)
	{
		if (HookedProcess == null) return false;
		var shiftType = shift ? Messaging.ShiftType.SHIFT : Messaging.ShiftType.NONE;
		var shiftKey = shift ? Messaging.VKeys.KEY_SHIFT : Messaging.VKeys.NULL;
		var key = new Key(code, shiftKey, shiftType);
		return key.PressBackground(HookedProcess.MainWindowHandle);
	}

	public static bool EnableNoClip()
	{
		return _writeNoClip(new byte[] { 0xC3, 0x90 });
	}

	public static bool DisableNoClip()
	{
		return _writeNoClip(new byte[] { 0x40, 0x57 });
	}

	private static bool _writeNoClip(byte[] bytes)
	{
		if (HookedProcess?.MainModule == null) return false;
		if (bytes.Length != 2)
		{
			throw new ArgumentException("Bytes array must have a length of 2");
		}

		var baseAddress = HookedProcess.MainModule.BaseAddress;
		var withOffset = baseAddress + StaticOffsets.NoClipFunction;
		var bytesWritten = 0;
		try
		{
			WriteProcessMemory(HookedProcess.Handle, withOffset,
				bytes, bytes.Length, ref bytesWritten);
			return bytesWritten > 0;
		}
		catch (Exception ex)
		{
			MainWindow.Logger.Error($"An exception occurred when attempting enable no-clip");
			MainWindow.Logger.Error(ex.Message);
			if (ex.StackTrace != null)
			{
				MainWindow.Logger.Error(ex.StackTrace);
			}

			return false;
		}
	}
}