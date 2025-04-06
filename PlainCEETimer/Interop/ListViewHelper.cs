﻿using PlainCEETimer.Modules;
using System;
using System.Runtime.InteropServices;

namespace PlainCEETimer.Interop
{
    public static class ListViewHelper
    {
        [DllImport(App.AppNativesDll, CallingConvention = CallingConvention.StdCall)]
        public static extern void SelectAllItems(IntPtr hLV, int state);
    }
}
