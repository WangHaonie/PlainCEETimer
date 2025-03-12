using System;

namespace PlainCEETimer.Modules
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

    public enum WorkingArea
    {
        Funny,
        SyncTime,
        SetPPTService,
        LastColor,
        SelectedColor,
        ChangeFont
    }

    public enum MessageLevel
    {
        Info,
        Warning,
        Error
    }

    public enum MessageBoxExButtons
    {
        OK,
        YesNo
    }

    public enum ExitReason
    {
        NormalExit,
        AppUpdating,
        UserShutdown,
        UserRestart,
        InvalidExeName,
        AnotherInstanceIsRunning
    }

    [Flags]
    public enum DialogExProp
    {
        BindButtons = 0B00100001,
        KeyPreview = 0B001000_10
    }

    public enum CountdownPhase
    {
        P1,
        P2,
        P3
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

    public enum TaskbarProgressState
    {
        None,
        Indeterminate,
        Normal,
        Error = 4,
        Paused = 8
    }

    public enum DarkControlType
    {
        Button,
        ComboBox,
        CheckBox
    }
}
