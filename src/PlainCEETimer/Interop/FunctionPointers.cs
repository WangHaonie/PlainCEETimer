using System;

namespace PlainCEETimer.Interop;

public delegate bool EnumChildProc(IntPtr hWnd, IntPtr lParam);

public delegate bool EnumDisplayProc(SystemDisplay info);

public delegate IntPtr WNDPROC(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

public delegate IntPtr HOOKPROC(int nCode, IntPtr wParam, IntPtr lParam);
