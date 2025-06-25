using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Interop
{
    public static class TaskbarProgress
    {
        private static readonly int IsWin7Above = (App.OSBuild >= WindowsBuilds.Windows7).ToWin32();

        public static void Initialize(IWin32Window owner)
        {
            Initialize(owner.Handle, IsWin7Above);
        }

        [DllImport(App.NativesDll, EntryPoint = "#5")]
        private static extern void Initialize(IntPtr hWnd, int enable);

        [DllImport(App.NativesDll, EntryPoint = "#6")]
        public static extern void SetState(TaskbarProgressState tbpFlags);

        [DllImport(App.NativesDll, EntryPoint = "#7")]
        public static extern void SetValue(ulong ullCompleted, ulong ullTotal);

        [DllImport(App.NativesDll, EntryPoint = "#8")]
        public static extern void Release();
    }
}
