using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI;

public static class FullScreenDetector
{
    public static event EventHandler<FullScreenWindowEventArgs> FullScreenEntered;
    public static event EventHandler<FullScreenWindowEventArgs> FullScreenExited;

    private static bool hasStarted;
    private static bool wasFullScreen;
    private static IntPtr m_hHook;
    private static Type m_sender;
    private static Screen m_screen;
    private static WINEVENTPROC m_proc;

    public static void SetScreen(Screen screen)
    {
        m_screen = screen;
    }

    public static void Start()
    {
        if (!hasStarted)
        {
            m_proc = WinEventProc;
            m_sender = typeof(FullScreenDetector);
            m_hHook = Win32.SetWinEventHook(WEH.EVENT_SYSTEM_FOREGROUND, WEH.EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, m_proc, 0, 0, WEH.WINEVENT_OUTOFCONTEXT);
            hasStarted = m_hHook != IntPtr.Zero;
        }
    }

    public static void Stop()
    {
        if (hasStarted)
        {
            Win32.UnhookWinEvent(m_hHook);
            m_proc = null;
            m_sender = null;
            m_screen = null;
            hasStarted = false;
        }
    }

    private static void WinEventProc(IntPtr hWinEventHook, int dwEvent, IntPtr hwnd, int idObject, int idChild, int idEventThread, int dwmsEventTime)
    {
        if (idObject != WEH.OBJID_WINDOW || idChild != WEH.CHILDID_SELF || hwnd == IntPtr.Zero)
        {
            return;
        }

        var hWnd = Win32UI.GetForegroundWindow();

        if (hWnd != IntPtr.Zero)
        {
            var screen = Screen.FromHandle(hWnd);

            if (screen != null && screen.Equals(m_screen))
            {
                var isfs = IsFullScreenWindow(hWnd, screen);

                if (isfs ^ wasFullScreen)
                {
                    if (isfs)
                    {
                        OnFullScreenEntered(hWnd);
                        wasFullScreen = true;
                    }
                    else
                    {
                        OnFullScreenExited(hWnd);
                        wasFullScreen = false;
                    }
                }
            }
            else
            {
                if (wasFullScreen)
                {
                    OnFullScreenExited(hWnd);
                    wasFullScreen = false;
                }
            }
        }
    }

    private static bool IsFullScreenWindow(IntPtr hWnd, Screen screen)
    {
        if (Win32UI.GetWindowRect(hWnd, out var rcw))
        {
            var rc = screen.Bounds;

            return rc.X - rcw.Left >= 0
                && rc.Y - rcw.Top >= 0
                && rcw.Right - rc.Right >= 0
                && rcw.Bottom - rc.Bottom >= 0;
        }

        return false;
    }

    private static void OnFullScreenEntered(IntPtr hWnd)
    {
        FullScreenEntered?.Invoke(m_sender, new(hWnd));
    }

    private static void OnFullScreenExited(IntPtr hWnd)
    {
        FullScreenExited?.Invoke(m_sender, new(hWnd));
    }
}