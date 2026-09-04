using System;
using System.Windows.Forms;

namespace PlainCEETimer.UI;

public static class NativeWindowHelper
{
    public static void Attach<TNativeWindow>(IntPtr hWnd, ref TNativeWindow window)
        where TNativeWindow : NativeWindow, new()
    {
        if (hWnd != IntPtr.Zero)
        {
            Detach(window);
            window ??= new();
            window.AssignHandle(hWnd);
        }
    }

    public static void Detach(NativeWindow window)
    {
        window?.ReleaseHandle();
    }
}