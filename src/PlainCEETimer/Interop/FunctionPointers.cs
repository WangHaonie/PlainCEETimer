using System;

namespace PlainCEETimer.Interop;

public delegate BOOL EnumChildProc(HWND hWnd, IntPtr lParam);

public delegate BOOL EnumDisplayProc(SystemDisplay info);

public delegate IntPtr WNDPROC(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
