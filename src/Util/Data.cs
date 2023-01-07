namespace ElfBot;

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

public enum KeybindType
{
    None,
    Attack,
    Skill,
    HpFood,
    HpInstant,
    MpFood,
    MpInstant
}