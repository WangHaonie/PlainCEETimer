using System;
using System.Collections.Generic;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI;

public class AppMessageFilter : IDisposable
{
    private bool isRunning;
    private int filtersCount;
    private List<IAppMessageFilter> filters;
    private readonly int m_tid;
    private readonly WHGETMESSAGE GetMsgHook;
    private static AppMessageFilter instance;

    private unsafe AppMessageFilter()
    {
        GetMsgHook = GetMsgHookProc;
        m_tid = Win32.GetCurrentThreadId();
    }

    private unsafe IntPtr GetMsgHookProc(int nCode, IntPtr wParam, MSG* lParam)
    {
        if (nCode >= 0 && (int)wParam == NativeConstants.PM_REMOVE)
        {
            for (int i = 0; i < filtersCount; i++)
            {
                if (filters[i].OnMessage(lParam))
                {
                    lParam->message = WM.NULL;
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
            if (filter == null)
            {
                return;
            }

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