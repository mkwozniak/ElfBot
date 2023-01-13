using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ElfBot;

public static class RoseProcess
{
	public static Process? HookedProcess { get; set; }

	[DllImport("ROSE_Input.dll")]
	public static extern void SendKey(int key);

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