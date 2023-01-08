using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ElfBot;

public static class RoseProcess
{
	[DllImport("user32.dll")]
	private static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll", SetLastError = true)]
	static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

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
					continue;
				}

				if (dualClient && foundClient)
				{
					mainID = theprocess.Id;
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

	public static Process? getForegroundProcess()
	{
		var hWnd = GetForegroundWindow(); 
		GetWindowThreadProcessId(hWnd, out var processId);
		return Process.GetProcessById(Convert.ToInt32(processId)); 
	}
}