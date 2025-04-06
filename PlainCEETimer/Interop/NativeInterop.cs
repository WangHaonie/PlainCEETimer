using System;
using System.Runtime.InteropServices;

namespace PlainCEETimer.Interop
{
    public static class NativeInterop
    {
        #region 来自网络
        /*
        
        提取 DLL 里的图标参考:

        How can I use the images within shell32.dll in my C# project? - Stack Overflow
        https://stackoverflow.com/a/6873026/21094697

        */
        [DllImport("shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int ExtractIconEx(string lpszFile, int nIconIndex, out IntPtr phiconLarge, out IntPtr phiconSmall, int nIcons);
        #endregion

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern uint GetDpiForSystem();

        #region 来自网络
        /*
        
        WinAPI 获取应用程序是否启用深色模式 参考：

        How to detect Windows dark mode - Microsoft Q&A
        https://learn.microsoft.com/en-us/answers/questions/715081/how-to-detect-windows-dark-mode

        */
        [DllImport("uxtheme.dll", EntryPoint = "#132", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShouldAppsUseDarkMode();
        #endregion
    }
}
