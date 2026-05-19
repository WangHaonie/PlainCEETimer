using System.Windows;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;
using PlainCEETimer.WPF.Modules;

namespace PlainCEETimer.WPF;

public sealed class WPFApp : Application
{
    public static bool IsSystemClosing { get; private set; }

    private readonly bool nt10 = !SystemVersion.BeforeNT10;
    private ResourceDictionary themeDict;

    public WPFApp()
    {
        ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
#if !DEBUG
        DispatcherUnhandledException += (_, e) => App.HandleException(e.Exception.PassIf(!e.Handled));
#endif
        InitializeComponent();
    }

    protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
    {
        IsSystemClosing = true;
        base.OnSessionEnding(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        ThemeManager.ThemeChanged -= ThemeManager_ThemeChanged;
        base.OnExit(e);
    }

    private void InitializeComponent()
    {
        ShutdownMode = ShutdownMode.OnMainWindowClose;
        Resources.MergedDictionaries
            .AddEx(Resource.Create("WPF/Appearance/RoundCorner.xaml"))
            .AddEx(Resource.Create("WPF/Appearance/Default.xaml"), nt10)
            .AddEx(Resource.Create("WPF/Appearance/Default.Windows11.xaml"), SystemVersion.IsWindows11);
        ApplyTheme();
    }

    private void ApplyTheme()
    {
        Resources.MergedDictionaries
            .RemoveEx(themeDict, themeDict != null)
            .AddEx(themeDict = Resource.Create("WPF/Appearance/Default." + (ThemeManager.ShouldUseDarkMode ? "Dark.xaml" : "Light.xaml")), nt10);
    }

    private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
    {
        ApplyTheme();
    }
}
