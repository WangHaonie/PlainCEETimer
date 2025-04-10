using PlainCEETimer.Modules;
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
        [DllImport(App.Shell32Dll, CharSet = CharSet.Unicode)]
        public static extern int ExtractIconEx(string lpszFile, int nIconIndex, out IntPtr phiconLarge, out IntPtr phiconSmall, int nIcons);
        #endregion

        [DllImport(App.User32Dll)]
        public static extern uint GetDpiForSystem();
    }
}
