using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI;

public static class FullScreenTracker
{
    public static FullScreenTrackingMode TrackingMode
    {
        get => m_mode;
        set
        {
            if (m_mode != value)
            {
                m_mode = value;
                UpdateTrackedWindow(m_hwndTracked);
            }
        }
    }

    public static event EventHandler<FullScreenWindowEventArgs> FullScreenEntered;
    public static event EventHandler<FullScreenWindowEventArgs> FullScreenExited;

    private static bool hasStarted;
    private static IntPtr m_hForegroundHook;
    private static IntPtr m_hLocationChangeHook;
    private static IntPtr m_hwndTracked;
    private static IntPtr m_hwndActive;
    private static FullScreenTrackingMode m_mode;
    private static Screen m_screen;
    private static WINEVENTPROC m_proc;

    private static readonly HashSet<string> IgnoredWindowClasses = new(StringComparer.Ordinal)
    {
        "XamlExplorerHostIslandWindow"
    };

    public static void SetScreen(Screen screen)
    {
        m_screen = screen;
        SyncCurrentState();
    }

    public static void Start()
    {
        if (!hasStarted)
        {
            m_proc = WinEventProc;
            m_hForegroundHook = Win32.SetWinEventHook(WEH.EVENT_SYSTEM_FOREGROUND, WEH.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, m_proc, 0, 0, WEH.WINEVENT_OUTOFCONTEXT);
            m_hLocationChangeHook = Win32.SetWinEventHook(WEH.EVENT_OBJECT_LOCATIONCHANGE, WEH.EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, m_proc, 0, 0, WEH.WINEVENT_OUTOFCONTEXT);
            hasStarted = m_hForegroundHook != IntPtr.Zero && m_hLocationChangeHook != IntPtr.Zero;
            m_screen ??= Screen.PrimaryScreen;

            if (!hasStarted)
            {
                Stop();
                return;
            }

            SyncCurrentState();
        }
    }

    public static void Stop()
    {
        if (hasStarted)
        {
            Win32.UnhookWinEvent(m_hForegroundHook);
            Win32.UnhookWinEvent(m_hLocationChangeHook);
            m_proc = null;
        }

        m_hForegroundHook = IntPtr.Zero;
        m_hLocationChangeHook = IntPtr.Zero;
        m_hwndTracked = IntPtr.Zero;
        m_hwndActive = IntPtr.Zero;
        m_screen = null;
        hasStarted = false;
    }

    private static void WinEventProc(IntPtr hWinEventHook, int dwEvent, IntPtr hwnd, int idObject, int idChild, int idEventThread, int dwmsEventTime)
    {
        if (idObject != WEH.OBJID_WINDOW || idChild != WEH.CHILDID_SELF || hwnd == IntPtr.Zero)
        {
            return;
        }

        UpdateTrackedWindow(FindTrackedWindow(GetRootWindow(hwnd)));
    }

    private static void SyncCurrentState()
    {
        m_hwndTracked = FindTrackedWindow(Win32UI.GetForegroundWindow());
        m_hwndActive = ResolveActiveFullScreenWindow(m_hwndTracked);
    }

    private static IntPtr FindTrackedWindow(IntPtr hWnd)
    {
        hWnd = GetRootWindow(hWnd);

        if (IsTrackedFullScreenWindow(hWnd))
        {
            return hWnd;
        }

        if (m_hwndTracked != IntPtr.Zero && m_hwndTracked != hWnd && IsTrackedFullScreenWindow(m_hwndTracked))
        {
            return m_hwndTracked;
        }

        var fore = GetRootWindow(Win32UI.GetForegroundWindow());

        if (fore != IntPtr.Zero && fore != hWnd && fore != m_hwndTracked && IsTrackedFullScreenWindow(fore))
        {
            return fore;
        }

        return IntPtr.Zero;
    }

    private static bool IsTrackedFullScreenWindow(IntPtr hWnd)
    {
        return hWnd != IntPtr.Zero
            && m_screen != null
            && hWnd != Win32UI.GetShellWindow()
            && !ShouldIgnoreWindow(hWnd)
            && Win32UI.IsWindowVisible(hWnd)
            && (Win32UI.GetWindowLong(hWnd, GWL.STYLE) & (WS.CHILD | WS.MINIMIZE)) == 0
            && IsOnTargetScreen(hWnd)
            && IsFullScreenWindow(hWnd);
    }

    private static bool ShouldIgnoreWindow(IntPtr hWnd)
    {
        return IgnoredWindowClasses.Contains(Win32UI.GetClassName(hWnd));
    }

    private static bool IsOnTargetScreen(IntPtr hWnd)
    {
        return m_screen.Equals(Screen.FromHandle(hWnd));
    }

    private static bool IsFullScreenWindow(IntPtr hWnd)
    {
        if (Win32UI.GetWindowRect(hWnd, out var rcw))
        {
            var rc = m_screen.Bounds;

            return rc.X - rcw.Left >= 0
                && rc.Y - rcw.Top >= 0
                && rcw.Right - rc.Right >= 0
                && rcw.Bottom - rc.Bottom >= 0;
        }

        return false;
    }

    private static IntPtr GetRootWindow(IntPtr hWnd)
    {
        var root = Win32UI.GetAncestor(hWnd, GA.ROOT);
        return root != IntPtr.Zero ? root : hWnd;
    }

    private static IntPtr ResolveActiveFullScreenWindow(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero)
        {
            return IntPtr.Zero;
        }

        if (TrackingMode == FullScreenTrackingMode.TreatFocusLossAsExit)
        {
            var fore = GetRootWindow(Win32UI.GetForegroundWindow());

            if (fore != hWnd)
            {
                return IntPtr.Zero;
            }
        }

        return hWnd;
    }

    private static void UpdateTrackedWindow(IntPtr hwndNext)
    {
        m_hwndTracked = hwndNext;
        var hwndNextActive = ResolveActiveFullScreenWindow(hwndNext);
        var old = m_hwndActive;

        if (hwndNextActive == old)
        {
            return;
        }

        m_hwndActive = hwndNextActive;

        if (old != IntPtr.Zero)
        {
            OnFullScreenExited(old);
        }

        if (hwndNextActive != IntPtr.Zero)
        {
            OnFullScreenEntered(hwndNextActive);
        }
    }

    private static void OnFullScreenEntered(IntPtr hWnd)
    {
        FullScreenEntered?.Invoke(null, new(hWnd));
    }

    private static void OnFullScreenExited(IntPtr hWnd)
    {
        FullScreenExited?.Invoke(null, new(hWnd));
    }
}
