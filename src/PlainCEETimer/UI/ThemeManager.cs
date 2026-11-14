using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.Fody;

namespace PlainCEETimer.UI;

[NoConstants]
public static class ThemeManager
{
    public static bool IsDarkModeSupported => initialized;

    public static bool ShouldUseDarkMode => shouldUseDarkMode;

    public static bool NewThemeAvailable => canUseNewTheme;

    public static event EventHandler<ThemeChangedEventArgs> ThemeChanged
    {
        add => ThemeChangedEvent.Add(value);
        remove => ThemeChangedEvent.Remove(value);
    }

    private static bool canFireThemeChanged;
    private static bool initialized;
    private static bool shouldUseDarkMode;
    private static bool canUseNewTheme;
    private static bool isNewDwma;
    private static bool isDetecting;
    private static SystemTheme theme;
    private static readonly EfficientInvocationList<ThemeChangedEventArgs> ThemeChangedEvent = new(150);

    public static void Initialize()
    {
        if (SystemVersion.Current.AtLeast(WindowsVersions.Windows10_1903) && !SystemInformation.HighContrast)
        {
            var option = UpdateThemeForUserChoice();
            theme = GetCurrentSystemTheme();

            if ((canFireThemeChanged && theme == SystemTheme.Dark) || option == SystemTheme.Dark)
            {
                canUseNewTheme = SystemVersion.Current.AtLeast(WindowsVersions.Windows11_24H2_WIP_NewDarkTheme);
                shouldUseDarkMode = true;
            }

            UpdateAppTheme();
            initialized = true;
            isNewDwma = SystemVersion.Current.AtLeast(WindowsVersions.Windows10_20H1);
        }
    }

    public static void EnableDarkModeForWindow(IntPtr hWnd, bool enabled)
    {
        if (initialized)
        {
            Win32UI.PnEnableDarkModeForWindowFrame(hWnd, isNewDwma, enabled);
        }
    }

    public static void ApplyControlTheme(IWin32Window control, SystemStyle type, bool AutoUpgrade = false)
    {
        ApplyControlTheme(control.Handle, type, AutoUpgrade);
    }

    public static void ApplyControlTheme(IntPtr hWnd, SystemStyle type, bool AutoUpgrade = false)
    {
        if (AutoUpgrade)
        {
            UpgradeStyle(ref type);
        }

        Win32UI.SetWindowTheme(hWnd, GetSubAppName(type), null);
    }

    public static bool IsThemeChanged(SystemTheme oldValue, SystemTheme newValue)
    {
        return GetTheme(oldValue) != GetTheme(newValue);
    }

    public static Color GetAccentColor(nint wParam = default)
    {
        return Color.FromArgb(wParam != default ? (int)wParam : Win32UI.PnGetSystemAccentColor());
    }

    internal static SystemTheme UpdateThemeForUserChoice()
    {
        var option = App.Current.AppConfig.Theme;
        canFireThemeChanged = option == SystemTheme.Auto;

        if (!canFireThemeChanged)
        {
            StopDetectingThemeChanges();
        }
        else
        {
            theme = GetCurrentSystemTheme();
            StartDetectingThemeChanges();
        }

        return option;
    }

    internal static void OnThemeChanged(SystemTheme theme)
    {
        theme = GetTheme(theme);
        shouldUseDarkMode = theme == SystemTheme.Dark;
        UpdateAppTheme();
        ThemeChangedEvent.Invoke(null, new(theme));
    }

    private static void UpgradeStyle(ref SystemStyle style)
    {
        if (canUseNewTheme && (style is SystemStyle.CfdDark or SystemStyle.ExplorerDark))
        {
            style = SystemStyle.DarkTheme;
        }
    }

    private static void UpdateAppTheme()
    {
        Win32UI.PnEnableDarkModeForApp(shouldUseDarkMode);
        Win32UI.PnHookThemeData();
    }

    private static void OnAppsUseLightThemeChanged(object sender, SystemSettingChangedEventArgs<bool> e)
    {
        if (canFireThemeChanged)
        {
            var th = e.NewValue ? SystemTheme.Light : SystemTheme.Dark;
            theme = th;
            OnThemeChanged(th);
        }
    }

    private static void StartDetectingThemeChanges()
    {
        if (initialized && !isDetecting)
        {
            isDetecting = true;
            SystemSettings.AppsUseLightTheme.Changed += OnAppsUseLightThemeChanged;
        }
    }

    private static void StopDetectingThemeChanges()
    {
        if (isDetecting)
        {
            isDetecting = false;
            SystemSettings.AppsUseLightTheme.Changed -= OnAppsUseLightThemeChanged;
        }
    }

    private static SystemTheme GetCurrentSystemTheme()
    {
        return SystemSettings.AppsUseLightTheme.Value ? SystemTheme.Light : SystemTheme.Dark;
    }

    /*

    控件使用系统内置深色外观 参考：

    Win32 Dark Mode
    https://gist.github.com/rounk-ctrl/b04e5622e30e0d62956870d5c22b7017

    BlueMystical/Dark-Mode-Forms: Apply Dark Mode to all Controls in a Form [WinForms]
    https://github.com/BlueMystical/Dark-Mode-Forms

    ysc3839/win32-darkmode: Example application shows how to use undocumented dark mode API introduced in Windows 10 1809.
    https://github.com/ysc3839/win32-darkmode

    */

    private static string GetSubAppName(SystemStyle style)
    {
        return style switch
        {
            SystemStyle.ExplorerDark => "DarkMode_Explorer",
            SystemStyle.CfdDark => "DarkMode_CFD",
            SystemStyle.Cfd => "CFD",
            SystemStyle.ItemsViewDark => "DarkMode_ItemsView",
            SystemStyle.ItemsView => "ItemsView",
            SystemStyle.DarkTheme => "DarkMode_DarkTheme",
            SystemStyle.Progress => nameof(SystemStyle.Progress),
            _ => "Explorer",
        };
    }

    private static SystemTheme GetTheme(SystemTheme st)
    {
        if (st == SystemTheme.Auto)
        {
            return theme;
        }

        return st;
    }
}
