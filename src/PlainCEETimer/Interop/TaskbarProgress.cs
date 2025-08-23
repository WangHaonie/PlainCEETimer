using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop
{
    public static class TaskbarProgress
    {
        [DllImport(App.NativesDll, EntryPoint = "#6")]
        public static extern void Initialize(HWND hWnd);

        [DllImport(App.NativesDll, EntryPoint = "#7")]
        public static extern void SetState(TaskbarProgressState tbpFlags);

        [DllImport(App.NativesDll, EntryPoint = "#8")]
        public static extern void SetValue(ulong ullCompleted, ulong ullTotal);

        [DllImport(App.NativesDll, EntryPoint = "#9")]
        public static extern void Release();
    }
}
