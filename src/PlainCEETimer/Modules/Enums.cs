namespace PlainCEETimer.Modules;

public enum NativeStyle
{
    Explorer,
    ExplorerDark,
    CfdDark,
    ItemsView,
    ItemsViewDark,
    DarkTheme // ≥ 26120.6682 ?
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
    NeverNotify = 1,
    AppsOnlyNoDimmed = 2,
    AppsOnlyDimmed = 3,
    AlwaysDimmed = 4,
}

public enum AdminRights
{
    Unknown,
    Yes,
    No
}

public enum FileDialogKind
{
    OpenFile,
    SaveFile
}