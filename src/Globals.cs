using Trace = System.Diagnostics.Trace;
using VirtualKeyCode = WindowsInput.Native.VirtualKeyCode;

namespace ElfBot
{
    using Mem = Memory.Mem;
	using ConfigKeyEntryDict = System.Collections.Generic.Dictionary<string, VirtualKeyCode>;
	using System.Windows.Controls;

	public static class Globals
	{

		public static readonly Mem TargetApplicationMemory = new Mem();
        public static bool Hooked = false;
        public static readonly Logger Logger = new Logger();

		public static string Key_CombatCamera = "CombatCamera";
		public static string Key_CameraYawWave = "CameraYawWave";
		public static string Key_AutoHP = "AutoHP";
		public static string Key_AutoMP = "AutoMP";
		public static string Key_CombatLoot = "CombatLoot";
		public static string Key_CombatActionDelay = "CombatActionDelay";
		public static string Key_CombatKeyDelay = "CombatKeyDelay";
		public static string Key_RetargetTimeout = "RetargetTimeout";
		public static string Key_CombatLootTime = "CombatLootTime";
		public static string Key_HpPercent = "HpPercent";
		public static string Key_MpPercent = "MpPercent";
		public static string Key_FoodKeyDelay = "FoodKeyDelay";
		public static string Key_FoodDelay = "FoodDelay";
		public static string Key_CombatKeys = "CombatKeys";
		public static string Key_HPKeys = "HPKeys";
		public static string Key_MPKeys = "MPKeys";

		public static readonly ConfigKeyEntryDict ConfigKeyEntries = new ConfigKeyEntryDict()
        {
            { "VK_1", VirtualKeyCode.VK_1 },
			{ "VK_2", VirtualKeyCode.VK_2 },
			{ "VK_3", VirtualKeyCode.VK_3 },
			{ "VK_4", VirtualKeyCode.VK_4 },
			{ "VK_5", VirtualKeyCode.VK_5 },
			{ "VK_6", VirtualKeyCode.VK_6 },
			{ "VK_7", VirtualKeyCode.VK_7 },
			{ "VK_8", VirtualKeyCode.VK_8 },
			{ "VK_9", VirtualKeyCode.VK_9 },
			{ "VK_0", VirtualKeyCode.VK_0 },
		};
	}
}