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

    public enum SettingsArea
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

    public enum AppMessageBoxButtons
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
    public enum AppDialogProp
    {
        BindButtons = 0b01,
        KeyPreview = 0b10
    }

    public enum TaskbarProgressState
    {
        None,
        Indeterminate,
        Normal,
        Error = 4,
        Paused = 8
    }
}
