namespace ElfBot;

using Process = System.Diagnostics.Process;
using StringComparison = System.StringComparison;
using IntPtr = System.IntPtr;
using Int32 = System.Int32;

public static class RoseProcess
{
	// Externals 
	[System.Runtime.InteropServices.DllImport("user32.dll")]
	private static extern IntPtr GetForegroundWindow();

	[System.Runtime.InteropServices.DllImport("user32.dll")]
	private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

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

	/// <summary> Returns the name of the process owning the foreground window. </summary>
	/// <returns></returns>
	public static string GetForegroundProcessName()
	{
		IntPtr hwnd = GetForegroundWindow();

		// The foreground window can be NULL in certain circumstances, 
		// such as when a window is losing activation.
		if (hwnd == null)
			return "Unknown";

		uint pid;
		GetWindowThreadProcessId(hwnd, out pid);

		foreach (Process p in Process.GetProcesses())
		{
			if (p.Id == pid)
				return p.ProcessName;
		}

		return "Unknown";
	}
}