namespace ElfBot
{
	partial class LogViewerWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogViewerWindow));
			this.SystemLog = new System.Windows.Forms.TextBox();
			this.ClearLogsButton = new System.Windows.Forms.Button();
			this.LogRefreshTimer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// SystemLog
			// 
			this.SystemLog.AcceptsReturn = true;
			this.SystemLog.AcceptsTab = true;
			this.SystemLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(71)))), ((int)(((byte)(90)))));
			this.SystemLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.SystemLog.Font = new System.Drawing.Font("MS Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SystemLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
			this.SystemLog.Location = new System.Drawing.Point(12, 28);
			this.SystemLog.Multiline = true;
			this.SystemLog.Name = "SystemLog";
			this.SystemLog.ReadOnly = true;
			this.SystemLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.SystemLog.Size = new System.Drawing.Size(776, 380);
			this.SystemLog.TabIndex = 41;
			this.SystemLog.WordWrap = false;
			// 
			// ClearLogsButton
			// 
			this.ClearLogsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(71)))), ((int)(((byte)(90)))));
			this.ClearLogsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ClearLogsButton.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ClearLogsButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
			this.ClearLogsButton.Location = new System.Drawing.Point(12, 418);
			this.ClearLogsButton.Name = "ClearLogsButton";
			this.ClearLogsButton.Size = new System.Drawing.Size(80, 20);
			this.ClearLogsButton.TabIndex = 42;
			this.ClearLogsButton.Text = "Clear";
			this.ClearLogsButton.UseVisualStyleBackColor = false;
			this.ClearLogsButton.Click += new System.EventHandler(this.ClearLogsButton_Click);
			// 
			// LogRefreshTimer
			// 
			this.LogRefreshTimer.Interval = 50;
			this.LogRefreshTimer.Tick += new System.EventHandler(this.LogRefreshTimer_Tick);
			// 
			// LogViewerWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(42)))), ((int)(((byte)(54)))));
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.ClearLogsButton);
			this.Controls.Add(this.SystemLog);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Location = new System.Drawing.Point(15, 15);
			this.Name = "LogViewerWindow";
			this.Text = "Logs";
			this.Load += new System.EventHandler(this.FormLoadEventHandler);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.Button ClearLogsButton;

		private System.Windows.Forms.TextBox SystemLog;

		#endregion

		private System.Windows.Forms.Timer LogRefreshTimer;
	}
}