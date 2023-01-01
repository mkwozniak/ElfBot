
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.mainWorker = new System.ComponentModel.BackgroundWorker();
			this.AutoCombatLabel = new System.Windows.Forms.Label();
			this.AutoCombatBox = new System.Windows.Forms.CheckBox();
			this.lootTimer = new System.Windows.Forms.Timer(this.components);
			this.CurrentXPLabel = new System.Windows.Forms.Label();
			this.XPBeforeKillLabel = new System.Windows.Forms.Label();
			this.AutoCombatState = new System.Windows.Forms.Label();
			this.TargetLabel = new System.Windows.Forms.Label();
			this.TargetUIDLabel = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.ProcessHookLabel = new System.Windows.Forms.Label();
			this.monsterRemoveBtn = new System.Windows.Forms.Button();
			this.monsterTablePanel = new System.Windows.Forms.Panel();
			this.monsterTableText = new System.Windows.Forms.Label();
			this.monsterAddBtn = new System.Windows.Forms.Button();
			this.monsterTableTitle = new System.Windows.Forms.Label();
			this.monsterInputBox = new System.Windows.Forms.MaskedTextBox();
			this.hookButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lootTimeInputBox = new System.Windows.Forms.MaskedTextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.actionDelayInputBox = new System.Windows.Forms.MaskedTextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.checkedListBox4 = new System.Windows.Forms.CheckedListBox();
			this.label9 = new System.Windows.Forms.Label();
			this.combatLootCheckbox = new System.Windows.Forms.CheckBox();
			this.label13 = new System.Windows.Forms.Label();
			this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
			this.checkedListBox2 = new System.Windows.Forms.CheckedListBox();
			this.checkedListBox3 = new System.Windows.Forms.CheckedListBox();
			this.label14 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.checkedListBox5 = new System.Windows.Forms.CheckedListBox();
			this.checkedListBox6 = new System.Windows.Forms.CheckedListBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.checkBox4 = new System.Windows.Forms.CheckBox();
			this.checkBox5 = new System.Windows.Forms.CheckBox();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.checkedListBox7 = new System.Windows.Forms.CheckedListBox();
			this.checkedListBox8 = new System.Windows.Forms.CheckedListBox();
			this.label18 = new System.Windows.Forms.Label();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.CombatOptions = new System.Windows.Forms.TabPage();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.mainTimer = new System.Windows.Forms.Timer(this.components);
			this.combatTimer = new System.Windows.Forms.Timer(this.components);
			this.targettingTimer = new System.Windows.Forms.Timer(this.components);
			this.checkTimer = new System.Windows.Forms.Timer(this.components);
			this.ErrorLabel = new System.Windows.Forms.Label();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.monsterTablePanel.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.CombatOptions.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.SuspendLayout();
			// 
			// AutoCombatLabel
			// 
			this.AutoCombatLabel.AutoSize = true;
			this.AutoCombatLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.AutoCombatLabel.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AutoCombatLabel.Location = new System.Drawing.Point(12, 28);
			this.AutoCombatLabel.Name = "AutoCombatLabel";
			this.AutoCombatLabel.Size = new System.Drawing.Size(65, 16);
			this.AutoCombatLabel.TabIndex = 2;
			this.AutoCombatLabel.Text = "DISABLED";
			// 
			// AutoCombatBox
			// 
			this.AutoCombatBox.AutoSize = true;
			this.AutoCombatBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AutoCombatBox.Location = new System.Drawing.Point(12, 5);
			this.AutoCombatBox.Name = "AutoCombatBox";
			this.AutoCombatBox.Size = new System.Drawing.Size(115, 20);
			this.AutoCombatBox.TabIndex = 3;
			this.AutoCombatBox.Text = "Auto Combat";
			this.AutoCombatBox.UseVisualStyleBackColor = true;
			this.AutoCombatBox.CheckedChanged += new System.EventHandler(this.AutoCombatBox_CheckedChanged);
			// 
			// CurrentXPLabel
			// 
			this.CurrentXPLabel.AutoSize = true;
			this.CurrentXPLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.CurrentXPLabel.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CurrentXPLabel.Location = new System.Drawing.Point(9, 45);
			this.CurrentXPLabel.Name = "CurrentXPLabel";
			this.CurrentXPLabel.Size = new System.Drawing.Size(100, 16);
			this.CurrentXPLabel.TabIndex = 4;
			this.CurrentXPLabel.Text = "Current XP: 0";
			// 
			// XPBeforeKillLabel
			// 
			this.XPBeforeKillLabel.AutoSize = true;
			this.XPBeforeKillLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.XPBeforeKillLabel.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.XPBeforeKillLabel.Location = new System.Drawing.Point(9, 61);
			this.XPBeforeKillLabel.Name = "XPBeforeKillLabel";
			this.XPBeforeKillLabel.Size = new System.Drawing.Size(135, 16);
			this.XPBeforeKillLabel.TabIndex = 5;
			this.XPBeforeKillLabel.Text = "XP Before Kill: -1";
			// 
			// AutoCombatState
			// 
			this.AutoCombatState.AutoSize = true;
			this.AutoCombatState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.AutoCombatState.Font = new System.Drawing.Font("Courier New", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AutoCombatState.Location = new System.Drawing.Point(83, 28);
			this.AutoCombatState.MaximumSize = new System.Drawing.Size(65, 16);
			this.AutoCombatState.Name = "AutoCombatState";
			this.AutoCombatState.Size = new System.Drawing.Size(65, 16);
			this.AutoCombatState.TabIndex = 7;
			this.AutoCombatState.Text = "INACTIVE";
			// 
			// TargetLabel
			// 
			this.TargetLabel.AutoSize = true;
			this.TargetLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.TargetLabel.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TargetLabel.Location = new System.Drawing.Point(9, 91);
			this.TargetLabel.Name = "TargetLabel";
			this.TargetLabel.Size = new System.Drawing.Size(58, 16);
			this.TargetLabel.TabIndex = 8;
			this.TargetLabel.Text = "Target:";
			// 
			// TargetUIDLabel
			// 
			this.TargetUIDLabel.AutoSize = true;
			this.TargetUIDLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.TargetUIDLabel.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TargetUIDLabel.Location = new System.Drawing.Point(9, 107);
			this.TargetUIDLabel.Name = "TargetUIDLabel";
			this.TargetUIDLabel.Size = new System.Drawing.Size(44, 16);
			this.TargetUIDLabel.TabIndex = 9;
			this.TargetUIDLabel.Text = "UID: ";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(751, 556);
			this.label1.Name = "label1";
			this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.label1.Size = new System.Drawing.Size(45, 17);
			this.label1.TabIndex = 10;
			this.label1.Text = "0.0.2 ";
			// 
			// ProcessHookLabel
			// 
			this.ProcessHookLabel.BackColor = System.Drawing.Color.Gray;
			this.ProcessHookLabel.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ProcessHookLabel.Location = new System.Drawing.Point(16, 547);
			this.ProcessHookLabel.Name = "ProcessHookLabel";
			this.ProcessHookLabel.Size = new System.Drawing.Size(175, 26);
			this.ProcessHookLabel.TabIndex = 11;
			this.ProcessHookLabel.Text = "Process Not Hooked";
			this.ProcessHookLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// monsterRemoveBtn
			// 
			this.monsterRemoveBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.monsterRemoveBtn.FlatAppearance.BorderSize = 2;
			this.monsterRemoveBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.monsterRemoveBtn.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.monsterRemoveBtn.ForeColor = System.Drawing.Color.Maroon;
			this.monsterRemoveBtn.Location = new System.Drawing.Point(53, 46);
			this.monsterRemoveBtn.Name = "monsterRemoveBtn";
			this.monsterRemoveBtn.Size = new System.Drawing.Size(64, 34);
			this.monsterRemoveBtn.TabIndex = 13;
			this.monsterRemoveBtn.Text = "REMOVE";
			this.monsterRemoveBtn.UseVisualStyleBackColor = true;
			this.monsterRemoveBtn.Click += new System.EventHandler(this.monsterRemoveBtn_Click);
			// 
			// monsterTablePanel
			// 
			this.monsterTablePanel.AutoScroll = true;
			this.monsterTablePanel.AutoSize = true;
			this.monsterTablePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.monsterTablePanel.BackColor = System.Drawing.Color.Silver;
			this.monsterTablePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.monsterTablePanel.Controls.Add(this.monsterTableText);
			this.monsterTablePanel.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.monsterTablePanel.Location = new System.Drawing.Point(7, 90);
			this.monsterTablePanel.MaximumSize = new System.Drawing.Size(150, 300);
			this.monsterTablePanel.MinimumSize = new System.Drawing.Size(110, 300);
			this.monsterTablePanel.Name = "monsterTablePanel";
			this.monsterTablePanel.Size = new System.Drawing.Size(110, 300);
			this.monsterTablePanel.TabIndex = 16;
			// 
			// monsterTableText
			// 
			this.monsterTableText.AutoSize = true;
			this.monsterTableText.Location = new System.Drawing.Point(3, 8);
			this.monsterTableText.Name = "monsterTableText";
			this.monsterTableText.Size = new System.Drawing.Size(42, 15);
			this.monsterTableText.TabIndex = 13;
			this.monsterTableText.Text = "Empty";
			// 
			// monsterAddBtn
			// 
			this.monsterAddBtn.FlatAppearance.BorderColor = System.Drawing.Color.Lime;
			this.monsterAddBtn.FlatAppearance.BorderSize = 2;
			this.monsterAddBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.monsterAddBtn.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.monsterAddBtn.ForeColor = System.Drawing.Color.ForestGreen;
			this.monsterAddBtn.Location = new System.Drawing.Point(7, 46);
			this.monsterAddBtn.Name = "monsterAddBtn";
			this.monsterAddBtn.Size = new System.Drawing.Size(45, 34);
			this.monsterAddBtn.TabIndex = 15;
			this.monsterAddBtn.Text = "ADD";
			this.monsterAddBtn.UseVisualStyleBackColor = true;
			this.monsterAddBtn.Click += new System.EventHandler(this.monsterAddBtn_Click);
			// 
			// monsterTableTitle
			// 
			this.monsterTableTitle.AutoSize = true;
			this.monsterTableTitle.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.monsterTableTitle.Location = new System.Drawing.Point(13, 3);
			this.monsterTableTitle.Name = "monsterTableTitle";
			this.monsterTableTitle.Size = new System.Drawing.Size(98, 14);
			this.monsterTableTitle.TabIndex = 14;
			this.monsterTableTitle.Text = "Monster Table";
			// 
			// monsterInputBox
			// 
			this.monsterInputBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.monsterInputBox.Location = new System.Drawing.Point(7, 20);
			this.monsterInputBox.Name = "monsterInputBox";
			this.monsterInputBox.Size = new System.Drawing.Size(112, 20);
			this.monsterInputBox.TabIndex = 13;
			// 
			// hookButton
			// 
			this.hookButton.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.hookButton.Location = new System.Drawing.Point(194, 547);
			this.hookButton.Name = "hookButton";
			this.hookButton.Size = new System.Drawing.Size(88, 23);
			this.hookButton.TabIndex = 13;
			this.hookButton.Text = "HOOK";
			this.hookButton.UseVisualStyleBackColor = true;
			this.hookButton.Click += new System.EventHandler(this.hookButton_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(6, 3);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 17);
			this.label2.TabIndex = 14;
			this.label2.Text = "Info";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label3.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(9, 29);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(107, 16);
			this.label3.TabIndex = 16;
			this.label3.Text = "Current Lvl: 0";
			// 
			// lootTimeInputBox
			// 
			this.lootTimeInputBox.Location = new System.Drawing.Point(9, 44);
			this.lootTimeInputBox.Name = "lootTimeInputBox";
			this.lootTimeInputBox.Size = new System.Drawing.Size(58, 20);
			this.lootTimeInputBox.TabIndex = 14;
			this.lootTimeInputBox.Text = "4";
			this.lootTimeInputBox.TextChanged += new System.EventHandler(this.lootTimeInputBox_InputChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(127, 3);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(91, 14);
			this.label5.TabIndex = 14;
			this.label5.Text = "Action Delay";
			// 
			// actionDelayInputBox
			// 
			this.actionDelayInputBox.Location = new System.Drawing.Point(126, 20);
			this.actionDelayInputBox.Name = "actionDelayInputBox";
			this.actionDelayInputBox.Size = new System.Drawing.Size(58, 20);
			this.actionDelayInputBox.TabIndex = 14;
			this.actionDelayInputBox.Text = "1";
			this.actionDelayInputBox.TextChanged += new System.EventHandler(this.actionDelayInputBox_InputChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 27);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(70, 14);
			this.label6.TabIndex = 14;
			this.label6.Text = "Loot Time";
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Enabled = false;
			this.checkBox1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox1.Location = new System.Drawing.Point(158, 5);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(99, 20);
			this.checkBox1.TabIndex = 15;
			this.checkBox1.Text = "Auto Food";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// checkedListBox4
			// 
			this.checkedListBox4.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.checkedListBox4.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkedListBox4.FormattingEnabled = true;
			this.checkedListBox4.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "0"});
			this.checkedListBox4.Location = new System.Drawing.Point(130, 276);
			this.checkedListBox4.Name = "checkedListBox4";
			this.checkedListBox4.Size = new System.Drawing.Size(33, 154);
			this.checkedListBox4.TabIndex = 1;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Courier New", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label9.Location = new System.Drawing.Point(167, 433);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(35, 14);
			this.label9.TabIndex = 0;
			this.label9.Text = "SHFT";
			// 
			// combatLootCheckbox
			// 
			this.combatLootCheckbox.AutoSize = true;
			this.combatLootCheckbox.Location = new System.Drawing.Point(8, 6);
			this.combatLootCheckbox.Name = "combatLootCheckbox";
			this.combatLootCheckbox.Size = new System.Drawing.Size(103, 18);
			this.combatLootCheckbox.TabIndex = 21;
			this.combatLootCheckbox.Text = "Combat Loot";
			this.combatLootCheckbox.UseVisualStyleBackColor = true;
			// 
			// label13
			// 
			this.label13.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label13.Location = new System.Drawing.Point(4, 245);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(45, 32);
			this.label13.TabIndex = 22;
			this.label13.Text = "Loot Keys";
			// 
			// checkedListBox1
			// 
			this.checkedListBox1.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.checkedListBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkedListBox1.FormattingEnabled = true;
			this.checkedListBox1.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "0"});
			this.checkedListBox1.Location = new System.Drawing.Point(169, 276);
			this.checkedListBox1.Name = "checkedListBox1";
			this.checkedListBox1.Size = new System.Drawing.Size(33, 154);
			this.checkedListBox1.TabIndex = 23;
			// 
			// checkedListBox2
			// 
			this.checkedListBox2.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.checkedListBox2.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkedListBox2.FormattingEnabled = true;
			this.checkedListBox2.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "0"});
			this.checkedListBox2.Location = new System.Drawing.Point(43, 280);
			this.checkedListBox2.Name = "checkedListBox2";
			this.checkedListBox2.Size = new System.Drawing.Size(33, 154);
			this.checkedListBox2.TabIndex = 24;
			// 
			// checkedListBox3
			// 
			this.checkedListBox3.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.checkedListBox3.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkedListBox3.FormattingEnabled = true;
			this.checkedListBox3.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "0"});
			this.checkedListBox3.Location = new System.Drawing.Point(4, 280);
			this.checkedListBox3.Name = "checkedListBox3";
			this.checkedListBox3.Size = new System.Drawing.Size(33, 154);
			this.checkedListBox3.TabIndex = 25;
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Font = new System.Drawing.Font("Courier New", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label14.Location = new System.Drawing.Point(41, 437);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(35, 14);
			this.label14.TabIndex = 26;
			this.label14.Text = "SHFT";
			// 
			// label15
			// 
			this.label15.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label15.Location = new System.Drawing.Point(127, 241);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(57, 32);
			this.label15.TabIndex = 27;
			this.label15.Text = "Combat Keys";
			// 
			// checkedListBox5
			// 
			this.checkedListBox5.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.checkedListBox5.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkedListBox5.FormattingEnabled = true;
			this.checkedListBox5.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "0"});
			this.checkedListBox5.Location = new System.Drawing.Point(46, 60);
			this.checkedListBox5.Name = "checkedListBox5";
			this.checkedListBox5.Size = new System.Drawing.Size(33, 154);
			this.checkedListBox5.TabIndex = 29;
			// 
			// checkedListBox6
			// 
			this.checkedListBox6.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.checkedListBox6.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkedListBox6.FormattingEnabled = true;
			this.checkedListBox6.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "0"});
			this.checkedListBox6.Location = new System.Drawing.Point(7, 60);
			this.checkedListBox6.Name = "checkedListBox6";
			this.checkedListBox6.Size = new System.Drawing.Size(33, 154);
			this.checkedListBox6.TabIndex = 28;
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(83, 60);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(45, 32);
			this.label8.TabIndex = 30;
			this.label8.Text = "HP Keys";
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label16.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label16.Location = new System.Drawing.Point(158, 28);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(65, 16);
			this.label16.TabIndex = 22;
			this.label16.Text = "DISABLED";
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label17.Font = new System.Drawing.Font("Courier New", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label17.Location = new System.Drawing.Point(229, 28);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(65, 16);
			this.label17.TabIndex = 23;
			this.label17.Text = "INACTIVE";
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            "10%",
            "20%",
            "30%",
            "40%",
            "50%",
            "60%",
            "70%",
            "80%",
            "90%"});
			this.comboBox1.Location = new System.Drawing.Point(6, 28);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(121, 22);
			this.comboBox1.TabIndex = 0;
			this.comboBox1.Text = "40%";
			// 
			// checkBox4
			// 
			this.checkBox4.AutoSize = true;
			this.checkBox4.Location = new System.Drawing.Point(6, 6);
			this.checkBox4.Name = "checkBox4";
			this.checkBox4.Size = new System.Drawing.Size(75, 18);
			this.checkBox4.TabIndex = 22;
			this.checkBox4.Text = "Auto HP";
			this.checkBox4.UseVisualStyleBackColor = true;
			// 
			// checkBox5
			// 
			this.checkBox5.AutoSize = true;
			this.checkBox5.Location = new System.Drawing.Point(133, 6);
			this.checkBox5.Name = "checkBox5";
			this.checkBox5.Size = new System.Drawing.Size(75, 18);
			this.checkBox5.TabIndex = 23;
			this.checkBox5.Text = "Auto MP";
			this.checkBox5.UseVisualStyleBackColor = true;
			// 
			// comboBox2
			// 
			this.comboBox2.FormattingEnabled = true;
			this.comboBox2.Items.AddRange(new object[] {
            "10%",
            "20%",
            "30%",
            "40%",
            "50%",
            "60%",
            "70%",
            "80%",
            "90%"});
			this.comboBox2.Location = new System.Drawing.Point(133, 28);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(121, 22);
			this.comboBox2.TabIndex = 24;
			this.comboBox2.Text = "40%";
			// 
			// checkedListBox7
			// 
			this.checkedListBox7.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.checkedListBox7.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkedListBox7.FormattingEnabled = true;
			this.checkedListBox7.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "0"});
			this.checkedListBox7.Location = new System.Drawing.Point(134, 60);
			this.checkedListBox7.Name = "checkedListBox7";
			this.checkedListBox7.Size = new System.Drawing.Size(33, 154);
			this.checkedListBox7.TabIndex = 31;
			// 
			// checkedListBox8
			// 
			this.checkedListBox8.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.checkedListBox8.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkedListBox8.FormattingEnabled = true;
			this.checkedListBox8.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "0"});
			this.checkedListBox8.Location = new System.Drawing.Point(173, 60);
			this.checkedListBox8.Name = "checkedListBox8";
			this.checkedListBox8.Size = new System.Drawing.Size(33, 154);
			this.checkedListBox8.TabIndex = 32;
			// 
			// label18
			// 
			this.label18.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label18.Location = new System.Drawing.Point(214, 60);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(45, 32);
			this.label18.TabIndex = 33;
			this.label18.Text = "MP Keys";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.CombatOptions);
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabControl1.Location = new System.Drawing.Point(12, 56);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(691, 481);
			this.tabControl1.TabIndex = 24;
			// 
			// CombatOptions
			// 
			this.CombatOptions.BackColor = System.Drawing.Color.Gray;
			this.CombatOptions.Controls.Add(this.label15);
			this.CombatOptions.Controls.Add(this.actionDelayInputBox);
			this.CombatOptions.Controls.Add(this.monsterRemoveBtn);
			this.CombatOptions.Controls.Add(this.label5);
			this.CombatOptions.Controls.Add(this.label9);
			this.CombatOptions.Controls.Add(this.monsterTablePanel);
			this.CombatOptions.Controls.Add(this.checkedListBox1);
			this.CombatOptions.Controls.Add(this.monsterAddBtn);
			this.CombatOptions.Controls.Add(this.monsterInputBox);
			this.CombatOptions.Controls.Add(this.monsterTableTitle);
			this.CombatOptions.Controls.Add(this.checkedListBox4);
			this.CombatOptions.Location = new System.Drawing.Point(4, 23);
			this.CombatOptions.Name = "CombatOptions";
			this.CombatOptions.Padding = new System.Windows.Forms.Padding(3);
			this.CombatOptions.Size = new System.Drawing.Size(683, 454);
			this.CombatOptions.TabIndex = 0;
			this.CombatOptions.Text = "Combat Options";
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.Color.Gray;
			this.tabPage1.Controls.Add(this.lootTimeInputBox);
			this.tabPage1.Controls.Add(this.label6);
			this.tabPage1.Controls.Add(this.combatLootCheckbox);
			this.tabPage1.Controls.Add(this.label13);
			this.tabPage1.Controls.Add(this.label14);
			this.tabPage1.Controls.Add(this.checkedListBox2);
			this.tabPage1.Controls.Add(this.checkedListBox3);
			this.tabPage1.Location = new System.Drawing.Point(4, 23);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(683, 454);
			this.tabPage1.TabIndex = 1;
			this.tabPage1.Text = "Loot Options";
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.Color.Gray;
			this.tabPage2.Controls.Add(this.label18);
			this.tabPage2.Controls.Add(this.checkedListBox8);
			this.tabPage2.Controls.Add(this.checkBox4);
			this.tabPage2.Controls.Add(this.checkedListBox7);
			this.tabPage2.Controls.Add(this.comboBox1);
			this.tabPage2.Controls.Add(this.label8);
			this.tabPage2.Controls.Add(this.checkedListBox6);
			this.tabPage2.Controls.Add(this.comboBox2);
			this.tabPage2.Controls.Add(this.checkBox5);
			this.tabPage2.Controls.Add(this.checkedListBox5);
			this.tabPage2.Location = new System.Drawing.Point(4, 23);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(683, 454);
			this.tabPage2.TabIndex = 2;
			this.tabPage2.Text = "Food Options";
			// 
			// tabPage3
			// 
			this.tabPage3.BackColor = System.Drawing.Color.Gray;
			this.tabPage3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tabPage3.Controls.Add(this.label3);
			this.tabPage3.Controls.Add(this.CurrentXPLabel);
			this.tabPage3.Controls.Add(this.XPBeforeKillLabel);
			this.tabPage3.Controls.Add(this.label2);
			this.tabPage3.Controls.Add(this.TargetLabel);
			this.tabPage3.Controls.Add(this.TargetUIDLabel);
			this.tabPage3.Location = new System.Drawing.Point(4, 23);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(683, 454);
			this.tabPage3.TabIndex = 3;
			this.tabPage3.Text = "Misc";
			// 
			// ErrorLabel
			// 
			this.ErrorLabel.BackColor = System.Drawing.Color.Gray;
			this.ErrorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.ErrorLabel.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ErrorLabel.Location = new System.Drawing.Point(288, 540);
			this.ErrorLabel.Name = "ErrorLabel";
			this.ErrorLabel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 8);
			this.ErrorLabel.Size = new System.Drawing.Size(415, 35);
			this.ErrorLabel.TabIndex = 25;
			this.ErrorLabel.Text = "Have fun!";
			// 
			// checkBox2
			// 
			this.checkBox2.AutoSize = true;
			this.checkBox2.Enabled = false;
			this.checkBox2.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox2.Location = new System.Drawing.Point(305, 5);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(99, 20);
			this.checkBox2.TabIndex = 26;
			this.checkBox2.Text = "Auto Chat";
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label4.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(305, 28);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(65, 16);
			this.label4.TabIndex = 27;
			this.label4.Text = "DISABLED";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label7.Font = new System.Drawing.Font("Courier New", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(376, 28);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(65, 16);
			this.label7.TabIndex = 28;
			this.label7.Text = "INACTIVE";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.DimGray;
			this.ClientSize = new System.Drawing.Size(802, 582);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.checkBox2);
			this.Controls.Add(this.ErrorLabel);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.label17);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.ProcessHookLabel);
			this.Controls.Add(this.AutoCombatLabel);
			this.Controls.Add(this.hookButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.AutoCombatBox);
			this.Controls.Add(this.AutoCombatState);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "ElfBot";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.monsterTablePanel.ResumeLayout(false);
			this.monsterTablePanel.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.CombatOptions.ResumeLayout(false);
			this.CombatOptions.PerformLayout();
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.ComponentModel.BackgroundWorker mainWorker;
		private System.Windows.Forms.Label AutoCombatLabel;
		private System.Windows.Forms.CheckBox AutoCombatBox;
		private System.Windows.Forms.Timer lootTimer;
		private System.Windows.Forms.Label CurrentXPLabel;
		private System.Windows.Forms.Label XPBeforeKillLabel;
		private System.Windows.Forms.Label AutoCombatState;
		private System.Windows.Forms.Label TargetLabel;
		private System.Windows.Forms.Label TargetUIDLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label ProcessHookLabel;
		private System.Windows.Forms.MaskedTextBox monsterInputBox;
		private System.Windows.Forms.Label monsterTableTitle;
		private System.Windows.Forms.Button monsterAddBtn;
		private System.Windows.Forms.Panel monsterTablePanel;
		private System.Windows.Forms.Label monsterTableText;
		private System.Windows.Forms.Button monsterRemoveBtn;
		private System.Windows.Forms.Button hookButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.MaskedTextBox lootTimeInputBox;
		private System.Windows.Forms.MaskedTextBox actionDelayInputBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.CheckBox combatLootCheckbox;
		private System.Windows.Forms.CheckedListBox checkedListBox4;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.CheckedListBox checkedListBox1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckedListBox checkedListBox5;
		private System.Windows.Forms.CheckedListBox checkedListBox6;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.CheckedListBox checkedListBox3;
		private System.Windows.Forms.CheckedListBox checkedListBox2;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.CheckedListBox checkedListBox8;
		private System.Windows.Forms.CheckedListBox checkedListBox7;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.CheckBox checkBox5;
		private System.Windows.Forms.CheckBox checkBox4;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage CombatOptions;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.Timer mainTimer;
		private System.Windows.Forms.Timer combatTimer;
		private System.Windows.Forms.Timer targettingTimer;
		private System.Windows.Forms.Timer checkTimer;
		private System.Windows.Forms.Label ErrorLabel;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label7;
	}
}

