using Memory;

namespace ElfBot
{
	public static class Globals
	{
		public static readonly Mem TargetApplicationMemory = new Mem();
		public static bool Hooked = false;
	}
}