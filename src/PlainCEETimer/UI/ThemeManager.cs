using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.Fody;

namespace PlainCEETimer.UI;

[NoConstants]
public static class ThemeManager
{
    private class ThemeChangedMessageFilter : IAppMessageFilter
    {
        private readonly Throttler thr;

        public ThemeChangedMessageFilter()
        {
            thr = new();
        }

        public unsafe bool OnMessage(MSG* lpMsg)
        {
            if (IsThemeChanged(lpMsg) && thr.Throttle())
            {
                TryFireOnThemeChanged();
                return true;
            }

            return false;
        }

        private unsafe static bool IsThemeChanged(MSG* lpMsg)
        {
            return lpMsg->message switch
            {
                WinUser.WM_SYSCOLORCHANGE
                    => true,
                WinUser.WM_SETTINGCHANGE or WinUser.WM_SETTINGCHANGE + WinUser.WM_REFLECT
                    => lpMsg->lParam.AsStringUni(free: false) == "ImmersiveColorSet",
                _
                    => false,
            };
        }
    }

    public static bool IsDarkModeSupported => initialized;

    public static bool ShouldUseDarkMode => shouldUseDarkMode;

    public static bool NewThemeAvailable => canUseNewTheme;

    public static bool TransparencyEnabled
    {
        get
        {
            if (!SystemVersion.BeforeNT10)
            {
                using var reg = RegistryHelper.Open(PersonalizeRegPath);
                return reg.Check("EnableTransparency", 1, 0);
            }

            return false;
        }
    }

    private static bool canFireThemeChanged;
    private static bool initialized;
    private static bool shouldUseDarkMode;
    private static bool canUseNewTheme;
    private static bool isNewDwma;
    private static SystemTheme theme;
    private static ThemeChangedMessageFilter msgfilter;
    private const string PersonalizeRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";

    public static event EventHandler<ThemeChangedEventArgs> ThemeChanged;

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
            Win32UI.EnableDarkModeForWindowFrame(hWnd, isNewDwma, enabled);
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

    public unsafe static Color GetAccentColor(IntPtr wParam = default)
    {
        return Color.FromArgb(wParam != default ? (int)(uint)(void*)wParam : Win32UI.GetSystemAccentColor());
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
        ThemeChanged?.Invoke(null, new(theme));
    }

    private static void UpgradeStyle(ref SystemStyle style)
    {
        if (canUseNewTheme && style == SystemStyle.CfdDark)
        {
            style = SystemStyle.DarkTheme;
        }
    }

    private static void UpdateAppTheme()
    {
        Win32UI.EnableDarkModeForApp(shouldUseDarkMode);
    }

    private static void TryFireOnThemeChanged()
    {
        var th = GetCurrentSystemTheme();

        if (IsThemeChanged(theme, th))
        {
            theme = th;
            OnThemeChanged(th);
        }
    }

    private static void StartDetectingThemeChanges()
    {
        if (initialized && msgfilter == null)
        {
            msgfilter = new();
            AppMessageFilter.AddMessageFilter(msgfilter);
            App.Current.AppExit += StopDetectingThemeChanges;
        }
    }

    private static void StopDetectingThemeChanges()
    {
        if (msgfilter != null)
        {
            AppMessageFilter.RemoveMessageFilter(msgfilter);
            msgfilter = null;
            App.Current.AppExit -= StopDetectingThemeChanges;
        }
    }

    private static SystemTheme GetCurrentSystemTheme()
    {
        using var reg = RegistryHelper.Open(PersonalizeRegPath);
        return reg.Check("AppsUseLightTheme", 0, 1) ? SystemTheme.Dark : SystemTheme.Light;
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
