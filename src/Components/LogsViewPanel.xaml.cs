using System;

namespace ElfBot.Components;

public partial class LogsViewPanel
{
	public LogsViewPanel()
	{
		InitializeComponent();
	}
	
	/// <summary> Clears the log </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void ClearLogsButton_Click(object sender, EventArgs e)
	{
		MainWindow.Logger.Clear();
		SystemMsgLog.Text = "";
	}
}