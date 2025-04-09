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
        public static Color DarkBack { get; } = Color.FromArgb(32, 32, 32);
        public static Color DarkFore { get; } = Color.White;
        public static int Initialize;

        static ThemeManager()
        {
            IsDarkModeSupported = App.OSBuild >= WindowsBuilds.Windows10_1903
                && !SystemInformation.HighContrast;
            var option = App.AppConfig.Dark;
            ShouldUseDarkMode = IsDarkModeSupported && ((option == 0 && ShouldAppsUseDarkMode()) || option == 1);

            if (ShouldUseDarkMode)
            {
                SetPreferredAppMode(2);
            }

        }

        public static void FlushDarkControl(Control control, DarkControlType type)
        {
            FlushDarkControl(control.Handle, type);
        }

        public static void FlushDarkControl(IntPtr hWnd, DarkControlType type)
        {
            SetWindowTheme(hWnd, GetPszSubAppName(type), null);
        }

        #region 来自网络
        /*
        
        WinAPI 获取应用程序是否启用深色模式 参考：

        How to detect Windows dark mode - Microsoft Q&A
        https://learn.microsoft.com/en-us/answers/questions/715081/how-to-detect-windows-dark-mode

        */
        [DllImport(App.UxThemeDll, EntryPoint = "#132", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShouldAppsUseDarkMode();
        #endregion

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
        [DllImport(App.UxThemeDll, CallingConvention = CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        [DllImport(App.UxThemeDll, EntryPoint = "#135", CallingConvention = CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int SetPreferredAppMode(int preferredAppMode);
        #endregion

        [DllImport(App.NativesDll, CallingConvention = CallingConvention.StdCall)]
        public static extern int FlushDarkWindow(IntPtr hWnd);

        private static string GetPszSubAppName(DarkControlType type) => type switch
        {
            DarkControlType.Explorer => "DarkMode_Explorer",
            DarkControlType.CFD => "DarkMode_CFD",
            DarkControlType.ItemsView => "DarkMode_ItemsView",
            _ => "Explorer"
        };
    }
}
