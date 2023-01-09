namespace ElfBot;

public delegate void FinishedHooking();
public delegate void SendingKey(int key);

public enum KeybindAction
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