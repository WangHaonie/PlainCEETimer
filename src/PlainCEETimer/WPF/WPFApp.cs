using System.Windows;
using System.Windows.Threading;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI;
using PlainCEETimer.WPF.Modules;

namespace PlainCEETimer.WPF;

public sealed class WPFApp : Application, IThemeAware
{
    public static bool IsSystemClosing { get; private set; }

    private ResourceDictionary themeDict;
    private ThemeHelper themeHelper;
    private readonly bool nt10 = !SystemVersion.BeforeNT10;

    public WPFApp()
    {
        InitializeComponent();
    }

    protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
    {
        IsSystemClosing = true;
        base.OnSessionEnding(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        themeHelper.Destroy();
        base.OnExit(e);
        App.Exit();
    }

    private void InitializeComponent()
    {
        DispatcherUnhandledException += (_, e) =>
        {
            if (!e.Handled)
            {
                App.HandleException(e.Exception);
                e.Handled = true;
            }
        };

        ShutdownMode = ShutdownMode.OnMainWindowClose;

        Resources.MergedDictionaries
            .AddEx(Resource.Create("WPF/Appearance/RoundCorner.xaml"))
            .AddEx(Resource.Create("WPF/Appearance/Default.Windows11.xaml"), SystemVersion.IsWindows11)
            .AddEx(Resource.Create("WPF/Appearance/Default.xaml"), nt10);

        themeHelper ??= new(this);
        SafeExecutionContext.SetContext(new DispatcherSynchronizationContext());
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        if (nt10)
        {
            var dict = Resources.MergedDictionaries;
            var old = themeDict;
            themeDict = Resource.Create("WPF/Appearance/Default." + (useDark ? "Dark.xaml" : "Light.xaml"));

            if (old == null)
            {
                dict.Insert(1, themeDict);
            }
            else
            {
                var index = dict.IndexOf(old);
                dict[index != -1 ? index : 1] = themeDict;
            }
        }
    }
}
