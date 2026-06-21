using System;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;

namespace PlainCEETimer.UI;

public class FullScreenWindowEventArgs(IntPtr hWnd)
{
    public IntPtr Handle => hWnd;

    public string Text => field ??= Win32UI.GetWindowText(hWnd);

    public string ClassName => field ??= Win32UI.GetClassName(hWnd).AsStringUni().ToString();
}