namespace PlainCEETimer.Modules
{
    public enum ExitReason
    {
        Normal,
        UserShutdown,
        UserRestart,
        AppUpdating,
        InvalidExeName,
        MultipleInstances
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
        ItemsView,
        ExplorerLight
    }

    public enum SystemTheme
    {
        None,
        Light,
        Dark
    }

    public enum UACNotifyLevel
    {
        AllDimming,
        AppsOnlyDimming,
        AppsOnlyNoDimming,
        Never,
        Disabled,
        Unknown
    }
}
