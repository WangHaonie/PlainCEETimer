using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI;

public class AppMessageFilter : IDisposable
{
    private bool isRunning;
    private int filtersCount;
    private List<IAppMessageFilter> filters;
    private readonly int m_tid;
    private readonly HOOKPROC GetMsgHook;
    private static AppMessageFilter instance;

    private AppMessageFilter()
    {
        GetMsgHook = GetMsgHookProc;
        m_tid = Win32.GetCurrentThreadId();
    }

    private IntPtr GetMsgHookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            for (int i = 0; i < filtersCount; i++)
            {
                if (filters[i].OnMessage(lParam))
                {
                    Marshal.WriteInt32(lParam, MSG.message, WM.NULL);
                }
            }
        }

        return IntPtr.Zero;
    }

    private void TryRun()
    {
        if (!isRunning)
        {
            Win32UI.HookGetMessage(GetMsgHook, m_tid);
            App.AppExit += Dispose;
            isRunning = true;
        }
    }

    private void Add(IAppMessageFilter filter)
    {
        if (filter == null)
        {
            return;
        }

        TryRun();
        filters ??= [];
        filters.Add(filter);
        filtersCount = filters.Count;
    }

    private void Remove(IAppMessageFilter filter)
    {
        if (isRunning)
        {
            filters.Remove(filter);
            filtersCount = filters.Count;
        }
    }

    public void Dispose()
    {
        Win32UI.UnhookGetMessage();
        GC.SuppressFinalize(this);
    }

    public static void Initialize()
    {
        instance ??= new();
    }

    public static void AddMessageFilter(IAppMessageFilter filter)
    {
        instance.Add(filter);
    }

    public static void RemoveMessageFilter(IAppMessageFilter filter)
    {
        instance.Remove(filter);
    }

    ~AppMessageFilter()
    {
        Dispose();
    }
}