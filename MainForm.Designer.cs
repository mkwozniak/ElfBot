
namespace ElfBot
{
	partial class MainForm
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
			this.mainWorker = new System.ComponentModel.BackgroundWorker();
			this.jellybeanTimer = new System.Windows.Forms.Timer(this.components);
			this.AutoJellyBeanLabel = new System.Windows.Forms.Label();
			this.AutoJellyBeanBox = new System.Windows.Forms.CheckBox();
			this.lootTimer = new System.Windows.Forms.Timer(this.components);
			this.CurrentXPLabel = new System.Windows.Forms.Label();
			this.XPBeforeKillLabel = new System.Windows.Forms.Label();
			this.AutoJellyBeanState = new System.Windows.Forms.Label();
			this.TargetLabel = new System.Windows.Forms.Label();
			this.TargetUIDLabel = new System.Windows.Forms.Label();
			this.attackingTimer = new System.Windows.Forms.Timer(this.components);
			this.targettingTimer = new System.Windows.Forms.Timer(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// jellybeanTimer
			// 
			this.jellybeanTimer.Tick += new System.EventHandler(this.jellybeanTimer_Tick);
			// 
			// AutoJellyBeanLabel
			// 
			this.AutoJellyBeanLabel.AutoSize = true;
			this.AutoJellyBeanLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.AutoJellyBeanLabel.Location = new System.Drawing.Point(9, 38);
			this.AutoJellyBeanLabel.Name = "AutoJellyBeanLabel";
			this.AutoJellyBeanLabel.Size = new System.Drawing.Size(62, 15);
			this.AutoJellyBeanLabel.TabIndex = 2;
			this.AutoJellyBeanLabel.Text = "DISABLED";
			// 
			// AutoJellyBeanBox
			// 
			this.AutoJellyBeanBox.AutoSize = true;
			this.AutoJellyBeanBox.Location = new System.Drawing.Point(12, 12);
			this.AutoJellyBeanBox.Name = "AutoJellyBeanBox";
			this.AutoJellyBeanBox.Size = new System.Drawing.Size(99, 17);
			this.AutoJellyBeanBox.TabIndex = 3;
			this.AutoJellyBeanBox.Text = "Auto Jelly Bean";
			this.AutoJellyBeanBox.UseVisualStyleBackColor = true;
			this.AutoJellyBeanBox.CheckedChanged += new System.EventHandler(this.AutoJellyBeanBox_CheckedChanged);
			// 
			// CurrentXPLabel
			// 
			this.CurrentXPLabel.AutoSize = true;
			this.CurrentXPLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.CurrentXPLabel.Location = new System.Drawing.Point(9, 65);
			this.CurrentXPLabel.Name = "CurrentXPLabel";
			this.CurrentXPLabel.Size = new System.Drawing.Size(72, 15);
			this.CurrentXPLabel.TabIndex = 4;
			this.CurrentXPLabel.Text = "Current XP: 0";
			// 
			// XPBeforeKillLabel
			// 
			this.XPBeforeKillLabel.AutoSize = true;
			this.XPBeforeKillLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.XPBeforeKillLabel.Location = new System.Drawing.Point(9, 90);
			this.XPBeforeKillLabel.Name = "XPBeforeKillLabel";
			this.XPBeforeKillLabel.Size = new System.Drawing.Size(88, 15);
			this.XPBeforeKillLabel.TabIndex = 5;
			this.XPBeforeKillLabel.Text = "XP Before Kill: -1";
			// 
			// AutoJellyBeanState
			// 
			this.AutoJellyBeanState.AutoSize = true;
			this.AutoJellyBeanState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.AutoJellyBeanState.Location = new System.Drawing.Point(77, 38);
			this.AutoJellyBeanState.Name = "AutoJellyBeanState";
			this.AutoJellyBeanState.Size = new System.Drawing.Size(58, 15);
			this.AutoJellyBeanState.TabIndex = 7;
			this.AutoJellyBeanState.Text = "INACTIVE";
			// 
			// TargetLabel
			// 
			this.TargetLabel.AutoSize = true;
			this.TargetLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TargetLabel.Location = new System.Drawing.Point(9, 114);
			this.TargetLabel.Name = "TargetLabel";
			this.TargetLabel.Size = new System.Drawing.Size(43, 15);
			this.TargetLabel.TabIndex = 8;
			this.TargetLabel.Text = "Target:";
			// 
			// TargetUIDLabel
			// 
			this.TargetUIDLabel.AutoSize = true;
			this.TargetUIDLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TargetUIDLabel.Location = new System.Drawing.Point(9, 139);
			this.TargetUIDLabel.Name = "TargetUIDLabel";
			this.TargetUIDLabel.Size = new System.Drawing.Size(65, 15);
			this.TargetUIDLabel.TabIndex = 9;
			this.TargetUIDLabel.Text = "Target UID:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(752, 426);
			this.label1.Name = "label1";
			this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.label1.Size = new System.Drawing.Size(45, 17);
			this.label1.TabIndex = 10;
			this.label1.Text = "0.0.1 ";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.TargetUIDLabel);
			this.Controls.Add(this.TargetLabel);
			this.Controls.Add(this.AutoJellyBeanState);
			this.Controls.Add(this.XPBeforeKillLabel);
			this.Controls.Add(this.CurrentXPLabel);
			this.Controls.Add(this.AutoJellyBeanBox);
			this.Controls.Add(this.AutoJellyBeanLabel);
			this.Name = "MainForm";
			this.Text = "ElfBot";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.ComponentModel.BackgroundWorker mainWorker;
		private System.Windows.Forms.Timer jellybeanTimer;
		private System.Windows.Forms.Label AutoJellyBeanLabel;
		private System.Windows.Forms.CheckBox AutoJellyBeanBox;
		private System.Windows.Forms.Timer lootTimer;
		private System.Windows.Forms.Label CurrentXPLabel;
		private System.Windows.Forms.Label XPBeforeKillLabel;
		private System.Windows.Forms.Label AutoJellyBeanState;
		private System.Windows.Forms.Label TargetLabel;
		private System.Windows.Forms.Label TargetUIDLabel;
		private System.Windows.Forms.Timer attackingTimer;
		private System.Windows.Forms.Timer targettingTimer;
		private System.Windows.Forms.Label label1;
	}
}

