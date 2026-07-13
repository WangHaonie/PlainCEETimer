using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Fody;
using Timer = System.Threading.Timer;

namespace PlainCEETimer.UI;

[NoConstants]
public class FullScreenTracker : IDisposable
{
    public FullScreenTrackingMode TrackingMode
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

    public static FullScreenTracker Instance { get; } = new();

    public event EventHandler<FullScreenWindowEventArgs> FullScreenEntered;
    public event EventHandler<FullScreenWindowEventArgs> FullScreenExited;

    private bool hasStarted;
    private bool hasPendingEvent;
    private IntPtr m_hForegroundHook;
    private IntPtr m_hLocationChangeHook;
    private IntPtr m_hwndPending;
    private IntPtr m_hwndTracked;
    private IntPtr m_hwndActive;
    private FullScreenTrackingMode m_mode;
    private Screen m_screen;
    private Timer m_timer;
    private WINEVENTPROC m_proc;
    private readonly object syncLock = new();
    private readonly Throttler throttler;
    private readonly Action ProcessPendingWindowAction;
    private readonly ActionInvoker<IntPtr> OnFullScreenEnteredInvoker;
    private readonly ActionInvoker<IntPtr> OnFullScreenExitedInvoker;

    private const int FSTolerance = 2;
    private const long ThrottleInterval = 300;

    private readonly string[] m_banlist =
    [
        "XamlExplorerHostIslandWindow"
    ];

    private FullScreenTracker()
    {
        throttler = new(ThrottleInterval);
        ProcessPendingWindowAction = ProcessPendingWindow;
        OnFullScreenEnteredInvoker = new(hWnd => FullScreenEntered?.Invoke(null, new(hWnd)));
        OnFullScreenExitedInvoker = new(hWnd => FullScreenExited?.Invoke(null, new(hWnd)));
    }

    static FullScreenTracker()
    {
        App.Current.AppExit += Instance.Destroy;
    }

    public void SetScreen(Screen screen)
    {
        m_screen = screen;

        if (hasStarted)
        {
            SyncCurrentState();
        }
    }

    public void Start()
    {
        if (!hasStarted && m_mode != FullScreenTrackingMode.None)
        {
            m_proc = WinEventProc;
            m_hForegroundHook = Win32.SetWinEventHook(WEH.EVENT_SYSTEM_FOREGROUND, WEH.EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, m_proc, 0, 0, WEH.WINEVENT_OUTOFCONTEXT);
            m_hLocationChangeHook = Win32.SetWinEventHook(WEH.EVENT_OBJECT_LOCATIONCHANGE, WEH.EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, m_proc, 0, 0, WEH.WINEVENT_OUTOFCONTEXT);
            hasStarted = m_hForegroundHook != IntPtr.Zero && m_hLocationChangeHook != IntPtr.Zero;
            m_screen ??= Screen.PrimaryScreen;
            m_timer = new(TimerCallback, null, Timeout.Infinite, Timeout.Infinite);

            if (!hasStarted)
            {
                Stop();
                return;
            }

            SyncCurrentState();
        }
    }

    public void Stop()
    {
        if (hasStarted)
        {
            Win32.UnhookWinEvent(m_hForegroundHook);
            Win32.UnhookWinEvent(m_hLocationChangeHook);
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

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }

    private void WinEventProc(IntPtr hWinEventHook, int dwEvent, IntPtr hwnd, int idObject, int idChild, int idEventThread, int dwmsEventTime)
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

    private void SyncCurrentState()
    {
        m_hwndTracked = FindTrackedWindow(Win32UI.GetForegroundWindow());
        m_hwndActive = ResolveActiveFullScreenWindow(m_hwndTracked);
    }

    private IntPtr FindTrackedWindow(IntPtr hWnd)
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

    private bool IsTrackedFullScreenWindow(IntPtr hWnd)
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

    private bool ShouldIgnoreWindow(IntPtr hWnd)
    {
        using var cn = Win32UI.GetClassName(hWnd).AsStringUni();

        foreach (var wnd in m_banlist)
        {
            if (cn == wnd)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsOnTargetScreen(IntPtr hWnd)
    {
        return m_screen.Equals(Screen.FromHandle(hWnd));
    }

    private bool IsFullScreenWindow(IntPtr hWnd)
    {
        if (Win32UI.GetWindowRect(hWnd, out var rcw))
        {
            Rectangle r = rcw;
            r.Inflate(FSTolerance, FSTolerance);
            return r.Contains(m_screen.Bounds);
        }

        return false;
    }

    private IntPtr ResolveActiveFullScreenWindow(IntPtr hWnd)
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

    private void UpdateTrackedWindow(IntPtr hwndNext)
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

    private void ProcessPendingWindow()
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

    private void TimerCallback(object state)
    {
        ProcessPendingWindow();
    }

    private void OnFullScreenEntered(IntPtr hWnd)
    {
        SafeExecutionContext.Post(OnFullScreenEnteredInvoker.WithArgs(hWnd));
    }

    private void OnFullScreenExited(IntPtr hWnd)
    {
        SafeExecutionContext.Post(OnFullScreenExitedInvoker.WithArgs(hWnd));
    }

    private static IntPtr GetRootWindow(IntPtr hWnd)
    {
        var root = Win32UI.GetAncestor(hWnd, GA.ROOT);
        return root != IntPtr.Zero ? root : hWnd;
    }

    ~FullScreenTracker()
    {
        Dispose();
    }
}
