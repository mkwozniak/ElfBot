using VirtualKeyCode = WindowsInput.Native.VirtualKeyCode;

namespace ElfBot
{
    using Mem = Memory.Mem;
	using ConfigKeyEntryDict = System.Collections.Generic.Dictionary<string, VirtualKeyCode>;

    public static class Globals
	{

		public static readonly Mem TargetApplicationMemory = new Mem();
        public static readonly Logger Logger = new Logger();

	}
}