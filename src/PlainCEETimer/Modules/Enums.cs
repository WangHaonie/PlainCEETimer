namespace PlainCEETimer.Modules;

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
    ExplorerDark,
    CfdDark,
    ItemsView,
    ItemsViewDark
}

public enum SystemTheme
{
    None,
    Light,
    Dark
}

public enum UacNotifyLevel
{
    Unknown = -1,
    Disabled = 0,
    Never = 0,
    AppsOnlyNoDimming = 1,
    AppsOnlyDimming = 2,
    AlwaysDimming = 3,
}
