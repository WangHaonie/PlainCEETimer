﻿using System;

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

    public enum MessageButtons
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
    public enum AppFormParam
    {
        BindButtons = 0b00001,
        KeyPreview = 0b00010,
        AllControl = BindButtons | KeyPreview,
        Special = 0b00100,
        CompositedStyle = 0b01000,
        CenterScreen = 0b10000
    }

    public enum TaskbarProgressState
    {
        None,
        Indeterminate,
        Normal,
        Error = 4,
        Paused = 8
    }

    public enum NativeStyle
    {
        Explorer,
        CFD,
        ExplorerLight
    }

    public enum CommonDialogKind
    {
        Color,
        Font
    }

    public enum NativeControl
    {
        Label,
        Button,
        ComboBox,
        ComboLBox,
        TextBox
    }

    public enum SystemTheme
    {
        None,
        Light,
        Dark
    }
}
