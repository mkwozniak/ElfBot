namespace ElfBot;

public delegate void FinishedHooking();
public delegate void SendingKey(int key);

public enum LogTypes
{
    System,
    Combat,
    Food,
    Camera,
    ZHack,
}

public enum KeybindType
{
    None,
    Attack,
    Skill,
    HpFood,
    HpInstant,
    MpFood,
    MpInstant,
    Buff,
}