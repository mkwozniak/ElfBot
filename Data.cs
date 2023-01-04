﻿namespace ElfBot
{
	public delegate void FinishedHooking();

	public enum LogTypes
	{
		System,
		Combat,
		Food,
		Camera,
		ZHack,
	}

	public enum CombatStates
	{
		Inactive,
		Targetting,
		CheckingTarget,
		Attacking,
		Looting,
	}
}