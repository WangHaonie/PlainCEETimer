﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class IconHelper
    {
        public static Icon GetIcon(string file, int index = 0)
        {
            ExtractIconEx(file, index, out IntPtr hIcon, IntPtr.Zero, 1);
            var result = (Icon)Icon.FromHandle(hIcon).Clone();
            DestroyIcon(hIcon);
            return result;
        }

        /*
        
        提取 DLL 里的图标参考:

        How can I use the images within shell32.dll in my C# project? - Stack Overflow
        https://stackoverflow.com/a/6873026/21094697

        */
        [DllImport(App.Shell32Dll, CharSet = CharSet.Unicode)]
        private static extern int ExtractIconEx(string lpszFile, int nIconIndex, out IntPtr phiconLarge, IntPtr phiconSmall, int nIcons);

        [DllImport(App.User32Dll)]
        private static extern BOOL DestroyIcon(IntPtr hIcon);
    }
}
