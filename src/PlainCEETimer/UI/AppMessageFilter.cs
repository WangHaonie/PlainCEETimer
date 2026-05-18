using System;
using System.Collections.Generic;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI;

public class AppMessageFilter : IDisposable
{
    private int filtersCount;
    private List<IAppMessageFilter> filters;
    private readonly HOOKPROC GetMsgHook;
    private static AppMessageFilter instance;

    private AppMessageFilter()
    {
        GetMsgHook = GetMsgHookProc;
        Win32UI.HookGetMessage(GetMsgHook);
    }

    private IntPtr GetMsgHookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            for (int i = 0; i < filtersCount; i++)
            {
                filters[i].OnMessage(lParam);
            }
        }

        return IntPtr.Zero;
    }

    private void Add(IAppMessageFilter filter)
    {
        if (filter == null)
        {
            return;
        }

        filters ??= [];
        filters.Add(filter);
        filtersCount = filters.Count;
    }

    private void Remove(IAppMessageFilter filter)
    {
        if (filters != null && filter != null)
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

    public static void AddMessageFilter(IAppMessageFilter filter)
    {
        instance ??= new();
        instance.Add(filter);
    }

    public static void RemoveMessageFilter(IAppMessageFilter filter)
    {
        instance ??= new();
        instance.Remove(filter);
    }

    ~AppMessageFilter()
    {
        Dispose();
    }
}