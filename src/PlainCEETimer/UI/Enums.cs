using System;

namespace PlainCEETimer.UI;

public enum CountdownPhase
{
    P1,
    P2,
    P3,
    None
}

public enum CountdownPosition
{
    TopLeft,
    LeftCenter,
    BottomLeft,
    TopCenter,
    Center,
    BottomCenter,
    TopRight,
    RightCenter,
    BottomRight
}

public enum SettingsArea
{
    Restart,
    SyncTime,
    PPTService,
    StartUp
}

public enum MessageButtons
{
    OK,
    YesNo
}

[Flags]
public enum AppFormParam
{
    None,
    BindButtons = 1,
    KeyPreview = 1 << 1,
    Special = 1 << 2,
    CompositedStyle = 1 << 3,
    CenterScreen = 1 << 4,
    OnEscClosing = 1 << 5 | KeyPreview,
    RoundCorner = 1 << 6,
    RoundCornerSmall = 1 << 7 | RoundCorner,
    ModelessDialog = 1 << 8,
    AllControl = BindButtons | KeyPreview
}

public enum ConsoleParam
{
    None,
    AutoClose,
    ShowLeftButton,
    NoMenu
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

public enum HotKeyStatus
{
    /// <summary>
    /// 热键无效
    /// </summary>
    Invalid,

    /// <summary>
    /// 已在当前应用程序注册
    /// </summary>
    Ready,

    /// <summary>
    /// 可以注册
    /// </summary>
    Success,

    /// <summary>
    /// 无法注册
    /// </summary>
    Failed
}
