using System;

namespace PlainCEETimer.Interop;

[Flags]
public enum KeyModifiers : byte
{
    None = 0x00,
    Shit = 0x01, // (●'◡'●)
    Control = 0x02,
    Alt = 0x04,
    Ext = 0x08
}

public enum ShowWindowCommand
{
    Normal = 1,
    Maximize = 3,
    Minimize = 7
}

public enum ProgressStyle
{
    None,
    Indeterminate,
    Normal,
    Error = 4,
    Paused = 8
}

public enum TaskLogonType
{
    None,
    Password,
    ServiceForUser,
    InteractiveToken,
    Group,
    ServiceAccount,
    InteractiveTokenOrPassword = Password
}
