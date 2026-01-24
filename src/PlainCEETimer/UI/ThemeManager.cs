using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI;

public static class ThemeManager
{
    public static bool IsDarkModeSupported => _isDarkModeSupported;
    public static bool ShouldUseDarkMode => _shouldUseDarkMode;
    public static bool NewThemeAvailable => _canUseNewTheme;

    private static bool _isDarkModeSupported;
    private static bool _shouldUseDarkMode;
    private static bool _canUseNewTheme;
    private static bool isNewDwma;
    private static SystemTheme Theme;

    public static void Initialize()
    {
        if (SystemVersion.Current >= WindowsBuilds.Windows10_1903 && !SystemInformation.HighContrast)
        {
            var tmp = RegistryHelper.Open(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize").Check("AppsUseLightTheme", 0, 1);
            Theme = tmp ? SystemTheme.Dark : SystemTheme.Light;
            var option = App.AppConfig.Dark;

            if ((option == 0 && tmp) || option == 2)
            {
                Win32UI.EnableDarkModeForApp();
                _canUseNewTheme = SystemVersion.Current is var v && v >= WindowsBuilds.Windows11_24H2_WIP && v.UBR >= 6682;
                _shouldUseDarkMode = true;
            }

            _isDarkModeSupported = true;
            isNewDwma = SystemVersion.Current >= WindowsBuilds.Windows10_20H1;
        }
    }

    public static void EnableDarkModeForWindow(IntPtr hWnd)
    {
        Win32UI.EnableDarkModeForWindowFrame(hWnd, isNewDwma);
    }

    public static void EnableDarkModeForControl(IWin32Window control, NativeStyle type, bool AutoUpgrade = false)
    {
        EnableDarkModeForControl(control.Handle, type, AutoUpgrade);
    }

    public static void EnableDarkModeForControl(IntPtr hWnd, NativeStyle type, bool AutoUpgrade = false)
    {
        if (_canUseNewTheme && AutoUpgrade && type == NativeStyle.CfdDark)
        {
            type = NativeStyle.DarkTheme;
        }

        Win32UI.SetWindowTheme(hWnd, GetSubAppName(type), null);
    }

    public static bool IsThemeChanged(int oldValue, int newValue)
    {
        return GetTheme(oldValue) != GetTheme(newValue);
    }

    public unsafe static Color GetAccentColor(IntPtr wParam = default)
    {
        return Color.FromArgb(wParam != default ? (int)(uint)(void*)wParam : Win32UI.GetSystemAccentColor());
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
            return Theme;
        }

        return (SystemTheme)ordinal;
    }
}
