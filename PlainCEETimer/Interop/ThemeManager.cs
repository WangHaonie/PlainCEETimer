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
        public static Color DarkForeHeader { get; } = Color.FromArgb(222, 222, 222);
        public static Color LightForeHeader { get; } = Color.FromArgb(76, 96, 122);
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

            Application.EnableVisualStyles();
        }

        public static void FlushWindow(HWND hWnd)
        {
            FlushWindow(hWnd, IsNewDwma);
        }

        public static void FlushControl(IWin32Window control, NativeStyle type)
        {
            FlushControl(control.Handle, type);
        }

        public static void FlushControl(HWND hWnd, NativeStyle type)
        {
            SetWindowTheme(hWnd, GetPszSubAppName(type), null);
        }

        public static bool IsThemeChanged(int oldValue, int newValue)
        {
            return GetTheme(oldValue) != GetTheme(newValue);
        }

        private static string GetPszSubAppName(NativeStyle style)
        {
            return style switch
            {
                NativeStyle.Explorer => "DarkMode_Explorer",
                NativeStyle.CFD => "DarkMode_CFD",
                NativeStyle.ItemsView => "DarkMode_ItemsView",
                NativeStyle.ItemsViewLight => "ItemsView",
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

        /*
        
        控件使用系统内置深色外观 参考：
        
        Win32 Dark Mode
        https://gist.github.com/rounk-ctrl/b04e5622e30e0d62956870d5c22b7017
        
        BlueMystical/Dark-Mode-Forms: Apply Dark Mode to all Controls in a Form [WinForms]
        https://github.com/BlueMystical/Dark-Mode-Forms
        
        ysc3839/win32-darkmode: Example application shows how to use undocumented dark mode API introduced in Windows 10 1809.
        https://github.com/ysc3839/win32-darkmode

        */

        [DllImport(App.UxThemeDll, CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(HWND hWnd, string pszSubAppName, string pszSubIdList);

        [DllImport(App.NativesDll, EntryPoint = "#10")]
        private static extern void FlushApp();

        [DllImport(App.NativesDll, EntryPoint = "#11")]
        private static extern void FlushWindow(HWND hWnd, BOOL newStyle);
    }
}
