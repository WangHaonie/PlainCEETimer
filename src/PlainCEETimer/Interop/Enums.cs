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

public enum DpiAwarenessContext
{
    Unknown,
    Unaware = -1,
    System = -2,
    PerMonitor = -3,
    PerMonitorV2 = -4,

    /// <summary>
    /// After Windows 10 Build 1809
    /// </summary>
    GdiScaled = -5
}