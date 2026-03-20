using System.Windows;
using PlainCEETimer.Modules;
using PlainCEETimer.UI;
using PlainCEETimer.WPF.Extensions;

namespace PlainCEETimer.WPF;

public sealed class WPFApp : Application
{
    public static bool IsSystemClosing { get; private set; }

    public WPFApp()
    {
        InitializeComponent();
    }

    protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
    {
        IsSystemClosing = true;
        base.OnSessionEnding(e);
    }

    private void InitializeComponent()
    {
        var a = !SystemVersion.BeforeWinNT10;
        ShutdownMode = ShutdownMode.OnMainWindowClose;

        Resources.MergedDictionaries
            .AddResource("WPF/Appearance/Default." + (ThemeManager.ShouldUseDarkMode ? "Dark.xaml" : "Light.xaml"), a)
            .AddResource("WPF/Appearance/Default.xaml", a)
            .AddResource("WPF/Appearance/Default.Windows11.xaml", a && SystemVersion.IsWindows11)
            .AddResource("WPF/Appearance/RoundCorner.xaml");
    }
}