using System;

namespace PlainCEETimer.Interop;

[Flags]
public enum HotkeyF : byte
{
    None = 0x00,
    Shit = 0x01,
    Ctrl = 0x02,
    Alt = 0x04,
    Ext = 0x08
}

[Flags]
public enum HotKeyModifiers : uint
{
    None = 0x0000,
    Alt = 0x0001,
    Ctrl = 0x0002,
    Shit = 0x0004,
    Win = 0x0008
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
