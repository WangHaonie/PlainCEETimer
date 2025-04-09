using PlainCEETimer.Modules;
using System;
using System.Runtime.InteropServices;

namespace PlainCEETimer.Interop
{
    public class TaskbarProgress
    {
        private static readonly bool IsWindows7Above;

        public TaskbarProgress(IntPtr hWnd)
        {
            if (IsWindows7Above)
            {
                InitilizeTaskbarList(hWnd);
            }
        }

        static TaskbarProgress()
        {
            IsWindows7Above = App.OSBuild >= WindowsBuilds.Windows7;
        }

        public void SetState(TaskbarProgressState State)
        {
            if (IsWindows7Above)
            {
                SetTaskbarProgressState((int)State);
            }
        }

        public void SetValue(ulong ullCompleted, ulong ullTotal)
        {
            if (IsWindows7Above)
            {
                SetTaskbarProgressValue(ullCompleted, ullTotal);
            }
        }

        public void Release()
        {
            if (IsWindows7Above)
            {
                ReleaseTaskbarList();
            }
        }

        [DllImport(App.NativesDll, CallingConvention = CallingConvention.StdCall)]
        private static extern void InitilizeTaskbarList(IntPtr hWnd);

        [DllImport(App.NativesDll, CallingConvention = CallingConvention.StdCall)]
        private static extern void SetTaskbarProgressState(int tbpFlags);

        [DllImport(App.NativesDll, CallingConvention = CallingConvention.StdCall)]
        private static extern void SetTaskbarProgressValue(ulong ullCompleted, ulong ullTotal);

        [DllImport(App.NativesDll, CallingConvention = CallingConvention.StdCall)]
        private static extern void ReleaseTaskbarList();
    }
}
