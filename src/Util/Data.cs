namespace ElfBot;

public delegate void FinishedHooking();
public delegate void ConfigLoadingSuccess(string[] options);

public delegate void SettingConfigFloat(string name, float value);
public delegate void SettingConfigBool(string name, bool value);
public delegate void SettingConfigKey(string name, WindowsInput.Native.VirtualKeyCode value);

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