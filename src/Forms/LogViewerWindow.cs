using System;
using System.Windows.Forms;

namespace ElfBot
{
	public partial class LogViewerWindow : Form
	{
		public LogViewerWindow()
		{
			InitializeComponent();
		}

		private void FormLoadEventHandler(object sender, EventArgs e)
		{
			LogRefreshTimer.Tick += LogRefreshTimer_Tick;
			LogRefreshTimer.Interval = 50;
			LogRefreshTimer.Start();
		}

		private void ClearLogsButton_Click(object sender, EventArgs e)
		{
			Globals.Logger.Entries.Clear();
			SystemLog.Clear();
		}

		private void LogRefreshTimer_Tick(object sender, EventArgs e)
		{
			var logEntries = Globals.Logger.Entries.ToArray();
			var currentLogLines = SystemLog.Lines.Length - 1;

			if (logEntries.Length == 0)
			{
				SystemLog.Clear();
				return;
			}

			// If the number of lines in the text box is the same as the logger entry count,
			// it means there are no updates to be performed so we exit the function.
			var numberOfMissingEntries = currentLogLines < 0 ? logEntries.Length : logEntries.Length - currentLogLines;
			if (numberOfMissingEntries == 0) return;
			
			// We need to update the SystemLog text array with all of the new entries
			// it is not yet displaying. We take the existing count and append all new 
			// values.
			for (var i = 0; i < numberOfMissingEntries; i++)
			{
				var index = logEntries.Length - (numberOfMissingEntries - i);
				var entry = logEntries[index];
				var date = entry.TimeStamp.ToString("hh:mm:ss tt");
				var line = $"{index} ({date}) {entry.Level}: {entry.Text}{Environment.NewLine}";
				SystemLog.AppendText(line);
			}
		}
	}
}
