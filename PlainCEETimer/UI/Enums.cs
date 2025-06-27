using System;

namespace PlainCEETimer.UI
{
    public enum CountdownState
    {
        Normal,
        DaysOnly,
        DaysOnlyWithCeiling,
        HoursOnly,
        MinutesOnly,
        SecondsOnly
    }

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

    /// <summary>
    /// 表示倒计时要显示哪些阶段
    /// </summary>
    public enum CountdownMode
    {
        /// <summary>
        /// 只显示 P1 阶段。
        /// </summary>
        Mode1 = 0b001,

        /// <summary>
        /// 显示 P1 和 P2 阶段。
        /// </summary>
        Mode2 = Mode1 | 0b010,

        /// <summary>
        /// 显示全部阶段。
        /// </summary>
        Mode3 = Mode2 | 0b100
    }

    public enum SettingsArea
    {
        Funny,
        SyncTime,
        SetPPTService,
        ChangeFont
    }

    public enum MessageLevel
    {
        Info,
        Warning,
        Error
    }

    public enum MessageButtons
    {
        OK,
        YesNo
    }

    [Flags]
    public enum AppFormParam
    {
        BindButtons = 1,
        KeyPreview = 1 << 1,
        Special = 1 << 2,
        CompositedStyle = 1 << 3,
        CenterScreen = 1 << 4,
        OnEscClosing = 1 << 5 | KeyPreview,
        RoundCorner = 1 << 6,
        AllControl = BindButtons | KeyPreview
    }

    public enum CommonDialogKind
    {
        Color,
        Font
    }
}
