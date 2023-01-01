namespace ElfBot
{
	using System;
	using System.ComponentModel;
	using System.Windows.Forms;
	using Memory;
	using WindowsInput.Native;
	using WindowsInput;

	using AddressDictionary = System.Collections.Generic.Dictionary<string, string>;

	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		#region Members

		public static AddressDictionary Addresses = new AddressDictionary()
		{
			{ "CurrentTarget" , "trose.exe+10D8C10" },
			{ "CurrentXP" , "trose.exe+10BCA14" },
			{ "TargetUID" , "trose.exe+10C0458,0x8"},
		};

		private CombatStates _combatState;
		private Mem m;
		private InputSimulator sim;

		private int _currentTargetUID = -1;
		private int _currentXP = 0;
		private int _xpBeforeKill = -1;
		private float _lootForSeconds = 4f; // TODO: use with form element
		private string _currentTarget = "";

		#endregion

		#region Init Methods

		private void Form1_Load(object sender, EventArgs e)
		{
			m = new Mem();
			sim = new InputSimulator();

			_combatState = CombatStates.Targetting;

			if (!TryOpenProcess()) { return; }  // TODO: add error to fail process open

			// if worker threads are needed, this may be useful
			// if (!workerVar.IsBusy) { workerVar.RunWorkerAsync(); }
		}

		private bool TryOpenProcess()
		{
			int pID = m.GetProcIdFromName("trose");

			if (pID > 0)
			{
				m.OpenProcess(pID);
				return true;
			}

			return false;
		}

		#endregion

		#region Tick Methods

		private void jellybeanTimer_Tick(object sender, EventArgs e)
		{
			if(_combatState == CombatStates.Attacking && !attackingTimer.Enabled)
			{
				attackingTimer.Interval = 1000;
				attackingTimer.Tick += new EventHandler(attacking_Tick);
				attackingTimer.Enabled = true;
			}

			if(_combatState == CombatStates.Targetting && !targettingTimer.Enabled)
			{
				targettingTimer.Interval = 1000;
				targettingTimer.Tick += new EventHandler(targetting_Tick);
				targettingTimer.Enabled = true;
			}


			if (_combatState == CombatStates.CheckingTarget && !targettingTimer.Enabled)
			{
				targettingTimer.Interval = 1000;
				targettingTimer.Tick += new EventHandler(checkingTarget_Tick);
				targettingTimer.Enabled = true;
			}

			if (_combatState == CombatStates.Looting)
			{
				AutoJellyBeanState.Text = "Looting";
				sim.Keyboard.KeyPress(VirtualKeyCode.VK_4);
			}

			TargetLabel.Text = _currentTarget;
			TargetUIDLabel.Text = _currentTargetUID.ToString();
			AutoJellyBeanState.Text = _combatState.ToString();
		}

		private void targetting_Tick(object sender, EventArgs e)
		{
			AutoJellyBeanState.Text = _combatState.ToString();

			// unassign timer event
			targettingTimer.Tick -= targetting_Tick;
			targettingTimer.Enabled = false;

			// if current target doesnt equal jelly bean or there is no unique target id
			if (_currentTarget != "Mini-Jelly Bean" || _currentTargetUID == 0)
			{
				// press targetting key
				sim.Keyboard.KeyPress(VirtualKeyCode.TAB);

				// go into checking target mode to make sure the tab target was OK
				_combatState = CombatStates.CheckingTarget;

				// update XP values
				_currentXP = m.ReadInt(Addresses["CurrentXP"]);
				CurrentXPLabel.Text = "Current XP: " + _currentXP.ToString();
				_xpBeforeKill = _currentXP;
				CurrentXPLabel.Text = "XP Before Kill: " + _xpBeforeKill.ToString();
				return;
			}

			_combatState = CombatStates.CheckingTarget;
		}

		private void checkingTarget_Tick(object sender, EventArgs e)
		{
			AutoJellyBeanState.Text = _combatState.ToString();

			// get target memory
			_currentTarget = m.ReadString(Addresses["CurrentTarget"]);
			_currentTargetUID = m.ReadInt(Addresses["TargetUID"]);

			// unassign timer event
			targettingTimer.Tick -= checkingTarget_Tick;
			targettingTimer.Enabled = false;

			// if current target is mini jelly bean
			if (_currentTarget == "Mini-Jelly Bean")
			{
				// go into attack state
				Console.WriteLine("Found Target");
				_combatState = CombatStates.Attacking;
				return;
			}

			_combatState = CombatStates.Targetting;
		}

		private void attacking_Tick(object sender, EventArgs e)
		{
			AutoJellyBeanState.Text = _combatState.ToString();

			_currentXP = m.ReadInt(Addresses["CurrentXP"]); // get current xp
			_currentTarget = m.ReadString(Addresses["CurrentTarget"]); // make sure we are on the target we want.
			_currentTargetUID = m.ReadInt(Addresses["TargetUID"]);

			if (_currentTarget == "Mini-Jelly Bean")
			{
				// if current xp is greater than our xp while targetting
				if (_currentXP > _xpBeforeKill)
				{
					// enemy has died, loot now and start the loot timer
					_combatState = CombatStates.Looting;
					lootTimer.Interval = 4000;
					lootTimer.Tick += new EventHandler(lootEnd_Tick);
					lootTimer.Enabled = true;
				}

				if(_currentTargetUID != 0)
				{
					sim.Keyboard.KeyPress(VirtualKeyCode.VK_1); // attack press
				}
			}

			// no proper target while attacking
			if(_currentTargetUID == 0)
			{
				// back to targetting
				_combatState = CombatStates.Targetting;
			}

			// if the current XP is less than the xp before the last kill, then the char leveled up
			if(_currentXP < _xpBeforeKill)
			{
				// reset the xp before kill to -1
				_xpBeforeKill = -1;
				// back to targetting
				_combatState = CombatStates.Targetting;
			}

			// unassign timer event
			attackingTimer.Tick -= attacking_Tick;
			attackingTimer.Enabled = false;
		}

		private void lootEnd_Tick(object sender, EventArgs e)
		{
			AutoJellyBeanState.Text = _combatState.ToString();

			// finished looting, unassign timer event
			lootTimer.Tick -= new EventHandler(lootEnd_Tick);
			lootTimer.Enabled = false;

			// go back to targetting
			_currentTarget = "";
			_combatState = CombatStates.Targetting;
		}

		private void AutoJellyBeanBox_CheckedChanged(object sender, EventArgs e)
		{
			if (!AutoJellyBeanBox.Checked && jellybeanTimer.Enabled)
			{
				jellybeanTimer.Tick -= jellybeanTimer_Tick;
				jellybeanTimer.Enabled = false;
				AutoJellyBeanLabel.Text = "DISABLED";
				_currentTarget = "None";
				_combatState = CombatStates.Targetting;
				Console.WriteLine("Disabled AutoJellyBean Timer.");
				return;
			}

			AutoJellyBeanLabel.Text = "ENABLED";
			jellybeanTimer.Interval = 1000;
			jellybeanTimer.Tick += new EventHandler(jellybeanTimer_Tick);
			jellybeanTimer.Enabled = true;
			_xpBeforeKill = -1;
			_combatState = CombatStates.Targetting;
			Console.WriteLine("Enabled AutoJellyBean Timer.");
		}

		#endregion
	}
}
