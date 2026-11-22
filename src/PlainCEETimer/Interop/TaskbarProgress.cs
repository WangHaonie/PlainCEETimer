using System;
using System.Runtime.InteropServices;
using System.Security;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

[SuppressUnmanagedCodeSecurity]
public sealed class TaskbarProgress : IDisposable
{
    private bool m_disposed;
    private readonly IntPtr m_hWnd;
    private static int s_ref;
    private static readonly object syncLock = new();

    public TaskbarProgress(IntPtr hwnd)
    {
        m_hWnd = hwnd;

        lock (syncLock)
        {
            s_ref++;
            Initialize();
        }
    }

    public void SetState(ProgressStyle state)
    {
        SetState(m_hWnd, state);
    }

    public void SetValue(long completed, long total)
    {
        SetValue(m_hWnd, completed, total);
    }

    public void Dispose()
    {
        if (!m_disposed)
        {
            m_disposed = true;

            lock (syncLock)
            {
                if (--s_ref == 0)
                {
                    Release();
                }
            }
        }
    }

    [DllImport(App.NativesDll, EntryPoint = "#26")]
    private static extern void Initialize();

    [DllImport(App.NativesDll, EntryPoint = "#27")]
    private static extern void SetState(IntPtr hWnd, ProgressStyle tbpFlags);

    [DllImport(App.NativesDll, EntryPoint = "#28")]
    private static extern void SetValue(IntPtr hWnd, long ullCompleted, long ullTotal);

    [DllImport(App.NativesDll, EntryPoint = "#29")]
    private static extern void Release();
}
