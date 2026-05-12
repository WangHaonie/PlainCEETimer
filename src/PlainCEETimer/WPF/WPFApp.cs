using System.Windows;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;
using PlainCEETimer.WPF.Modules;

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
        var a = !SystemVersion.BeforeNT10;
        ShutdownMode = ShutdownMode.OnMainWindowClose;

        Resources.MergedDictionaries
            .AddEx(Resource.Create("WPF/Appearance/RoundCorner.xaml"))
            .AddEx(Resource.Create("WPF/Appearance/Default." + (ThemeManager.ShouldUseDarkMode ? "Dark.xaml" : "Light.xaml")), a)
            .AddEx(Resource.Create("WPF/Appearance/Default.xaml"), a)
            .AddEx(Resource.Create("WPF/Appearance/Default.Windows11.xaml"), SystemVersion.IsWindows11);
    }
}
