using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI;

public static class ThemeManager
{
    public static bool IsDarkModeSupported => Supported;
    public static bool ShouldUseDarkMode => UseDark;
    public static bool NewThemeAvailable => CanUseNewTheme;
    public static SystemTheme CurrentTheme => Theme;

    private static bool Supported;
    private static bool UseDark;
    private static bool CanUseNewTheme;
    private static bool IsNewDwma;
    private static SystemTheme Theme;

    public static void Initialize()
    {
        if (Supported = SystemVersion.Current >= WindowsBuilds.Windows10_1903 && !SystemInformation.HighContrast)
        {
            var tmp = RegistryHelper.Open(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize").Check("AppsUseLightTheme", 0, 1);
            Theme = tmp ? SystemTheme.Dark : SystemTheme.Light;
            var option = App.AppConfig.Dark;

            if (SystemVersion.Current >= WindowsBuilds.Windows10_20H1)
            {
                IsNewDwma = true;
            }

            if (UseDark = (option == 0 && tmp) || option == 2)
            {
                Win32UI.EnableDarkModeForApp();
                CanUseNewTheme = SystemVersion.Current is var v && v >= WindowsBuilds.Windows11_24H2_WIP && v.UBR >= 6682;
            }
        }

        Application.EnableVisualStyles();
    }

    public static void EnableDarkMode(HWND hWnd)
    {
        Win32UI.EnableDarkModeForWindowFrame(hWnd, IsNewDwma);
    }

    public static void EnableDarkMode(IWin32Window control, NativeStyle type, bool AutoUpgrade = false)
    {
        EnableDarkMode(control.Handle, type, AutoUpgrade);
    }

    public static void EnableDarkMode(HWND hWnd, NativeStyle type, bool AutoUpgrade = false)
    {
        if (CanUseNewTheme && AutoUpgrade)
        {
            type = type switch
            {
                NativeStyle.CfdDark => NativeStyle.DarkTheme,
                _ => type
            };
        }

        Win32UI.SetWindowTheme(hWnd, GetSubAppName(type), null);
    }

    public static bool IsThemeChanged(int oldValue, int newValue)
    {
        return GetTheme(oldValue) != GetTheme(newValue);
    }

    public static Color GetAccentColor(IntPtr wParam = default)
    {
        return Color.FromArgb(wParam != default ? (int)(wParam.ToInt64() & 0xFFFFFFFF) : Win32UI.GetSystemAccentColor());
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

    private static string GetSubAppName(NativeStyle style)
    {
        return style switch
        {
            NativeStyle.ExplorerDark => "DarkMode_Explorer",
            NativeStyle.CfdDark => "DarkMode_CFD",
            NativeStyle.ItemsViewDark => "DarkMode_ItemsView",
            NativeStyle.ItemsView => "ItemsView",
            NativeStyle.DarkTheme => "DarkMode_DarkTheme",
            _ => "Explorer",
        };
    }

    private static SystemTheme GetTheme(int ordinal)
    {
        if (ordinal == 0)
        {
            return CurrentTheme;
        }

        return (SystemTheme)ordinal;
    }
}
