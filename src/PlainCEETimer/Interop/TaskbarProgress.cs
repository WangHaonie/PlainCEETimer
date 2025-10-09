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

    public void SetState(ProgressStyle state)
    {
        SetState(hWnd, state);
    }

    public void SetValue(long completed, long total)
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
    private static extern void SetState(HWND hWnd, ProgressStyle tbpFlags);

    [DllImport(App.NativesDll, EntryPoint = "#14")]
    private static extern void SetValue(HWND hWnd, long ullCompleted, long ullTotal);

    [DllImport(App.NativesDll, EntryPoint = "#15")]
    private static extern void Release();
}
