using PlainCEETimer.Modules;
using System;
using System.Runtime.InteropServices;

namespace PlainCEETimer.Interop
{
    public static class TaskbarProgress
    {
        [DllImport(App.NativesDll, EntryPoint = "#5")]
        public static extern void Initialize(IntPtr hWnd, int enable);

        [DllImport(App.NativesDll, EntryPoint = "#6")]
        public static extern void SetState(TaskbarProgressState tbpFlags);

        [DllImport(App.NativesDll, EntryPoint = "#7")]
        public static extern void SetValue(ulong ullCompleted, ulong ullTotal);

        [DllImport(App.NativesDll, EntryPoint = "#8")]
        public static extern void Release();
    }
}
