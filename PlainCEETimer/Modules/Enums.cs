namespace PlainCEETimer.Modules
{
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
        ExplorerLight,
        CFD,
        ItemsView,
        ItemsViewLight
    }

    public enum SystemTheme
    {
        None,
        Light,
        Dark
    }

    public enum UacNotifyLevel
    {
        AllDimming,
        AppsOnlyDimming,
        AppsOnlyNoDimming,
        Never,
        Disabled,
        Unknown
    }
}
