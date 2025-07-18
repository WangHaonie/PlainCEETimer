﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class ThemeManager
    {
        public static int VerticalScrollBarWidth { get; } = SystemInformation.VerticalScrollBarWidth;
        public static bool IsDarkModeSupported { get; }
        public static bool ShouldUseDarkMode { get; }
        public static Color DarkFore { get; } = Color.White;
        public static Color DarkForeLink { get; } = Color.FromArgb(153, 235, 255);
        public static Color DarkBack { get; } = Color.FromArgb(32, 32, 32);
        public static Color DarkBorder { get; } = Color.FromArgb(60, 60, 60);
        public static SystemTheme CurrentTheme { get; } = SystemTheme.None;
        public static int Initialize;
        private static readonly BOOL IsNewDwma;

        static ThemeManager()
        {
            if (IsDarkModeSupported = App.OSBuild >= WindowsBuilds.Windows10_1903 && !SystemInformation.HighContrast)
            {
                var tmp = RegistryHelper.Open(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize").Check("AppsUseLightTheme", 0, 1);
                CurrentTheme = tmp ? SystemTheme.Dark : SystemTheme.Light;
                var option = App.AppConfig.Dark; // 顺便触发初始化 DefaultValues

                if (App.OSBuild >= WindowsBuilds.Windows10_20H1)
                {
                    IsNewDwma = BOOL.TRUE;
                }

                if (ShouldUseDarkMode = (option == 0 && tmp) || option == 2)
                {
                    FlushApp();
                }
            }
        }

        public static void FlushControl(IWin32Window control, NativeStyle type)
        {
            FlushControl(control.Handle, type);
        }

        public static void FlushWindow(IntPtr hWnd)
        {
            FlushWindow(hWnd, IsNewDwma);
        }

        public static void FlushControl(IntPtr hWnd, NativeStyle type)
        {
            SetTheme(hWnd, type);
        }

        public static bool IsThemeChanged(int oldValue, int newValue)
        {
            return GetTheme(oldValue) != GetTheme(newValue);
        }

        private static SystemTheme GetTheme(int ordinal)
        {
            if (ordinal == 0)
            {
                return CurrentTheme;
            }

            return (SystemTheme)ordinal;
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

        [DllImport(App.NativesDll, EntryPoint = "#9")]
        private static extern void FlushWindow(IntPtr hWnd, BOOL newStyle);

        [DllImport(App.NativesDll, EntryPoint = "#11")]
        private static extern void FlushApp();

        [DllImport(App.NativesDll, EntryPoint = "#12")]
        private static extern void SetTheme(IntPtr hWnd, NativeStyle type);
    }
}
