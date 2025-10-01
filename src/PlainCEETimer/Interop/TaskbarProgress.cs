using System;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

public class TaskbarProgress : IDisposable
{
    private readonly HWND hWnd;

    public TaskbarProgress(HWND hwnd)
    {
        hWnd = hwnd;
        Initialize();
    }

    public void SetState(TaskbarProgressState state)
    {
        SetState(hWnd, state);
    }

    public void SetValue(ulong completed, ulong total)
    {
        SetValue(hWnd, completed, total);
    }

    public void Dispose()
    {
        Release();
    }

    ~TaskbarProgress()
    {
        Dispose();
    }

    [DllImport(App.NativesDll, EntryPoint = "#12")]
    private static extern void Initialize();

    [DllImport(App.NativesDll, EntryPoint = "#13")]
    private static extern void SetState(HWND hWnd, TaskbarProgressState tbpFlags);

    [DllImport(App.NativesDll, EntryPoint = "#14")]
    private static extern void SetValue(HWND hWnd, ulong ullCompleted, ulong ullTotal);

    [DllImport(App.NativesDll, EntryPoint = "#15")]
    private static extern void Release();
}
