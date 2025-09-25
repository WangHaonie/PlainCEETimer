using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

public static class TaskbarProgress
{
    [DllImport(App.NativesDll, EntryPoint = "#12")]
    public static extern void Initialize(HWND hWnd);

    [DllImport(App.NativesDll, EntryPoint = "#13")]
    public static extern void SetState(TaskbarProgressState tbpFlags);

    [DllImport(App.NativesDll, EntryPoint = "#14")]
    public static extern void SetValue(ulong ullCompleted, ulong ullTotal);

    [DllImport(App.NativesDll, EntryPoint = "#15")]
    public static extern void Release();
}
