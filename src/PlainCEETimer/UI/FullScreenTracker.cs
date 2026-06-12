using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Fody;
using PlainCEETimer.Modules.Linq;
using Timer = System.Threading.Timer;

namespace PlainCEETimer.UI;

[NoConstants]
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

                if (hasStarted)
                {
                    if (value == FullScreenTrackingMode.None)
                    {
                        Stop();
                        return;
                    }

                    UpdateTrackedWindow(m_hwndTracked);
                }
            }
        }
    }

    public static event EventHandler<FullScreenWindowEventArgs> FullScreenEntered;
    public static event EventHandler<FullScreenWindowEventArgs> FullScreenExited;

    private static bool hasStarted;
    private static bool hasPendingEvent;
    private static IntPtr m_hForegroundHook;
    private static IntPtr m_hLocationChangeHook;
    private static IntPtr m_hwndPending;
    private static IntPtr m_hwndTracked;
    private static IntPtr m_hwndActive;
    private static FullScreenTrackingMode m_mode;
    private static Screen m_screen;
    private static Timer m_timer;
    private static WINEVENTPROC m_proc;
    private static readonly object syncLock = new();
    private static readonly Throttler throttler;
    private static readonly Action ProcessPendingWindowAction;
    private static readonly ActionInvoker<IntPtr> OnFullScreenEnteredInvoker;
    private static readonly ActionInvoker<IntPtr> OnFullScreenExitedInvoker;

    private const int FSTolerance = 2;
    private const long ThrottleInterval = 300;

    private static readonly string[] m_banlist =
    [
        "XamlExplorerHostIslandWindow"
    ];

    static FullScreenTracker()
    {
        throttler = new(ThrottleInterval);
        ProcessPendingWindowAction = ProcessPendingWindow;
        OnFullScreenEnteredInvoker = new(hWnd => FullScreenEntered?.Invoke(null, new(hWnd)));
        OnFullScreenExitedInvoker = new(hWnd => FullScreenExited?.Invoke(null, new(hWnd)));
    }

    public static void SetScreen(Screen screen)
    {
        m_screen = screen;

        if (hasStarted)
        {
            SyncCurrentState();
        }
    }

    public static void Start()
    {
        if (!hasStarted && m_mode != FullScreenTrackingMode.None)
        {
            m_proc = WinEventProc;
            m_hForegroundHook = Win32.SetWinEventHook(WEH.EVENT_SYSTEM_FOREGROUND, WEH.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, m_proc, 0, 0, WEH.WINEVENT_OUTOFCONTEXT);
            m_hLocationChangeHook = Win32.SetWinEventHook(WEH.EVENT_OBJECT_LOCATIONCHANGE, WEH.EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, m_proc, 0, 0, WEH.WINEVENT_OUTOFCONTEXT);
            hasStarted = m_hForegroundHook != IntPtr.Zero && m_hLocationChangeHook != IntPtr.Zero;
            m_screen ??= Screen.PrimaryScreen;
            m_timer = new(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);
            App.AppExit += Stop;

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
            App.AppExit -= Stop;
            m_proc = null;
        }

        m_hForegroundHook = IntPtr.Zero;
        m_hLocationChangeHook = IntPtr.Zero;
        m_hwndPending = IntPtr.Zero;
        m_hwndTracked = IntPtr.Zero;
        m_hwndActive = IntPtr.Zero;
        m_screen = null;
        hasPendingEvent = false;
        m_timer.Destroy();
        hasStarted = false;
    }

    private static void WinEventProc(IntPtr hWinEventHook, int dwEvent, IntPtr hwnd, int idObject, int idChild, int idEventThread, int dwmsEventTime)
    {
        if (idObject != WEH.OBJID_WINDOW || idChild != WEH.CHILDID_SELF || hwnd == IntPtr.Zero)
        {
            return;
        }

        lock (syncLock)
        {
            hasPendingEvent = true;
            m_hwndPending = GetRootWindow(hwnd);
            m_timer.Change(ThrottleInterval, Timeout.Infinite);
        }

        throttler.Throttle(ProcessPendingWindowAction);
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
        return m_banlist.ArrayContains(Win32UI.GetClassName(hWnd));
    }

    private static bool IsOnTargetScreen(IntPtr hWnd)
    {
        return m_screen.Equals(Screen.FromHandle(hWnd));
    }

    private static bool IsFullScreenWindow(IntPtr hWnd)
    {
        if (Win32UI.GetWindowRect(hWnd, out var rcw))
        {
            return Rectangle.Inflate(rcw, FSTolerance, FSTolerance).Contains(m_screen.Bounds);
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
        if (hWnd != IntPtr.Zero && m_mode == FullScreenTrackingMode.FocusLoss)
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

    private static void ProcessPendingWindow()
    {
        IntPtr hwnd;

        lock (syncLock)
        {
            if (!hasPendingEvent)
            {
                return;
            }

            hwnd = m_hwndPending;
            hasPendingEvent = false;
        }

        UpdateTrackedWindow(FindTrackedWindow(hwnd));
    }

    private static void TimerCallback(object state)
    {
        ProcessPendingWindow();
    }

    private static void OnFullScreenEntered(IntPtr hWnd)
    {
        SafeExecutionContext.Execute(OnFullScreenEnteredInvoker.WithArgs(hWnd));
    }

    private static void OnFullScreenExited(IntPtr hWnd)
    {
        SafeExecutionContext.Execute(OnFullScreenExitedInvoker.WithArgs(hWnd));
    }
}
