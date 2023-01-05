namespace ElfBot
{
    using Memory;

    public static class Globals
    {
        public static readonly Mem TargetApplicationMemory = new Mem();
        public static bool Hooked = false;
    }
}