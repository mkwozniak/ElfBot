using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
}