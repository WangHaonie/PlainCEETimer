using Microsoft.Win32;
using PlainCEETimer.Modules;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PlainCEETimer.Interop
{
    public static class ThemeManager
    {
        public static bool AllowDarkMode { get; }
        public static bool IsDarkMode { get; private set; }
        public static event EventHandler SystemThemeChanged;
        public static int Initialize;

        public static Color LightFore => Color.FromArgb(27, 27, 27);
        public static Color LightBack => Color.FromArgb(243, 243, 243);
        public static Color LightLinkColor => Color.Blue;
        public static Color DarkFore => Color.White;
        public static Color DarkBack => Color.FromArgb(32, 32, 32);
        public static Color DarkLinkColor => Color.FromArgb(95, 197, 255);

        private static readonly int DwmWa;

        static ThemeManager()
        {
            AllowDarkMode = App.OSBuild >= WindowsBuilds.Windows10_1903;
            DwmWa = AllowDarkMode && App.OSBuild <= WindowsBuilds.Windows10_20H1 ? 19 : 20;

            if (AllowDarkMode)
            {
                SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
                RefreshTheme();
            }
        }

        public static void FlushTitleBar(IntPtr hWnd, int enabled)
        {
            DwmSetWindowAttribute(hWnd, DwmWa, ref enabled, sizeof(int));
        }

        public static void FlushControl(Control Ctrl, DarkControlType Type)
        {
            var hWnd = Ctrl.Handle;
            AllowDarkModeForWindow(hWnd, IsDarkMode);
            SetWindowTheme(hWnd, GetPszSubAppName(Type), null);
            SendMessage(hWnd, 0x031A, IntPtr.Zero, IntPtr.Zero);
        }

        private static string GetPszSubAppName(DarkControlType Type) => Type switch
        {
            DarkControlType.ComboBox => "CFD",
            _ => "Explorer"
        };

        private static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                RefreshTheme();
            }
        }

        private static void RefreshTheme()
        {
            var IsDark = (int)(Registry.CurrentUser.OpenSubKey(
            @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize")?.GetValue("AppsUseLightTheme") ?? 1) == 0;

            if (IsDark != IsDarkMode)
            {
                IsDarkMode = IsDark;
                SystemThemeChanged?.Invoke(null, EventArgs.Empty);
                UpdateTheme();
            }
        }

        private static void UpdateTheme()
        {
            SetPreferredAppMode(IsDarkMode ? 1 : 0);
        }

        [DllImport("uxtheme.dll", SetLastError = true, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        [DllImport("uxtheme.dll", EntryPoint = "#133", SetLastError = true, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllowDarkModeForWindow(IntPtr hWnd, bool allow);

        [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern int SetPreferredAppMode(int preferredAppMode);

        [DllImport("dwmapi.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
    }
}
