using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using MS.Internal;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Fody;
using PlainCEETimer.UI;
using PlainCEETimer.WPF.Modules;

namespace PlainCEETimer.WPF;

[NoConstants]
public sealed class WPFApp : Application, IThemeAware
{
    public static bool IsSystemClosing { get; private set; }

    private ResourceDictionary themeDict;
    private ThemeHelper themeHelper;
    private readonly bool nt10 = !SystemVersion.BeforeNT10;

    private const string ThemeDir = "WPF/Appearance/";

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
        App.Current.Shutdown();
    }

    private void InitializeComponent()
    {
        DispatcherUnhandledException += (_, e) =>
        {
            if (!e.Handled)
            {
                App.Current.HandleException(e.Exception);
                e.Handled = true;
            }
        };

        ShutdownMode = ShutdownMode.OnMainWindowClose;
        InternalInit();
        LoadTheme();
        SafeExecutionContext.SetContext(new DispatcherSynchronizationContext());
        FrameworkAppContextSwitches._useAdornerForTextboxSelectionRendering = -1;
    }

    private void LoadTheme()
    {
        var dict = Resources.MergedDictionaries;

        if (SystemVersion.IsWindows11)
        {
            dict.Add(Resource.Create(ThemeDir + "Default.Windows11.xaml"));
        }
        else if (SystemVersion.Current.AtLeast(WindowsVersions.Windows10_1903))
        {
            dict.Add(Resource.Create(ThemeDir + "Default.Windows10.xaml"));
        }

        dict.AddEx(Resource.Create(ThemeDir + "Default.xaml"), nt10)
            .AddEx(Resource.Create(ThemeDir + "Controls.xaml"));

        themeHelper ??= new(this);
    }

    private static void InternalInit()
    {
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

        AppCommands.Initialize();
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        if (nt10)
        {
            var dict = Resources.MergedDictionaries;
            var old = themeDict;
            themeDict = Resource.Create(ThemeDir + "Default." + (useDark ? "Dark.xaml" : "Light.xaml"));

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
