using PlainCEETimer.Modules;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PlainCEETimer.Interop
{
    public static class ThemeManager
    {
        public static bool IsDarkModeSupported { get; }
        public static bool ShouldUseDarkMode { get; }
        public static Color DarkFore { get; } = Color.White;
        public static Color DarkForeLink { get; } = Color.FromArgb(95, 197, 255);
        public static Color DarkBack { get; } = Color.FromArgb(32, 32, 32);
        public static Color DarkBackSelection { get; } = Color.FromArgb(77, 77, 77);
        public static Color DarkBorder { get; } = Color.FromArgb(100, 100, 100);
        public static SystemTheme CurrentTheme { get; } = SystemTheme.None;
        public static int Initialize;
        private static readonly int DarkDwmaType;

        static ThemeManager()
        {
            if (IsDarkModeSupported = App.OSBuild >= WindowsBuilds.Windows10_1903 && !SystemInformation.HighContrast)
            {
                var tmp = RegistryHelper.Open(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize").GetState("AppsUseLightTheme", 0, 1);
                CurrentTheme = tmp ? SystemTheme.Dark : SystemTheme.Light;
                var option = App.AppConfig.Dark; // 顺便触发初始化 DefaultValues

                if (App.OSBuild >= WindowsBuilds.Windows10_20H1)
                {
                    DarkDwmaType = 1;
                }

                if (ShouldUseDarkMode = (option == 0 && tmp) || option == 2)
                {
                    SetPreferredAppMode(2);
                }
            }
        }

        public static void FlushDarkTitleBar(IntPtr hWnd)
        {
            FlushDarkWindow(hWnd, DarkDwmaType);
        }

        public static void FlushDarkControl(Control control, NativeStyle type)
        {
            FlushDarkControl(control.Handle, type);
        }

        public static void FlushDarkControl(IntPtr hWnd, NativeStyle type)
        {
            var name = type switch
            {
                NativeStyle.Explorer => "DarkMode_Explorer",
                NativeStyle.CFD => "DarkMode_CFD",
                _ => "Explorer"
            };

            SetWindowTheme(hWnd, name, null);
        }

        #region 来自网络
        /*
        
        控件使用系统内置深色外观 参考：
        
        Win32 Dark Mode
        https://gist.github.com/rounk-ctrl/b04e5622e30e0d62956870d5c22b7017
        
        【MIT】BlueMystical/Dark-Mode-Forms: Apply Dark Mode to all Controls in a Form [WinForms]
        https://github.com/BlueMystical/Dark-Mode-Forms
        
        【MIT】ysc3839/win32-darkmode: Example application shows how to use undocumented dark mode API introduced in Windows 10 1809.
        https://github.com/ysc3839/win32-darkmode

        */

        [DllImport(App.UxThemeDll, EntryPoint = "#135")]
        private static extern int SetPreferredAppMode(int preferredAppMode);

        [DllImport(App.UxThemeDll, CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        [DllImport(App.NativesDll, EntryPoint = "#9")]
        private static extern int FlushDarkWindow(IntPtr hWnd, int type);
        #endregion
    }
}
