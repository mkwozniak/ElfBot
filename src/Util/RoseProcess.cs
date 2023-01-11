using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ElfBot;

public static class RoseProcess
{
	public static IntPtr CurrentProcessHandle;

	[DllImport("user32.dll")]
	private static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll", SetLastError = true)]
	static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

	[DllImport("kernel32.dll")]
	public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, 
		byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

	/// <summary> Gets a proc id by name. </summary>
	/// <param name="name"> The name of the process. </param>
	/// <returns> The id of the process if it exists, otherwise returns 0. </returns>
	public static int GetProcIdFromName(string name, bool dualClient = false)
	{
		Process[] processlist = Process.GetProcesses();

		if (name.ToLower().Contains(".exe"))
			name = name.Replace(".exe", "");
		if (name.ToLower().Contains(".bin")) // test
			name = name.Replace(".bin", "");

		bool foundClient = false;
		int mainID = 0;

		foreach (Process theprocess in processlist)
		{
			if (theprocess.ProcessName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
			{
				if (!foundClient)
				{
					foundClient = true;
					mainID = theprocess.Id;
					CurrentProcessHandle = theprocess.Handle;
					continue;
				}

				if (dualClient && foundClient)
				{
					mainID = theprocess.Id;
					CurrentProcessHandle = theprocess.Handle;
					MainWindow.Logger.Debug($"Hooking to second ROSE client with PID {mainID}", LogEntryTag.System);
					return mainID;
				}
			}
		}

		if (foundClient)
		{
			return mainID;
		}

		// failed to find process
		return mainID;
	}

	public static Process? GetForegroundProcess()
	{
		var hWnd = GetForegroundWindow(); 
		GetWindowThreadProcessId(hWnd, out var processId);
		return Process.GetProcessById(Convert.ToInt32(processId)); 
	}

	/// <summary> Writes an array of bytes at a specified address in the given process. </summary>
	/// <param name="processHandle"> The process handle. </param>
	/// <param name="MemoryAddress"> The int memory address to write to. </param>
	/// <param name="bytesToWrite"> The bytes to write into the address. </param>
	/// <param name="bytesWritten"> Stores the amount of bytes written after the operation. </param>
	public static void WriteBytes(IntPtr processHandle, IntPtr MemoryAddress, byte[] bytesToWrite, ref int bytesWritten)
	{
		WriteProcessMemory(processHandle, MemoryAddress, bytesToWrite, (int)bytesToWrite.Length, ref bytesWritten);
	}
}