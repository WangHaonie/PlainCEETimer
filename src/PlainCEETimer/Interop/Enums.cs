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
public enum MenuFlag
{
    ByCommand = 0x0000,
    ByPosition = 0x0400,
    Bitmap = 0x0004,
    Checked = 0x0008,
    Disabled = 0x0002,
    Enabled = 0x0000,
    Grayed = 0x0001,
    MenuBarBreak = 0x0020,
    MenuBreak = 0x0040,
    OwnerDraw = 0x0100,
    Popup = 0x0010,
    Separator = 0x0800,
    String = 0x0000,
    Unchecked = 0x0000
}

[Flags]
public enum TrackPopupMenu
{
    LeftAlign = 0x0000,
    RightAlign = 0x0008,
    HorizontalCenterAlign = 0x0004,

    BottomAlign = 0x0020,
    TopAlign = 0x0000,
    VerticalCenterAlign = 0x0010,

    NoNotify = 0x0080,
    ReturnCmd = 0x0100,

    LeftButton = 0x0000,
    RightButton = 0x0002,

    Horizontal = 0x0000,
    Vertical = 0x0040,

    Default = LeftAlign | TopAlign | RightButton | Vertical
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
