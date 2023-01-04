
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
			this.AutoCombatBox = new System.Windows.Forms.CheckBox();
			this.lootingEndTimer = new System.Windows.Forms.Timer(this.components);
			this.CurrentXPLabel = new System.Windows.Forms.Label();
			this.XPBeforeKillLabel = new System.Windows.Forms.Label();
			this.AutoCombatState = new System.Windows.Forms.Label();
			this.TargetLabel = new System.Windows.Forms.Label();
			this.TargetUIDLabel = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.ProcessHookLabel = new System.Windows.Forms.Label();
			this.monsterTablePanel = new System.Windows.Forms.Panel();
			this.monsterTableText = new System.Windows.Forms.Label();
			this.monsterTableTitle = new System.Windows.Forms.Label();
			this.monsterInputBox = new System.Windows.Forms.MaskedTextBox();
			this.hookButton = new System.Windows.Forms.Button();
			this.CharacterHeaderLabel = new System.Windows.Forms.Label();
			this.lootTimeInputBox = new System.Windows.Forms.MaskedTextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.actionDelayInputBox = new System.Windows.Forms.MaskedTextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.combatKeys = new System.Windows.Forms.CheckedListBox();
			this.label9 = new System.Windows.Forms.Label();
			this.combatLootCheckbox = new System.Windows.Forms.CheckBox();
			this.label13 = new System.Windows.Forms.Label();
			this.combatShiftKeys = new System.Windows.Forms.CheckedListBox();
			this.checkedListBox2 = new System.Windows.Forms.CheckedListBox();
			this.checkedListBox3 = new System.Windows.Forms.CheckedListBox();
			this.label14 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.checkedListBox5 = new System.Windows.Forms.CheckedListBox();
			this.hpKeys = new System.Windows.Forms.CheckedListBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.hpComboBox = new System.Windows.Forms.ComboBox();
			this.hpFoodCheckbox = new System.Windows.Forms.CheckBox();
			this.mpFoodCheckbox = new System.Windows.Forms.CheckBox();
			this.mpComboBox = new System.Windows.Forms.ComboBox();
			this.mpKeys = new System.Windows.Forms.CheckedListBox();
			this.checkedListBox8 = new System.Windows.Forms.CheckedListBox();
			this.label18 = new System.Windows.Forms.Label();
			this.updateCombatKeysBtn = new System.Windows.Forms.Button();
			this.label25 = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.eatKeyDelayInputBox = new System.Windows.Forms.MaskedTextBox();
			this.label21 = new System.Windows.Forms.Label();
			this.foodDelayInputBox = new System.Windows.Forms.MaskedTextBox();
			this.updateFoodKeysBtn = new System.Windows.Forms.Button();
			this.label24 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.ForceMaxZoomCheckbox = new System.Windows.Forms.CheckBox();
			this.loadTableButton = new System.Windows.Forms.Button();
			this.combatKeyDelayInputBox = new System.Windows.Forms.MaskedTextBox();
			this.label19 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.retargetTimeoutInputBox = new System.Windows.Forms.MaskedTextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.comboBox3 = new System.Windows.Forms.ComboBox();
			this.label11 = new System.Windows.Forms.Label();
			this.combatTimer = new System.Windows.Forms.Timer(this.components);
			this.targettingTimer = new System.Windows.Forms.Timer(this.components);
			this.checkTimer = new System.Windows.Forms.Timer(this.components);
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.attackTimeoutTimer = new System.Windows.Forms.Timer(this.components);
			this.lootingTimer = new System.Windows.Forms.Timer(this.components);
			this.interfaceTimer = new System.Windows.Forms.Timer(this.components);
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.resetCombatTimer = new System.Windows.Forms.Timer(this.components);
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.panel1 = new System.Windows.Forms.Panel();
			this.SecondClientCheckbox = new System.Windows.Forms.CheckBox();
			this.label16 = new System.Windows.Forms.Label();
			this.checkBox4 = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.SystemMsgLog = new System.Windows.Forms.TextBox();
			this.label23 = new System.Windows.Forms.Label();
			this.hpFoodTimer = new System.Windows.Forms.Timer(this.components);
			this.mpFoodTimer = new System.Windows.Forms.Timer(this.components);
			this.hpFoodKeyTimer = new System.Windows.Forms.Timer(this.components);
			this.mpFoodKeyTimer = new System.Windows.Forms.Timer(this.components);
			this.CombatOptionsBtn = new System.Windows.Forms.Button();
			this.LootOptionsBtn = new System.Windows.Forms.Button();
			this.FoodOptionsBtn = new System.Windows.Forms.Button();
			this.CombatOptionsPanel = new System.Windows.Forms.Panel();
			this.LootOptionsPanel = new System.Windows.Forms.Panel();
			this.maskedTextBox2 = new System.Windows.Forms.MaskedTextBox();
			this.checkBox5 = new System.Windows.Forms.CheckBox();
			this.FoodOptionsPanel = new System.Windows.Forms.Panel();
			this.MorePanelsBtn = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.MiscHeaderLabel = new System.Windows.Forms.Label();
			this.PlayerMPLabel = new System.Windows.Forms.Label();
			this.PlayerHPLabel = new System.Windows.Forms.Label();
			this.PlayerStatusHeaderLabel = new System.Windows.Forms.Label();
			this.PlayerMapIdLabel = new System.Windows.Forms.Label();
			this.PlayerPosZLabel = new System.Windows.Forms.Label();
			this.PlayerPosYLabel = new System.Windows.Forms.Label();
			this.PlayerPosXLabel = new System.Windows.Forms.Label();
			this.LocationHeaderLabel = new System.Windows.Forms.Label();
			this.PlayerZulyLabel = new System.Windows.Forms.Label();
			this.PlayerLevelLabel = new System.Windows.Forms.Label();
			this.PlayerNameLabel = new System.Windows.Forms.Label();
			this.CameraYawLabel = new System.Windows.Forms.Label();
			this.CameraPitchLabel = new System.Windows.Forms.Label();
			this.CameraZoomLabel = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.cameraTimer = new System.Windows.Forms.Timer(this.components);
			this.CameraOptionsBtn = new System.Windows.Forms.Button();
			this.CameraOptionsPanel = new System.Windows.Forms.Panel();
			this.TimedCameraYawCheckbox = new System.Windows.Forms.CheckBox();
			this.ForceTopdownCheckbox = new System.Windows.Forms.CheckBox();
			this.CameraYawDelayLabel = new System.Windows.Forms.Label();
			this.CameraYawDelayInputbox = new System.Windows.Forms.MaskedTextBox();
			this.CameraYawTimer = new System.Windows.Forms.Timer(this.components);
			this.ZHackPanel = new System.Windows.Forms.Panel();
			this.TimedZHackAmountInputbox = new System.Windows.Forms.MaskedTextBox();
			this.TimedZHackAmountLabel = new System.Windows.Forms.Label();
			this.TimedZHackDelayLabel = new System.Windows.Forms.Label();
			this.TimedZHackDelayInputbox = new System.Windows.Forms.MaskedTextBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.CameraHeaderLabel = new System.Windows.Forms.Label();
			this.monsterTablePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.panel1.SuspendLayout();
			this.CombatOptionsPanel.SuspendLayout();
			this.LootOptionsPanel.SuspendLayout();
			this.FoodOptionsPanel.SuspendLayout();
			this.panel2.SuspendLayout();
			this.CameraOptionsPanel.SuspendLayout();
			this.ZHackPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// AutoCombatBox
			// 
			this.AutoCombatBox.BackColor = System.Drawing.Color.Black;
			this.AutoCombatBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AutoCombatBox.Font = new System.Drawing.Font("MS Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AutoCombatBox.ForeColor = System.Drawing.Color.White;
			this.AutoCombatBox.Location = new System.Drawing.Point(3, 3);
			this.AutoCombatBox.Name = "AutoCombatBox";
			this.AutoCombatBox.Size = new System.Drawing.Size(80, 40);
			this.AutoCombatBox.TabIndex = 3;
			this.AutoCombatBox.Text = "Auto Combat";
			this.AutoCombatBox.UseVisualStyleBackColor = false;
			this.AutoCombatBox.CheckedChanged += new System.EventHandler(this.AutoCombatBox_CheckedChanged);
			// 
			// CurrentXPLabel
			// 
			this.CurrentXPLabel.AutoSize = true;
			this.CurrentXPLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CurrentXPLabel.Location = new System.Drawing.Point(12, 67);
			this.CurrentXPLabel.Name = "CurrentXPLabel";
			this.CurrentXPLabel.Size = new System.Drawing.Size(47, 11);
			this.CurrentXPLabel.TabIndex = 4;
			this.CurrentXPLabel.Text = "XP: N/A";
			// 
			// XPBeforeKillLabel
			// 
			this.XPBeforeKillLabel.AutoSize = true;
			this.XPBeforeKillLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.XPBeforeKillLabel.Location = new System.Drawing.Point(12, 362);
			this.XPBeforeKillLabel.Name = "XPBeforeKillLabel";
			this.XPBeforeKillLabel.Size = new System.Drawing.Size(119, 11);
			this.XPBeforeKillLabel.TabIndex = 5;
			this.XPBeforeKillLabel.Text = "XP Before Kill: N/A";
			// 
			// AutoCombatState
			// 
			this.AutoCombatState.BackColor = System.Drawing.Color.Black;
			this.AutoCombatState.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AutoCombatState.ForeColor = System.Drawing.Color.White;
			this.AutoCombatState.Location = new System.Drawing.Point(3, 44);
			this.AutoCombatState.Name = "AutoCombatState";
			this.AutoCombatState.Size = new System.Drawing.Size(80, 20);
			this.AutoCombatState.TabIndex = 7;
			this.AutoCombatState.Text = "-";
			this.AutoCombatState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// TargetLabel
			// 
			this.TargetLabel.AutoSize = true;
			this.TargetLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TargetLabel.Location = new System.Drawing.Point(12, 379);
			this.TargetLabel.Name = "TargetLabel";
			this.TargetLabel.Size = new System.Drawing.Size(71, 11);
			this.TargetLabel.TabIndex = 8;
			this.TargetLabel.Text = "Target: N/A";
			// 
			// TargetUIDLabel
			// 
			this.TargetUIDLabel.AutoSize = true;
			this.TargetUIDLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TargetUIDLabel.Location = new System.Drawing.Point(12, 396);
			this.TargetUIDLabel.Name = "TargetUIDLabel";
			this.TargetUIDLabel.Size = new System.Drawing.Size(95, 11);
			this.TargetUIDLabel.TabIndex = 9;
			this.TargetUIDLabel.Text = "Target UID: N/A";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("MS Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(326, 528);
			this.label1.Name = "label1";
			this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.label1.Size = new System.Drawing.Size(42, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "0.0.5";
			// 
			// ProcessHookLabel
			// 
			this.ProcessHookLabel.BackColor = System.Drawing.Color.Gray;
			this.ProcessHookLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.ProcessHookLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ProcessHookLabel.Location = new System.Drawing.Point(318, 414);
			this.ProcessHookLabel.Name = "ProcessHookLabel";
			this.ProcessHookLabel.Size = new System.Drawing.Size(123, 30);
			this.ProcessHookLabel.TabIndex = 11;
			this.ProcessHookLabel.Text = "Process Not Hooked";
			this.ProcessHookLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
			this.monsterTablePanel.Location = new System.Drawing.Point(6, 20);
			this.monsterTablePanel.MaximumSize = new System.Drawing.Size(120, 270);
			this.monsterTablePanel.MinimumSize = new System.Drawing.Size(120, 270);
			this.monsterTablePanel.Name = "monsterTablePanel";
			this.monsterTablePanel.Size = new System.Drawing.Size(120, 270);
			this.monsterTablePanel.TabIndex = 16;
			// 
			// monsterTableText
			// 
			this.monsterTableText.AutoSize = true;
			this.monsterTableText.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.monsterTableText.Location = new System.Drawing.Point(3, 8);
			this.monsterTableText.Name = "monsterTableText";
			this.monsterTableText.Size = new System.Drawing.Size(35, 11);
			this.monsterTableText.TabIndex = 13;
			this.monsterTableText.Text = "Empty";
			// 
			// monsterTableTitle
			// 
			this.monsterTableTitle.AutoSize = true;
			this.monsterTableTitle.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.monsterTableTitle.Location = new System.Drawing.Point(16, 3);
			this.monsterTableTitle.Name = "monsterTableTitle";
			this.monsterTableTitle.Size = new System.Drawing.Size(96, 11);
			this.monsterTableTitle.TabIndex = 14;
			this.monsterTableTitle.Text = "Monster Table";
			// 
			// monsterInputBox
			// 
			this.monsterInputBox.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.monsterInputBox.Location = new System.Drawing.Point(9, 298);
			this.monsterInputBox.Name = "monsterInputBox";
			this.monsterInputBox.Size = new System.Drawing.Size(112, 18);
			this.monsterInputBox.TabIndex = 13;
			// 
			// hookButton
			// 
			this.hookButton.BackColor = System.Drawing.Color.DimGray;
			this.hookButton.FlatAppearance.BorderSize = 0;
			this.hookButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.hookButton.Font = new System.Drawing.Font("MS Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.hookButton.Location = new System.Drawing.Point(318, 447);
			this.hookButton.Margin = new System.Windows.Forms.Padding(0);
			this.hookButton.Name = "hookButton";
			this.hookButton.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this.hookButton.Size = new System.Drawing.Size(123, 41);
			this.hookButton.TabIndex = 13;
			this.hookButton.Text = "H O O K";
			this.hookButton.UseVisualStyleBackColor = false;
			this.hookButton.Click += new System.EventHandler(this.hookButton_Click);
			// 
			// CharacterHeaderLabel
			// 
			this.CharacterHeaderLabel.AutoSize = true;
			this.CharacterHeaderLabel.Font = new System.Drawing.Font("MS Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CharacterHeaderLabel.Location = new System.Drawing.Point(2, 11);
			this.CharacterHeaderLabel.Name = "CharacterHeaderLabel";
			this.CharacterHeaderLabel.Size = new System.Drawing.Size(70, 13);
			this.CharacterHeaderLabel.TabIndex = 14;
			this.CharacterHeaderLabel.Text = "Character";
			// 
			// lootTimeInputBox
			// 
			this.lootTimeInputBox.Location = new System.Drawing.Point(10, 44);
			this.lootTimeInputBox.Name = "lootTimeInputBox";
			this.lootTimeInputBox.Size = new System.Drawing.Size(58, 18);
			this.lootTimeInputBox.TabIndex = 14;
			this.lootTimeInputBox.Text = "4";
			this.lootTimeInputBox.TextChanged += new System.EventHandler(this.lootTimeInputBox_InputChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label5.Location = new System.Drawing.Point(132, 7);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(79, 13);
			this.label5.TabIndex = 14;
			this.label5.Text = "Action Delay";
			// 
			// actionDelayInputBox
			// 
			this.actionDelayInputBox.Location = new System.Drawing.Point(132, 20);
			this.actionDelayInputBox.Name = "actionDelayInputBox";
			this.actionDelayInputBox.Size = new System.Drawing.Size(58, 18);
			this.actionDelayInputBox.TabIndex = 14;
			this.actionDelayInputBox.Text = "0.5";
			this.actionDelayInputBox.TextChanged += new System.EventHandler(this.actionDelayInputBox_InputChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(10, 29);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(59, 11);
			this.label6.TabIndex = 14;
			this.label6.Text = "Loot Time";
			// 
			// combatKeys
			// 
			this.combatKeys.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.combatKeys.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.combatKeys.FormattingEnabled = true;
			this.combatKeys.Items.AddRange(new object[] {
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
			this.combatKeys.Location = new System.Drawing.Point(152, 268);
			this.combatKeys.Name = "combatKeys";
			this.combatKeys.Size = new System.Drawing.Size(33, 154);
			this.combatKeys.TabIndex = 1;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label9.Location = new System.Drawing.Point(189, 429);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(35, 11);
			this.label9.TabIndex = 0;
			this.label9.Text = "SHIFT";
			// 
			// combatLootCheckbox
			// 
			this.combatLootCheckbox.AutoSize = true;
			this.combatLootCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.combatLootCheckbox.Location = new System.Drawing.Point(10, 11);
			this.combatLootCheckbox.Name = "combatLootCheckbox";
			this.combatLootCheckbox.Size = new System.Drawing.Size(88, 15);
			this.combatLootCheckbox.TabIndex = 21;
			this.combatLootCheckbox.Text = "Combat Loot";
			this.combatLootCheckbox.UseVisualStyleBackColor = true;
			// 
			// label13
			// 
			this.label13.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label13.Location = new System.Drawing.Point(15, 239);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(72, 26);
			this.label13.TabIndex = 22;
			this.label13.Text = "Loot Keys";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// combatShiftKeys
			// 
			this.combatShiftKeys.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.combatShiftKeys.Enabled = false;
			this.combatShiftKeys.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.combatShiftKeys.FormattingEnabled = true;
			this.combatShiftKeys.Items.AddRange(new object[] {
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
			this.combatShiftKeys.Location = new System.Drawing.Point(191, 268);
			this.combatShiftKeys.Name = "combatShiftKeys";
			this.combatShiftKeys.Size = new System.Drawing.Size(33, 154);
			this.combatShiftKeys.TabIndex = 23;
			// 
			// checkedListBox2
			// 
			this.checkedListBox2.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.checkedListBox2.Enabled = false;
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
			this.checkedListBox2.Location = new System.Drawing.Point(54, 268);
			this.checkedListBox2.Name = "checkedListBox2";
			this.checkedListBox2.Size = new System.Drawing.Size(33, 154);
			this.checkedListBox2.TabIndex = 24;
			// 
			// checkedListBox3
			// 
			this.checkedListBox3.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.checkedListBox3.Enabled = false;
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
			this.checkedListBox3.Location = new System.Drawing.Point(15, 268);
			this.checkedListBox3.Name = "checkedListBox3";
			this.checkedListBox3.Size = new System.Drawing.Size(33, 154);
			this.checkedListBox3.TabIndex = 25;
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label14.Location = new System.Drawing.Point(52, 425);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(35, 11);
			this.label14.TabIndex = 26;
			this.label14.Text = "SHIFT";
			// 
			// label15
			// 
			this.label15.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label15.Location = new System.Drawing.Point(132, 213);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(124, 23);
			this.label15.TabIndex = 27;
			this.label15.Text = "Combat Keys";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// checkedListBox5
			// 
			this.checkedListBox5.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.checkedListBox5.Enabled = false;
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
			this.checkedListBox5.Location = new System.Drawing.Point(55, 262);
			this.checkedListBox5.Name = "checkedListBox5";
			this.checkedListBox5.Size = new System.Drawing.Size(33, 154);
			this.checkedListBox5.TabIndex = 29;
			// 
			// hpKeys
			// 
			this.hpKeys.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.hpKeys.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.hpKeys.FormattingEnabled = true;
			this.hpKeys.Items.AddRange(new object[] {
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
			this.hpKeys.Location = new System.Drawing.Point(16, 262);
			this.hpKeys.Name = "hpKeys";
			this.hpKeys.Size = new System.Drawing.Size(33, 154);
			this.hpKeys.TabIndex = 28;
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(16, 239);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(72, 20);
			this.label8.TabIndex = 30;
			this.label8.Text = "HP Keys";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label17
			// 
			this.label17.BackColor = System.Drawing.Color.Black;
			this.label17.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label17.ForeColor = System.Drawing.Color.White;
			this.label17.Location = new System.Drawing.Point(89, 44);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(80, 20);
			this.label17.TabIndex = 23;
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// hpComboBox
			// 
			this.hpComboBox.BackColor = System.Drawing.Color.DimGray;
			this.hpComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.hpComboBox.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.hpComboBox.ForeColor = System.Drawing.Color.White;
			this.hpComboBox.FormattingEnabled = true;
			this.hpComboBox.Items.AddRange(new object[] {
            "10%",
            "20%",
            "30%",
            "40%",
            "50%",
            "60%",
            "70%",
            "80%",
            "90%"});
			this.hpComboBox.Location = new System.Drawing.Point(6, 34);
			this.hpComboBox.Name = "hpComboBox";
			this.hpComboBox.Size = new System.Drawing.Size(121, 19);
			this.hpComboBox.TabIndex = 0;
			this.hpComboBox.Text = "40%";
			// 
			// hpFoodCheckbox
			// 
			this.hpFoodCheckbox.AutoSize = true;
			this.hpFoodCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.hpFoodCheckbox.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.hpFoodCheckbox.Location = new System.Drawing.Point(6, 12);
			this.hpFoodCheckbox.Name = "hpFoodCheckbox";
			this.hpFoodCheckbox.Size = new System.Drawing.Size(63, 15);
			this.hpFoodCheckbox.TabIndex = 22;
			this.hpFoodCheckbox.Text = "Auto HP";
			this.hpFoodCheckbox.UseVisualStyleBackColor = true;
			this.hpFoodCheckbox.CheckedChanged += new System.EventHandler(this.hpFoodCheckbox_CheckedChanged);
			// 
			// mpFoodCheckbox
			// 
			this.mpFoodCheckbox.AutoSize = true;
			this.mpFoodCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mpFoodCheckbox.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mpFoodCheckbox.Location = new System.Drawing.Point(133, 12);
			this.mpFoodCheckbox.Name = "mpFoodCheckbox";
			this.mpFoodCheckbox.Size = new System.Drawing.Size(63, 15);
			this.mpFoodCheckbox.TabIndex = 23;
			this.mpFoodCheckbox.Text = "Auto MP";
			this.mpFoodCheckbox.UseVisualStyleBackColor = true;
			this.mpFoodCheckbox.CheckedChanged += new System.EventHandler(this.mpFoodCheckbox_CheckedChanged);
			// 
			// mpComboBox
			// 
			this.mpComboBox.BackColor = System.Drawing.Color.DimGray;
			this.mpComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.mpComboBox.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mpComboBox.ForeColor = System.Drawing.Color.White;
			this.mpComboBox.FormattingEnabled = true;
			this.mpComboBox.Items.AddRange(new object[] {
            "10%",
            "20%",
            "30%",
            "40%",
            "50%",
            "60%",
            "70%",
            "80%",
            "90%"});
			this.mpComboBox.Location = new System.Drawing.Point(133, 34);
			this.mpComboBox.Name = "mpComboBox";
			this.mpComboBox.Size = new System.Drawing.Size(121, 19);
			this.mpComboBox.TabIndex = 24;
			this.mpComboBox.Text = "40%";
			// 
			// mpKeys
			// 
			this.mpKeys.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.mpKeys.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mpKeys.FormattingEnabled = true;
			this.mpKeys.Items.AddRange(new object[] {
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
			this.mpKeys.Location = new System.Drawing.Point(94, 262);
			this.mpKeys.Name = "mpKeys";
			this.mpKeys.Size = new System.Drawing.Size(33, 154);
			this.mpKeys.TabIndex = 31;
			// 
			// checkedListBox8
			// 
			this.checkedListBox8.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.checkedListBox8.Enabled = false;
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
			this.checkedListBox8.Location = new System.Drawing.Point(133, 262);
			this.checkedListBox8.Name = "checkedListBox8";
			this.checkedListBox8.Size = new System.Drawing.Size(33, 154);
			this.checkedListBox8.TabIndex = 32;
			// 
			// label18
			// 
			this.label18.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label18.Location = new System.Drawing.Point(94, 239);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(72, 20);
			this.label18.TabIndex = 33;
			this.label18.Text = "MP Keys";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// updateCombatKeysBtn
			// 
			this.updateCombatKeysBtn.BackColor = System.Drawing.Color.Gray;
			this.updateCombatKeysBtn.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.updateCombatKeysBtn.Location = new System.Drawing.Point(132, 239);
			this.updateCombatKeysBtn.Name = "updateCombatKeysBtn";
			this.updateCombatKeysBtn.Padding = new System.Windows.Forms.Padding(4, 3, 0, 0);
			this.updateCombatKeysBtn.Size = new System.Drawing.Size(124, 23);
			this.updateCombatKeysBtn.TabIndex = 33;
			this.updateCombatKeysBtn.Text = "UPDATE KEYS";
			this.updateCombatKeysBtn.UseVisualStyleBackColor = false;
			this.updateCombatKeysBtn.Click += new System.EventHandler(this.updateCombatKeysBtn_Click);
			// 
			// label25
			// 
			this.label25.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label25.Location = new System.Drawing.Point(136, 104);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(140, 40);
			this.label25.TabIndex = 41;
			this.label25.Text = "Don\'t set this too low or it will eat all your food!";
			// 
			// label22
			// 
			this.label22.AutoSize = true;
			this.label22.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label22.Location = new System.Drawing.Point(136, 67);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(83, 11);
			this.label22.TabIndex = 40;
			this.label22.Text = "Eat Key Delay";
			// 
			// eatKeyDelayInputBox
			// 
			this.eatKeyDelayInputBox.Location = new System.Drawing.Point(138, 81);
			this.eatKeyDelayInputBox.Name = "eatKeyDelayInputBox";
			this.eatKeyDelayInputBox.Size = new System.Drawing.Size(58, 18);
			this.eatKeyDelayInputBox.TabIndex = 39;
			this.eatKeyDelayInputBox.Text = "10";
			this.eatKeyDelayInputBox.TextChanged += new System.EventHandler(this.eatKeyDelayInputBox_InputChanged);
			// 
			// label21
			// 
			this.label21.AutoSize = true;
			this.label21.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label21.Location = new System.Drawing.Point(7, 67);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(101, 11);
			this.label21.TabIndex = 38;
			this.label21.Text = "Check Food Delay";
			// 
			// foodDelayInputBox
			// 
			this.foodDelayInputBox.Location = new System.Drawing.Point(6, 81);
			this.foodDelayInputBox.Name = "foodDelayInputBox";
			this.foodDelayInputBox.Size = new System.Drawing.Size(58, 18);
			this.foodDelayInputBox.TabIndex = 37;
			this.foodDelayInputBox.Text = "1";
			this.foodDelayInputBox.TextChanged += new System.EventHandler(this.foodDelayInputBox_InputChanged);
			// 
			// updateFoodKeysBtn
			// 
			this.updateFoodKeysBtn.BackColor = System.Drawing.Color.Gray;
			this.updateFoodKeysBtn.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.updateFoodKeysBtn.Location = new System.Drawing.Point(12, 213);
			this.updateFoodKeysBtn.Name = "updateFoodKeysBtn";
			this.updateFoodKeysBtn.Padding = new System.Windows.Forms.Padding(4, 3, 0, 0);
			this.updateFoodKeysBtn.Size = new System.Drawing.Size(153, 23);
			this.updateFoodKeysBtn.TabIndex = 36;
			this.updateFoodKeysBtn.Text = "UPDATE KEYS";
			this.updateFoodKeysBtn.UseVisualStyleBackColor = false;
			this.updateFoodKeysBtn.Click += new System.EventHandler(this.updateFoodKeysBtn_Click);
			// 
			// label24
			// 
			this.label24.AutoSize = true;
			this.label24.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label24.Location = new System.Drawing.Point(53, 419);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(35, 11);
			this.label24.TabIndex = 35;
			this.label24.Text = "SHIFT";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(130, 419);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 11);
			this.label3.TabIndex = 34;
			this.label3.Text = "SHIFT";
			// 
			// ForceMaxZoomCheckbox
			// 
			this.ForceMaxZoomCheckbox.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ForceMaxZoomCheckbox.Location = new System.Drawing.Point(4, 7);
			this.ForceMaxZoomCheckbox.Name = "ForceMaxZoomCheckbox";
			this.ForceMaxZoomCheckbox.Size = new System.Drawing.Size(74, 40);
			this.ForceMaxZoomCheckbox.TabIndex = 39;
			this.ForceMaxZoomCheckbox.Text = "Force Camera Max Zoom";
			this.ForceMaxZoomCheckbox.UseVisualStyleBackColor = true;
			this.ForceMaxZoomCheckbox.CheckedChanged += new System.EventHandler(this.ForceMaxZoomCheckbox_CheckedChanged);
			// 
			// loadTableButton
			// 
			this.loadTableButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.loadTableButton.Location = new System.Drawing.Point(9, 326);
			this.loadTableButton.Name = "loadTableButton";
			this.loadTableButton.Size = new System.Drawing.Size(112, 28);
			this.loadTableButton.TabIndex = 38;
			this.loadTableButton.Text = "LOAD TABLE";
			this.loadTableButton.UseVisualStyleBackColor = true;
			this.loadTableButton.Click += new System.EventHandler(this.loadTableButton_Click);
			// 
			// combatKeyDelayInputBox
			// 
			this.combatKeyDelayInputBox.Location = new System.Drawing.Point(134, 144);
			this.combatKeyDelayInputBox.Name = "combatKeyDelayInputBox";
			this.combatKeyDelayInputBox.Size = new System.Drawing.Size(58, 18);
			this.combatKeyDelayInputBox.TabIndex = 34;
			this.combatKeyDelayInputBox.Text = "1";
			this.combatKeyDelayInputBox.TextChanged += new System.EventHandler(this.combatKeyDelayInputBox_InputChanged);
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label19.Location = new System.Drawing.Point(134, 131);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(103, 13);
			this.label19.TabIndex = 35;
			this.label19.Text = "Combat Key Delay";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label12.Location = new System.Drawing.Point(132, 49);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(103, 13);
			this.label12.TabIndex = 32;
			this.label12.Text = "Retarget Timeout";
			// 
			// retargetTimeoutInputBox
			// 
			this.retargetTimeoutInputBox.Location = new System.Drawing.Point(132, 62);
			this.retargetTimeoutInputBox.Name = "retargetTimeoutInputBox";
			this.retargetTimeoutInputBox.Size = new System.Drawing.Size(58, 18);
			this.retargetTimeoutInputBox.TabIndex = 31;
			this.retargetTimeoutInputBox.Text = "15";
			this.retargetTimeoutInputBox.TextChanged += new System.EventHandler(this.retargetTimeoutInputBox_InputChanged);
			// 
			// label10
			// 
			this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label10.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(130, 170);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(124, 15);
			this.label10.TabIndex = 29;
			this.label10.Text = "Combat Key Mode";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// comboBox3
			// 
			this.comboBox3.Enabled = false;
			this.comboBox3.FormattingEnabled = true;
			this.comboBox3.Items.AddRange(new object[] {
            "Random",
            "Alpha Ordered"});
			this.comboBox3.Location = new System.Drawing.Point(133, 188);
			this.comboBox3.Name = "comboBox3";
			this.comboBox3.Size = new System.Drawing.Size(121, 19);
			this.comboBox3.TabIndex = 28;
			this.comboBox3.Text = "Random";
			// 
			// label11
			// 
			this.label11.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label11.ForeColor = System.Drawing.Color.Red;
			this.label11.Location = new System.Drawing.Point(14, 170);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(130, 40);
			this.label11.TabIndex = 30;
			this.label11.Text = "Disable Auto Combat before making any changes";
			// 
			// combatTimer
			// 
			this.combatTimer.Interval = 1000;
			// 
			// checkBox2
			// 
			this.checkBox2.BackColor = System.Drawing.Color.Black;
			this.checkBox2.Enabled = false;
			this.checkBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBox2.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox2.ForeColor = System.Drawing.Color.White;
			this.checkBox2.Location = new System.Drawing.Point(84, 1);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(80, 40);
			this.checkBox2.TabIndex = 26;
			this.checkBox2.Text = "Auto Chat";
			this.checkBox2.UseVisualStyleBackColor = false;
			// 
			// label7
			// 
			this.label7.BackColor = System.Drawing.Color.Black;
			this.label7.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.ForeColor = System.Drawing.Color.White;
			this.label7.Location = new System.Drawing.Point(175, 44);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(80, 20);
			this.label7.TabIndex = 28;
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::ElfBot.Properties.Resources.elfbot_icon_large;
			this.pictureBox1.InitialImage = global::ElfBot.Properties.Resources.elfbot_icon_large;
			this.pictureBox1.Location = new System.Drawing.Point(374, 514);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(71, 50);
			this.pictureBox1.TabIndex = 29;
			this.pictureBox1.TabStop = false;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			this.openFileDialog1.Filter = "Text files (*.txt)|*.txt";
			// 
			// panel1
			// 
			this.panel1.AutoScroll = true;
			this.panel1.AutoSize = true;
			this.panel1.BackColor = System.Drawing.Color.Black;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.SecondClientCheckbox);
			this.panel1.Controls.Add(this.label16);
			this.panel1.Controls.Add(this.checkBox4);
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.checkBox3);
			this.panel1.Controls.Add(this.AutoCombatBox);
			this.panel1.Controls.Add(this.label7);
			this.panel1.Controls.Add(this.AutoCombatState);
			this.panel1.Controls.Add(this.checkBox2);
			this.panel1.Controls.Add(this.label17);
			this.panel1.Location = new System.Drawing.Point(9, 9);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(436, 75);
			this.panel1.TabIndex = 30;
			// 
			// SecondClientCheckbox
			// 
			this.SecondClientCheckbox.AutoSize = true;
			this.SecondClientCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SecondClientCheckbox.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SecondClientCheckbox.ForeColor = System.Drawing.Color.White;
			this.SecondClientCheckbox.Location = new System.Drawing.Point(347, 55);
			this.SecondClientCheckbox.Name = "SecondClientCheckbox";
			this.SecondClientCheckbox.Size = new System.Drawing.Size(81, 15);
			this.SecondClientCheckbox.TabIndex = 19;
			this.SecondClientCheckbox.Text = "2nd Client";
			this.SecondClientCheckbox.UseVisualStyleBackColor = true;
			this.SecondClientCheckbox.CheckedChanged += new System.EventHandler(this.SecondClientCheckbox_CheckedChanged);
			// 
			// label16
			// 
			this.label16.BackColor = System.Drawing.Color.Black;
			this.label16.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label16.ForeColor = System.Drawing.Color.White;
			this.label16.Location = new System.Drawing.Point(351, 44);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(80, 20);
			this.label16.TabIndex = 44;
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBox4
			// 
			this.checkBox4.BackColor = System.Drawing.Color.Black;
			this.checkBox4.Enabled = false;
			this.checkBox4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBox4.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox4.ForeColor = System.Drawing.Color.White;
			this.checkBox4.Location = new System.Drawing.Point(260, 1);
			this.checkBox4.Name = "checkBox4";
			this.checkBox4.Size = new System.Drawing.Size(80, 40);
			this.checkBox4.TabIndex = 43;
			this.checkBox4.Text = "Auto Shop";
			this.checkBox4.UseVisualStyleBackColor = false;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.Black;
			this.label4.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.ForeColor = System.Drawing.Color.White;
			this.label4.Location = new System.Drawing.Point(263, 44);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 20);
			this.label4.TabIndex = 34;
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBox3
			// 
			this.checkBox3.BackColor = System.Drawing.Color.Black;
			this.checkBox3.Enabled = false;
			this.checkBox3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkBox3.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkBox3.ForeColor = System.Drawing.Color.White;
			this.checkBox3.Location = new System.Drawing.Point(172, 1);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(80, 40);
			this.checkBox3.TabIndex = 31;
			this.checkBox3.Text = "Auto Buff";
			this.checkBox3.UseVisualStyleBackColor = false;
			// 
			// SystemMsgLog
			// 
			this.SystemMsgLog.AcceptsReturn = true;
			this.SystemMsgLog.AcceptsTab = true;
			this.SystemMsgLog.BackColor = System.Drawing.Color.Black;
			this.SystemMsgLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.SystemMsgLog.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SystemMsgLog.ForeColor = System.Drawing.Color.White;
			this.SystemMsgLog.Location = new System.Drawing.Point(315, 109);
			this.SystemMsgLog.MaximumSize = new System.Drawing.Size(130, 302);
			this.SystemMsgLog.MinimumSize = new System.Drawing.Size(130, 302);
			this.SystemMsgLog.Multiline = true;
			this.SystemMsgLog.Name = "SystemMsgLog";
			this.SystemMsgLog.ReadOnly = true;
			this.SystemMsgLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.SystemMsgLog.Size = new System.Drawing.Size(130, 302);
			this.SystemMsgLog.TabIndex = 40;
			this.SystemMsgLog.Text = "Have fun!";
			// 
			// label23
			// 
			this.label23.AutoSize = true;
			this.label23.Font = new System.Drawing.Font("MS Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label23.ForeColor = System.Drawing.Color.White;
			this.label23.Location = new System.Drawing.Point(320, 496);
			this.label23.Name = "label23";
			this.label23.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.label23.Size = new System.Drawing.Size(125, 12);
			this.label23.TabIndex = 42;
			this.label23.Text = "ELFBOT - ROSE ONLINE";
			// 
			// hpFoodKeyTimer
			// 
			this.hpFoodKeyTimer.Tick += new System.EventHandler(this.hpFoodKeyTimer_Tick);
			// 
			// mpFoodKeyTimer
			// 
			this.mpFoodKeyTimer.Tick += new System.EventHandler(this.mpFoodKeyTimer_Tick);
			// 
			// CombatOptionsBtn
			// 
			this.CombatOptionsBtn.BackColor = System.Drawing.Color.DimGray;
			this.CombatOptionsBtn.FlatAppearance.BorderSize = 0;
			this.CombatOptionsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.CombatOptionsBtn.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CombatOptionsBtn.Location = new System.Drawing.Point(9, 83);
			this.CombatOptionsBtn.Margin = new System.Windows.Forms.Padding(0);
			this.CombatOptionsBtn.Name = "CombatOptionsBtn";
			this.CombatOptionsBtn.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this.CombatOptionsBtn.Size = new System.Drawing.Size(103, 26);
			this.CombatOptionsBtn.TabIndex = 43;
			this.CombatOptionsBtn.Text = "Combat Options";
			this.CombatOptionsBtn.UseVisualStyleBackColor = false;
			this.CombatOptionsBtn.Click += new System.EventHandler(this.CombatOptionsButton_Click);
			// 
			// LootOptionsBtn
			// 
			this.LootOptionsBtn.BackColor = System.Drawing.Color.DimGray;
			this.LootOptionsBtn.FlatAppearance.BorderSize = 0;
			this.LootOptionsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.LootOptionsBtn.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LootOptionsBtn.Location = new System.Drawing.Point(112, 83);
			this.LootOptionsBtn.Margin = new System.Windows.Forms.Padding(0);
			this.LootOptionsBtn.Name = "LootOptionsBtn";
			this.LootOptionsBtn.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this.LootOptionsBtn.Size = new System.Drawing.Size(103, 26);
			this.LootOptionsBtn.TabIndex = 45;
			this.LootOptionsBtn.Text = "Loot Options";
			this.LootOptionsBtn.UseVisualStyleBackColor = false;
			this.LootOptionsBtn.Click += new System.EventHandler(this.LootOptionsBtn_Click);
			// 
			// FoodOptionsBtn
			// 
			this.FoodOptionsBtn.BackColor = System.Drawing.Color.DimGray;
			this.FoodOptionsBtn.FlatAppearance.BorderSize = 0;
			this.FoodOptionsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.FoodOptionsBtn.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FoodOptionsBtn.Location = new System.Drawing.Point(215, 83);
			this.FoodOptionsBtn.Margin = new System.Windows.Forms.Padding(0);
			this.FoodOptionsBtn.Name = "FoodOptionsBtn";
			this.FoodOptionsBtn.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this.FoodOptionsBtn.Size = new System.Drawing.Size(103, 26);
			this.FoodOptionsBtn.TabIndex = 46;
			this.FoodOptionsBtn.Text = "Food Options";
			this.FoodOptionsBtn.UseVisualStyleBackColor = false;
			this.FoodOptionsBtn.Click += new System.EventHandler(this.FoodOptionsBtn_Click);
			// 
			// CombatOptionsPanel
			// 
			this.CombatOptionsPanel.BackColor = System.Drawing.Color.DimGray;
			this.CombatOptionsPanel.Controls.Add(this.updateCombatKeysBtn);
			this.CombatOptionsPanel.Controls.Add(this.label15);
			this.CombatOptionsPanel.Controls.Add(this.label9);
			this.CombatOptionsPanel.Controls.Add(this.label12);
			this.CombatOptionsPanel.Controls.Add(this.combatShiftKeys);
			this.CombatOptionsPanel.Controls.Add(this.retargetTimeoutInputBox);
			this.CombatOptionsPanel.Controls.Add(this.combatKeys);
			this.CombatOptionsPanel.Controls.Add(this.monsterTablePanel);
			this.CombatOptionsPanel.Controls.Add(this.loadTableButton);
			this.CombatOptionsPanel.Controls.Add(this.monsterTableTitle);
			this.CombatOptionsPanel.Controls.Add(this.combatKeyDelayInputBox);
			this.CombatOptionsPanel.Controls.Add(this.label5);
			this.CombatOptionsPanel.Controls.Add(this.label19);
			this.CombatOptionsPanel.Controls.Add(this.actionDelayInputBox);
			this.CombatOptionsPanel.Controls.Add(this.comboBox3);
			this.CombatOptionsPanel.Controls.Add(this.monsterInputBox);
			this.CombatOptionsPanel.Controls.Add(this.label10);
			this.CombatOptionsPanel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CombatOptionsPanel.Location = new System.Drawing.Point(9, 109);
			this.CombatOptionsPanel.Name = "CombatOptionsPanel";
			this.CombatOptionsPanel.Size = new System.Drawing.Size(306, 0);
			this.CombatOptionsPanel.TabIndex = 48;
			// 
			// LootOptionsPanel
			// 
			this.LootOptionsPanel.BackColor = System.Drawing.Color.DimGray;
			this.LootOptionsPanel.Controls.Add(this.maskedTextBox2);
			this.LootOptionsPanel.Controls.Add(this.checkBox5);
			this.LootOptionsPanel.Controls.Add(this.label13);
			this.LootOptionsPanel.Controls.Add(this.lootTimeInputBox);
			this.LootOptionsPanel.Controls.Add(this.label14);
			this.LootOptionsPanel.Controls.Add(this.combatLootCheckbox);
			this.LootOptionsPanel.Controls.Add(this.checkedListBox2);
			this.LootOptionsPanel.Controls.Add(this.checkedListBox3);
			this.LootOptionsPanel.Controls.Add(this.label6);
			this.LootOptionsPanel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LootOptionsPanel.Location = new System.Drawing.Point(112, 109);
			this.LootOptionsPanel.Name = "LootOptionsPanel";
			this.LootOptionsPanel.Size = new System.Drawing.Size(103, 0);
			this.LootOptionsPanel.TabIndex = 40;
			// 
			// maskedTextBox2
			// 
			this.maskedTextBox2.Enabled = false;
			this.maskedTextBox2.Location = new System.Drawing.Point(9, 89);
			this.maskedTextBox2.Name = "maskedTextBox2";
			this.maskedTextBox2.Size = new System.Drawing.Size(58, 18);
			this.maskedTextBox2.TabIndex = 28;
			// 
			// checkBox5
			// 
			this.checkBox5.AutoSize = true;
			this.checkBox5.Enabled = false;
			this.checkBox5.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.checkBox5.Location = new System.Drawing.Point(10, 68);
			this.checkBox5.Name = "checkBox5";
			this.checkBox5.Size = new System.Drawing.Size(76, 15);
			this.checkBox5.TabIndex = 27;
			this.checkBox5.Text = "Free Loot";
			this.checkBox5.UseVisualStyleBackColor = true;
			// 
			// FoodOptionsPanel
			// 
			this.FoodOptionsPanel.BackColor = System.Drawing.Color.Gray;
			this.FoodOptionsPanel.Controls.Add(this.updateFoodKeysBtn);
			this.FoodOptionsPanel.Controls.Add(this.label24);
			this.FoodOptionsPanel.Controls.Add(this.label25);
			this.FoodOptionsPanel.Controls.Add(this.label3);
			this.FoodOptionsPanel.Controls.Add(this.hpFoodCheckbox);
			this.FoodOptionsPanel.Controls.Add(this.label18);
			this.FoodOptionsPanel.Controls.Add(this.label22);
			this.FoodOptionsPanel.Controls.Add(this.checkedListBox8);
			this.FoodOptionsPanel.Controls.Add(this.mpFoodCheckbox);
			this.FoodOptionsPanel.Controls.Add(this.mpKeys);
			this.FoodOptionsPanel.Controls.Add(this.eatKeyDelayInputBox);
			this.FoodOptionsPanel.Controls.Add(this.label8);
			this.FoodOptionsPanel.Controls.Add(this.label11);
			this.FoodOptionsPanel.Controls.Add(this.mpComboBox);
			this.FoodOptionsPanel.Controls.Add(this.hpKeys);
			this.FoodOptionsPanel.Controls.Add(this.label21);
			this.FoodOptionsPanel.Controls.Add(this.checkedListBox5);
			this.FoodOptionsPanel.Controls.Add(this.hpComboBox);
			this.FoodOptionsPanel.Controls.Add(this.foodDelayInputBox);
			this.FoodOptionsPanel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FoodOptionsPanel.Location = new System.Drawing.Point(9, 109);
			this.FoodOptionsPanel.Name = "FoodOptionsPanel";
			this.FoodOptionsPanel.Size = new System.Drawing.Size(309, 0);
			this.FoodOptionsPanel.TabIndex = 49;
			// 
			// MorePanelsBtn
			// 
			this.MorePanelsBtn.BackColor = System.Drawing.Color.DimGray;
			this.MorePanelsBtn.FlatAppearance.BorderSize = 0;
			this.MorePanelsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.MorePanelsBtn.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MorePanelsBtn.Location = new System.Drawing.Point(318, 83);
			this.MorePanelsBtn.Margin = new System.Windows.Forms.Padding(0);
			this.MorePanelsBtn.Name = "MorePanelsBtn";
			this.MorePanelsBtn.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this.MorePanelsBtn.Size = new System.Drawing.Size(127, 26);
			this.MorePanelsBtn.TabIndex = 50;
			this.MorePanelsBtn.Text = "More >";
			this.MorePanelsBtn.UseVisualStyleBackColor = false;
			this.MorePanelsBtn.Click += new System.EventHandler(this.MorePanelsBtn_Click);
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.DimGray;
			this.panel2.Controls.Add(this.CameraHeaderLabel);
			this.panel2.Controls.Add(this.MiscHeaderLabel);
			this.panel2.Controls.Add(this.PlayerMPLabel);
			this.panel2.Controls.Add(this.PlayerHPLabel);
			this.panel2.Controls.Add(this.PlayerStatusHeaderLabel);
			this.panel2.Controls.Add(this.PlayerMapIdLabel);
			this.panel2.Controls.Add(this.PlayerPosZLabel);
			this.panel2.Controls.Add(this.PlayerPosYLabel);
			this.panel2.Controls.Add(this.PlayerPosXLabel);
			this.panel2.Controls.Add(this.LocationHeaderLabel);
			this.panel2.Controls.Add(this.PlayerZulyLabel);
			this.panel2.Controls.Add(this.PlayerLevelLabel);
			this.panel2.Controls.Add(this.PlayerNameLabel);
			this.panel2.Controls.Add(this.CameraYawLabel);
			this.panel2.Controls.Add(this.CameraPitchLabel);
			this.panel2.Controls.Add(this.CameraZoomLabel);
			this.panel2.Controls.Add(this.button2);
			this.panel2.Controls.Add(this.CharacterHeaderLabel);
			this.panel2.Controls.Add(this.TargetUIDLabel);
			this.panel2.Controls.Add(this.TargetLabel);
			this.panel2.Controls.Add(this.XPBeforeKillLabel);
			this.panel2.Controls.Add(this.CurrentXPLabel);
			this.panel2.Location = new System.Drawing.Point(451, 9);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(158, 543);
			this.panel2.TabIndex = 51;
			this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
			// 
			// MiscHeaderLabel
			// 
			this.MiscHeaderLabel.AutoSize = true;
			this.MiscHeaderLabel.Font = new System.Drawing.Font("MS Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MiscHeaderLabel.Location = new System.Drawing.Point(2, 340);
			this.MiscHeaderLabel.Name = "MiscHeaderLabel";
			this.MiscHeaderLabel.Size = new System.Drawing.Size(35, 13);
			this.MiscHeaderLabel.TabIndex = 30;
			this.MiscHeaderLabel.Text = "Misc";
			this.MiscHeaderLabel.Click += new System.EventHandler(this.label27_Click);
			// 
			// PlayerMPLabel
			// 
			this.PlayerMPLabel.AutoSize = true;
			this.PlayerMPLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PlayerMPLabel.Location = new System.Drawing.Point(12, 240);
			this.PlayerMPLabel.Name = "PlayerMPLabel";
			this.PlayerMPLabel.Size = new System.Drawing.Size(59, 11);
			this.PlayerMPLabel.TabIndex = 29;
			this.PlayerMPLabel.Text = "MP: - / -";
			// 
			// PlayerHPLabel
			// 
			this.PlayerHPLabel.AutoSize = true;
			this.PlayerHPLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PlayerHPLabel.Location = new System.Drawing.Point(12, 223);
			this.PlayerHPLabel.Name = "PlayerHPLabel";
			this.PlayerHPLabel.Size = new System.Drawing.Size(59, 11);
			this.PlayerHPLabel.TabIndex = 28;
			this.PlayerHPLabel.Text = "HP: - / -";
			// 
			// PlayerStatusHeaderLabel
			// 
			this.PlayerStatusHeaderLabel.AutoSize = true;
			this.PlayerStatusHeaderLabel.Font = new System.Drawing.Font("MS Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PlayerStatusHeaderLabel.Location = new System.Drawing.Point(2, 201);
			this.PlayerStatusHeaderLabel.Name = "PlayerStatusHeaderLabel";
			this.PlayerStatusHeaderLabel.Size = new System.Drawing.Size(49, 13);
			this.PlayerStatusHeaderLabel.TabIndex = 27;
			this.PlayerStatusHeaderLabel.Text = "Status";
			// 
			// PlayerMapIdLabel
			// 
			this.PlayerMapIdLabel.AutoSize = true;
			this.PlayerMapIdLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PlayerMapIdLabel.Location = new System.Drawing.Point(12, 179);
			this.PlayerMapIdLabel.Name = "PlayerMapIdLabel";
			this.PlayerMapIdLabel.Size = new System.Drawing.Size(71, 11);
			this.PlayerMapIdLabel.TabIndex = 26;
			this.PlayerMapIdLabel.Text = "Map ID: N/A";
			// 
			// PlayerPosZLabel
			// 
			this.PlayerPosZLabel.AutoSize = true;
			this.PlayerPosZLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PlayerPosZLabel.Location = new System.Drawing.Point(12, 162);
			this.PlayerPosZLabel.Name = "PlayerPosZLabel";
			this.PlayerPosZLabel.Size = new System.Drawing.Size(41, 11);
			this.PlayerPosZLabel.TabIndex = 25;
			this.PlayerPosZLabel.Text = "Z: N/A";
			// 
			// PlayerPosYLabel
			// 
			this.PlayerPosYLabel.AutoSize = true;
			this.PlayerPosYLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PlayerPosYLabel.Location = new System.Drawing.Point(12, 145);
			this.PlayerPosYLabel.Name = "PlayerPosYLabel";
			this.PlayerPosYLabel.Size = new System.Drawing.Size(41, 11);
			this.PlayerPosYLabel.TabIndex = 24;
			this.PlayerPosYLabel.Text = "Y: N/A";
			// 
			// PlayerPosXLabel
			// 
			this.PlayerPosXLabel.AutoSize = true;
			this.PlayerPosXLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PlayerPosXLabel.Location = new System.Drawing.Point(12, 128);
			this.PlayerPosXLabel.Name = "PlayerPosXLabel";
			this.PlayerPosXLabel.Size = new System.Drawing.Size(41, 11);
			this.PlayerPosXLabel.TabIndex = 23;
			this.PlayerPosXLabel.Text = "X: N/A";
			// 
			// LocationHeaderLabel
			// 
			this.LocationHeaderLabel.AutoSize = true;
			this.LocationHeaderLabel.Font = new System.Drawing.Font("MS Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LocationHeaderLabel.Location = new System.Drawing.Point(2, 106);
			this.LocationHeaderLabel.Name = "LocationHeaderLabel";
			this.LocationHeaderLabel.Size = new System.Drawing.Size(63, 13);
			this.LocationHeaderLabel.TabIndex = 22;
			this.LocationHeaderLabel.Text = "Location";
			// 
			// PlayerZulyLabel
			// 
			this.PlayerZulyLabel.AutoSize = true;
			this.PlayerZulyLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PlayerZulyLabel.Location = new System.Drawing.Point(12, 84);
			this.PlayerZulyLabel.Name = "PlayerZulyLabel";
			this.PlayerZulyLabel.Size = new System.Drawing.Size(59, 11);
			this.PlayerZulyLabel.TabIndex = 21;
			this.PlayerZulyLabel.Text = "Zuly: N/A";
			// 
			// PlayerLevelLabel
			// 
			this.PlayerLevelLabel.AutoSize = true;
			this.PlayerLevelLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PlayerLevelLabel.Location = new System.Drawing.Point(12, 50);
			this.PlayerLevelLabel.Name = "PlayerLevelLabel";
			this.PlayerLevelLabel.Size = new System.Drawing.Size(65, 11);
			this.PlayerLevelLabel.TabIndex = 20;
			this.PlayerLevelLabel.Text = "Level: N/A";
			// 
			// PlayerNameLabel
			// 
			this.PlayerNameLabel.AutoSize = true;
			this.PlayerNameLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.PlayerNameLabel.Location = new System.Drawing.Point(12, 33);
			this.PlayerNameLabel.Name = "PlayerNameLabel";
			this.PlayerNameLabel.Size = new System.Drawing.Size(59, 11);
			this.PlayerNameLabel.TabIndex = 19;
			this.PlayerNameLabel.Text = "Name: N/A";
			this.PlayerNameLabel.Click += new System.EventHandler(this.PlayerNameLabel_Click);
			// 
			// CameraYawLabel
			// 
			this.CameraYawLabel.AutoSize = true;
			this.CameraYawLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.CameraYawLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CameraYawLabel.Location = new System.Drawing.Point(12, 318);
			this.CameraYawLabel.Name = "CameraYawLabel";
			this.CameraYawLabel.Size = new System.Drawing.Size(31, 13);
			this.CameraYawLabel.TabIndex = 22;
			this.CameraYawLabel.Text = "Yaw:";
			// 
			// CameraPitchLabel
			// 
			this.CameraPitchLabel.AutoSize = true;
			this.CameraPitchLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.CameraPitchLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CameraPitchLabel.Location = new System.Drawing.Point(12, 301);
			this.CameraPitchLabel.Name = "CameraPitchLabel";
			this.CameraPitchLabel.Size = new System.Drawing.Size(43, 13);
			this.CameraPitchLabel.TabIndex = 21;
			this.CameraPitchLabel.Text = "Pitch:";
			// 
			// CameraZoomLabel
			// 
			this.CameraZoomLabel.AutoSize = true;
			this.CameraZoomLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.CameraZoomLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CameraZoomLabel.Location = new System.Drawing.Point(12, 284);
			this.CameraZoomLabel.Name = "CameraZoomLabel";
			this.CameraZoomLabel.Size = new System.Drawing.Size(37, 13);
			this.CameraZoomLabel.TabIndex = 20;
			this.CameraZoomLabel.Text = "Zoom:";
			// 
			// button2
			// 
			this.button2.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button2.Location = new System.Drawing.Point(0, 523);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(83, 20);
			this.button2.TabIndex = 18;
			this.button2.Text = "FLY UP";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// cameraTimer
			// 
			this.cameraTimer.Tick += new System.EventHandler(this.cameraTimer_Tick);
			// 
			// CameraOptionsBtn
			// 
			this.CameraOptionsBtn.BackColor = System.Drawing.Color.DimGray;
			this.CameraOptionsBtn.FlatAppearance.BorderSize = 0;
			this.CameraOptionsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.CameraOptionsBtn.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CameraOptionsBtn.Location = new System.Drawing.Point(9, 83);
			this.CameraOptionsBtn.Margin = new System.Windows.Forms.Padding(0);
			this.CameraOptionsBtn.Name = "CameraOptionsBtn";
			this.CameraOptionsBtn.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this.CameraOptionsBtn.Size = new System.Drawing.Size(103, 0);
			this.CameraOptionsBtn.TabIndex = 52;
			this.CameraOptionsBtn.Text = "Camera Options";
			this.CameraOptionsBtn.UseVisualStyleBackColor = false;
			// 
			// CameraOptionsPanel
			// 
			this.CameraOptionsPanel.BackColor = System.Drawing.Color.DimGray;
			this.CameraOptionsPanel.Controls.Add(this.TimedCameraYawCheckbox);
			this.CameraOptionsPanel.Controls.Add(this.ForceTopdownCheckbox);
			this.CameraOptionsPanel.Controls.Add(this.ForceMaxZoomCheckbox);
			this.CameraOptionsPanel.Controls.Add(this.CameraYawDelayLabel);
			this.CameraOptionsPanel.Controls.Add(this.CameraYawDelayInputbox);
			this.CameraOptionsPanel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CameraOptionsPanel.Location = new System.Drawing.Point(9, 109);
			this.CameraOptionsPanel.Name = "CameraOptionsPanel";
			this.CameraOptionsPanel.Size = new System.Drawing.Size(103, 0);
			this.CameraOptionsPanel.TabIndex = 53;
			// 
			// TimedCameraYawCheckbox
			// 
			this.TimedCameraYawCheckbox.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TimedCameraYawCheckbox.Location = new System.Drawing.Point(4, 94);
			this.TimedCameraYawCheckbox.Name = "TimedCameraYawCheckbox";
			this.TimedCameraYawCheckbox.Size = new System.Drawing.Size(74, 40);
			this.TimedCameraYawCheckbox.TabIndex = 41;
			this.TimedCameraYawCheckbox.Text = "Timed Camera Yaw";
			this.TimedCameraYawCheckbox.UseVisualStyleBackColor = true;
			this.TimedCameraYawCheckbox.CheckedChanged += new System.EventHandler(this.TimedCameraYawCheckbox_CheckedChanged);
			// 
			// ForceTopdownCheckbox
			// 
			this.ForceTopdownCheckbox.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ForceTopdownCheckbox.Location = new System.Drawing.Point(4, 49);
			this.ForceTopdownCheckbox.Name = "ForceTopdownCheckbox";
			this.ForceTopdownCheckbox.Size = new System.Drawing.Size(74, 40);
			this.ForceTopdownCheckbox.TabIndex = 40;
			this.ForceTopdownCheckbox.Text = "Force Camera Top Down";
			this.ForceTopdownCheckbox.UseVisualStyleBackColor = true;
			this.ForceTopdownCheckbox.CheckedChanged += new System.EventHandler(this.ForceTopdownCheckbox_CheckedChanged);
			// 
			// CameraYawDelayLabel
			// 
			this.CameraYawDelayLabel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CameraYawDelayLabel.Location = new System.Drawing.Point(5, 137);
			this.CameraYawDelayLabel.Name = "CameraYawDelayLabel";
			this.CameraYawDelayLabel.Size = new System.Drawing.Size(73, 23);
			this.CameraYawDelayLabel.TabIndex = 44;
			this.CameraYawDelayLabel.Text = "Camera Yaw Delay";
			// 
			// CameraYawDelayInputbox
			// 
			this.CameraYawDelayInputbox.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CameraYawDelayInputbox.Location = new System.Drawing.Point(7, 163);
			this.CameraYawDelayInputbox.Name = "CameraYawDelayInputbox";
			this.CameraYawDelayInputbox.Size = new System.Drawing.Size(65, 18);
			this.CameraYawDelayInputbox.TabIndex = 43;
			this.CameraYawDelayInputbox.Text = "8";
			this.CameraYawDelayInputbox.TextChanged += new System.EventHandler(this.CameraYawDelayInputbox_InputChanged);
			// 
			// CameraYawTimer
			// 
			this.CameraYawTimer.Tick += new System.EventHandler(this.CameraYawTimer_Tick);
			// 
			// ZHackPanel
			// 
			this.ZHackPanel.BackColor = System.Drawing.Color.DimGray;
			this.ZHackPanel.Controls.Add(this.TimedZHackAmountInputbox);
			this.ZHackPanel.Controls.Add(this.TimedZHackAmountLabel);
			this.ZHackPanel.Controls.Add(this.TimedZHackDelayLabel);
			this.ZHackPanel.Controls.Add(this.TimedZHackDelayInputbox);
			this.ZHackPanel.Controls.Add(this.checkBox1);
			this.ZHackPanel.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ZHackPanel.Location = new System.Drawing.Point(112, 109);
			this.ZHackPanel.Name = "ZHackPanel";
			this.ZHackPanel.Size = new System.Drawing.Size(103, 0);
			this.ZHackPanel.TabIndex = 54;
			// 
			// TimedZHackAmountInputbox
			// 
			this.TimedZHackAmountInputbox.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TimedZHackAmountInputbox.Location = new System.Drawing.Point(10, 105);
			this.TimedZHackAmountInputbox.Name = "TimedZHackAmountInputbox";
			this.TimedZHackAmountInputbox.Size = new System.Drawing.Size(65, 18);
			this.TimedZHackAmountInputbox.TabIndex = 42;
			// 
			// TimedZHackAmountLabel
			// 
			this.TimedZHackAmountLabel.AutoSize = true;
			this.TimedZHackAmountLabel.Location = new System.Drawing.Point(8, 86);
			this.TimedZHackAmountLabel.Name = "TimedZHackAmountLabel";
			this.TimedZHackAmountLabel.Size = new System.Drawing.Size(77, 11);
			this.TimedZHackAmountLabel.TabIndex = 41;
			this.TimedZHackAmountLabel.Text = "ZHack Amount";
			// 
			// TimedZHackDelayLabel
			// 
			this.TimedZHackDelayLabel.AutoSize = true;
			this.TimedZHackDelayLabel.Location = new System.Drawing.Point(8, 38);
			this.TimedZHackDelayLabel.Name = "TimedZHackDelayLabel";
			this.TimedZHackDelayLabel.Size = new System.Drawing.Size(71, 11);
			this.TimedZHackDelayLabel.TabIndex = 40;
			this.TimedZHackDelayLabel.Text = "ZHack Delay";
			// 
			// TimedZHackDelayInputbox
			// 
			this.TimedZHackDelayInputbox.Font = new System.Drawing.Font("MS Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TimedZHackDelayInputbox.Location = new System.Drawing.Point(9, 57);
			this.TimedZHackDelayInputbox.Name = "TimedZHackDelayInputbox";
			this.TimedZHackDelayInputbox.Size = new System.Drawing.Size(65, 18);
			this.TimedZHackDelayInputbox.TabIndex = 39;
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(8, 13);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(90, 15);
			this.checkBox1.TabIndex = 0;
			this.checkBox1.Text = "Timed ZHack";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// CameraHeaderLabel
			// 
			this.CameraHeaderLabel.AutoSize = true;
			this.CameraHeaderLabel.Font = new System.Drawing.Font("MS Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CameraHeaderLabel.Location = new System.Drawing.Point(2, 262);
			this.CameraHeaderLabel.Name = "CameraHeaderLabel";
			this.CameraHeaderLabel.Size = new System.Drawing.Size(49, 13);
			this.CameraHeaderLabel.TabIndex = 31;
			this.CameraHeaderLabel.Text = "Camera";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(619, 559);
			this.Controls.Add(this.ZHackPanel);
			this.Controls.Add(this.CameraOptionsPanel);
			this.Controls.Add(this.CameraOptionsBtn);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.MorePanelsBtn);
			this.Controls.Add(this.FoodOptionsPanel);
			this.Controls.Add(this.LootOptionsPanel);
			this.Controls.Add(this.CombatOptionsPanel);
			this.Controls.Add(this.FoodOptionsBtn);
			this.Controls.Add(this.LootOptionsBtn);
			this.Controls.Add(this.CombatOptionsBtn);
			this.Controls.Add(this.label23);
			this.Controls.Add(this.SystemMsgLog);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.ProcessHookLabel);
			this.Controls.Add(this.hookButton);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "ElfBot";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.monsterTablePanel.ResumeLayout(false);
			this.monsterTablePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.CombatOptionsPanel.ResumeLayout(false);
			this.CombatOptionsPanel.PerformLayout();
			this.LootOptionsPanel.ResumeLayout(false);
			this.LootOptionsPanel.PerformLayout();
			this.FoodOptionsPanel.ResumeLayout(false);
			this.FoodOptionsPanel.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.CameraOptionsPanel.ResumeLayout(false);
			this.CameraOptionsPanel.PerformLayout();
			this.ZHackPanel.ResumeLayout(false);
			this.ZHackPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.CheckBox AutoCombatBox;
		private System.Windows.Forms.Timer lootingEndTimer;
		private System.Windows.Forms.Label CurrentXPLabel;
		private System.Windows.Forms.Label XPBeforeKillLabel;
		private System.Windows.Forms.Label AutoCombatState;
		private System.Windows.Forms.Label TargetLabel;
		private System.Windows.Forms.Label TargetUIDLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label ProcessHookLabel;
		private System.Windows.Forms.MaskedTextBox monsterInputBox;
		private System.Windows.Forms.Label monsterTableTitle;
		private System.Windows.Forms.Panel monsterTablePanel;
		private System.Windows.Forms.Label monsterTableText;
		private System.Windows.Forms.Button hookButton;
		private System.Windows.Forms.Label CharacterHeaderLabel;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.MaskedTextBox lootTimeInputBox;
		private System.Windows.Forms.MaskedTextBox actionDelayInputBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox combatLootCheckbox;
		private System.Windows.Forms.CheckedListBox combatKeys;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.CheckedListBox combatShiftKeys;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckedListBox checkedListBox5;
		private System.Windows.Forms.CheckedListBox hpKeys;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.CheckedListBox checkedListBox3;
		private System.Windows.Forms.CheckedListBox checkedListBox2;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.CheckedListBox checkedListBox8;
		private System.Windows.Forms.CheckedListBox mpKeys;
		private System.Windows.Forms.ComboBox mpComboBox;
		private System.Windows.Forms.CheckBox mpFoodCheckbox;
		private System.Windows.Forms.CheckBox hpFoodCheckbox;
		private System.Windows.Forms.ComboBox hpComboBox;
		private System.Windows.Forms.Timer combatTimer;
		private System.Windows.Forms.Timer targettingTimer;
		private System.Windows.Forms.Timer checkTimer;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.ComboBox comboBox3;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.MaskedTextBox retargetTimeoutInputBox;
		private System.Windows.Forms.Button updateCombatKeysBtn;
		private System.Windows.Forms.MaskedTextBox combatKeyDelayInputBox;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Timer attackTimeoutTimer;
		private System.Windows.Forms.Timer lootingTimer;
		private System.Windows.Forms.Timer interfaceTimer;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Timer resetCombatTimer;
		private System.Windows.Forms.Button loadTableButton;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.TextBox SystemMsgLog;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.CheckBox ForceMaxZoomCheckbox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button updateFoodKeysBtn;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.MaskedTextBox foodDelayInputBox;
		private System.Windows.Forms.Timer hpFoodTimer;
		private System.Windows.Forms.Timer mpFoodTimer;
		private System.Windows.Forms.Timer hpFoodKeyTimer;
		private System.Windows.Forms.Timer mpFoodKeyTimer;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.MaskedTextBox eatKeyDelayInputBox;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.CheckBox checkBox4;
		private System.Windows.Forms.Button CombatOptionsBtn;
		private System.Windows.Forms.Button LootOptionsBtn;
		private System.Windows.Forms.Button FoodOptionsBtn;
		private System.Windows.Forms.Panel CombatOptionsPanel;
		private System.Windows.Forms.Panel LootOptionsPanel;
		private System.Windows.Forms.Panel FoodOptionsPanel;
		private System.Windows.Forms.MaskedTextBox maskedTextBox2;
		private System.Windows.Forms.CheckBox checkBox5;
		private System.Windows.Forms.Button MorePanelsBtn;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label PlayerNameLabel;
		private System.Windows.Forms.Label PlayerLevelLabel;
		private System.Windows.Forms.Label PlayerMPLabel;
		private System.Windows.Forms.Label PlayerHPLabel;
		private System.Windows.Forms.Label PlayerStatusHeaderLabel;
		private System.Windows.Forms.Label PlayerMapIdLabel;
		private System.Windows.Forms.Label PlayerPosZLabel;
		private System.Windows.Forms.Label PlayerPosYLabel;
		private System.Windows.Forms.Label PlayerPosXLabel;
		private System.Windows.Forms.Label LocationHeaderLabel;
		private System.Windows.Forms.Label PlayerZulyLabel;
		private System.Windows.Forms.Label MiscHeaderLabel;
		private System.Windows.Forms.Timer cameraTimer;
		private System.Windows.Forms.Button CameraOptionsBtn;
		private System.Windows.Forms.Panel CameraOptionsPanel;
		private System.Windows.Forms.CheckBox ForceTopdownCheckbox;
		private System.Windows.Forms.Timer CameraYawTimer;
		private System.Windows.Forms.CheckBox SecondClientCheckbox;
		private System.Windows.Forms.Label CameraYawLabel;
		private System.Windows.Forms.Label CameraPitchLabel;
		private System.Windows.Forms.Label CameraZoomLabel;
		private System.Windows.Forms.Panel ZHackPanel;
		private System.Windows.Forms.MaskedTextBox TimedZHackDelayInputbox;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.MaskedTextBox TimedZHackAmountInputbox;
		private System.Windows.Forms.Label TimedZHackAmountLabel;
		private System.Windows.Forms.Label TimedZHackDelayLabel;
		private System.Windows.Forms.CheckBox TimedCameraYawCheckbox;
		private System.Windows.Forms.Label CameraYawDelayLabel;
		private System.Windows.Forms.MaskedTextBox CameraYawDelayInputbox;
        private System.Windows.Forms.Label CameraHeaderLabel;
    }
}

