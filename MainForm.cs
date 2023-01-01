namespace ElfBot
{
	using System;
	using System.Linq;
	using System.ComponentModel;
	using System.Windows.Forms;
	using Memory;
	using WindowsInput.Native;
	using WindowsInput;

	using AddressDict = System.Collections.Generic.Dictionary<string, string>;
	using OptionDict = System.Collections.Generic.Dictionary<string, bool>;
	using MonsterHashTable = System.Collections.Generic.HashSet<string>;
	using MonsterList = System.Collections.Generic.List<string>;

	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		#region Members

		public static AddressDict Addresses = new AddressDict()
		{
			{ "CurrentTarget" , "trose.exe+10D8C10" },
			{ "CurrentXP" , "trose.exe+10BCA14" },
			{ "TargetUID" , "trose.exe+10C0458,0x8"},
		};

		public static OptionDict BotOptions = new OptionDict()
		{
			{ "AutoCombat" , false },
			{ "AutoCombatLoot" , false },
		};

		public static MonsterHashTable MonsterTable;

		private CombatStates _combatState;
		private Mem m;
		private InputSimulator sim;

		private int _currentTargetUID = -1;
		private int _currentXP = 0;
		private int _xpBeforeKill = -1;
		private float _lootForSeconds = 4f;
		private float _actionDelay = 1f;
		private string _currentTarget = "";

		#endregion

		#region Init Methods

		private void Form1_Load(object sender, EventArgs e)
		{
			m = new Mem();
			sim = new InputSimulator();

			MonsterTable = new MonsterHashTable();
			_combatState = CombatStates.Targetting;
			AutoCombatBox.Enabled = false;

			// if worker threads are needed, this may be useful
			// if (!workerVar.IsBusy) { workerVar.RunWorkerAsync(); }
		}

		private bool TryOpenProcess()
		{
			int pID = m.GetProcIdFromName("trose");

			if (pID > 0)
			{
				m.OpenProcess(pID);
				ProcessHookLabel.Text = "Process Hooked!";
				ProcessHookLabel.ForeColor = System.Drawing.Color.LimeGreen;

				AutoCombatBox.Enabled = true;

				return true;
			}

			ProcessHookLabel.Text = "Process Hook Failed :(";
			ProcessHookLabel.ForeColor = System.Drawing.Color.Red;
			return false;
		}

		private void ListenToTimer(Timer timer, int msDelay, EventHandler del)
		{
			timer.Interval = msDelay;
			timer.Tick += new EventHandler(del);
			timer.Enabled = true;
		}

		#endregion

		private void monsterAddBtn_Click(object sender, EventArgs e)
		{
			string inputTxt = monsterInputBox.Text;
			if (MonsterTable.Contains(inputTxt))
			{
				monsterInputBox.Text = "";
				return;
			}

			if(MonsterTable.Count == 0)
			{
				monsterTableText.Text = "";
			}

			MonsterTable.Add(inputTxt);
			monsterInputBox.Text = "";
			RebuildMonsterTable();
		}

		private void monsterRemoveBtn_Click(object sender, EventArgs e)
		{
			string inputTxt = monsterInputBox.Text;
			monsterInputBox.Text = "";
			if (MonsterTable.Contains(inputTxt))
			{
				MonsterTable.Remove(inputTxt);
				RebuildMonsterTable();
			}
		}

		private void RebuildMonsterTable()
		{
			MonsterList monsterList = new MonsterList(MonsterTable);
			monsterTableText.Text = "";

			for (int i = 0; i < monsterList.Count; i++)
			{
				monsterTableText.Text += monsterList[i] + "\n";
			}

			if (monsterTableText.Text.Length == 0)
			{
				monsterTableText.Text = "Empty";
			}
		}

		private void hookButton_Click(object sender, EventArgs e)
		{
			if (!TryOpenProcess()) { return; }  // TODO: add error to fail process open
		}

		private void lootTimeInputBox_InputChanged(object sender, EventArgs e)
		{
			float result = 0f;

			if(float.TryParse(lootTimeInputBox.Text, out result))
			{
				_lootForSeconds = result;
			}
		}

		private void actionDelayInputBox_InputChanged(object sender, EventArgs e)
		{
			float result = 0f;

			if (float.TryParse(actionDelayInputBox.Text, out result))
			{
				_actionDelay = result;
			}
		}
	}
}
