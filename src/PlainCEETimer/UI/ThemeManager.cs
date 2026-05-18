using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI;

public static class ThemeManager
{
    private class ThemeChangedMessageFilter : IAppMessageFilter
    {
        private readonly Throttler thr = new(TryFireOnThemeChanged);

        public void OnMessage(IntPtr lpMsg)
        {
            if (IsThemeChanged(lpMsg))
            {
                thr.Throttle();
            }
        }

        private static bool IsThemeChanged(IntPtr lpMsg)
        {
            var msg = Marshal.ReadInt32(lpMsg, MSG.message);

            if (msg == WM.SYSCOLORCHANGE)
            {
                return true;
            }

            if (msg == WM.SETTINGCHANGE || msg == WM.SETTINGCHANGE + WM.REFLECT)
            {
                return Marshal.PtrToStringUni(Marshal.ReadIntPtr(lpMsg, MSG.lParam)) == "ImmersiveColorSet";
            }

            return false;
        }
    }

    public static bool IsDarkModeSupported => isDarkModeSupported;

    public static bool ShouldUseDarkMode => shouldUseDarkMode;

    public static bool NewThemeAvailable => canUseNewTheme;

    private static bool canFireThemeChanged;
    private static bool isDarkModeSupported;
    private static bool shouldUseDarkMode;
    private static bool canUseNewTheme;
    private static bool isNewDwma;
    private static bool isDetecting;
    private static SystemTheme theme;
    private static ThemeChangedMessageFilter msgfilter;

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
            isDarkModeSupported = true;
            isNewDwma = SystemVersion.Current.AtLeast(WindowsVersions.Windows10_20H1);
        }
    }

    public static void EnableDarkModeForWindow(IntPtr hWnd, bool enabled)
    {
        Win32UI.EnableDarkModeForWindowFrame(hWnd, isNewDwma, enabled);
    }

    public static void EnableDarkModeForControl(IWin32Window control, SystemStyle type, bool AutoUpgrade = false)
    {
        EnableDarkModeForControl(control.Handle, type, AutoUpgrade);
    }

    public static void EnableDarkModeForControl(IntPtr hWnd, SystemStyle type, bool AutoUpgrade = false)
    {
        if (canUseNewTheme && AutoUpgrade && type == SystemStyle.CfdDark)
        {
            type = SystemStyle.DarkTheme;
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

    internal static void OnThemeChanged(SystemTheme theme)
    {
        shouldUseDarkMode = GetTheme(theme) == SystemTheme.Dark;
        UpdateAppTheme();
        ThemeChanged?.Invoke(null, new(theme));
    }

    private static void UpdateAppTheme()
    {
        Win32UI.EnableDarkModeForApp(shouldUseDarkMode);
    }

    internal static SystemTheme UpdateThemeForUserChoice()
    {
        var option = App.AppConfig.Theme;
        canFireThemeChanged = option == SystemTheme.Auto;

        if (!canFireThemeChanged)
        {
            StopDetectingThemeChanges();
        }
        else if (!isDetecting)
        {
            StartDetectingThemeChanges();
        }

        return option;
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
        AppMessageFilter.AddMessageFilter(msgfilter ??= new());
        App.AppExit += StopDetectingThemeChanges;
        isDetecting = true;
    }

    private static void StopDetectingThemeChanges()
    {
        AppMessageFilter.RemoveMessageFilter(msgfilter);
        isDetecting = false;
    }

    private static SystemTheme GetCurrentSystemTheme()
    {
        var tmp = RegistryHelper.Open(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize").Check("AppsUseLightTheme", 0, 1);
        return tmp ? SystemTheme.Dark : SystemTheme.Light;
    }

    private static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        throw new NotImplementedException();
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
            SystemStyle.ItemsViewDark => "DarkMode_ItemsView",
            SystemStyle.ItemsView => "ItemsView",
            SystemStyle.DarkTheme => "DarkMode_DarkTheme",
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
