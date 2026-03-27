using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

public static class Win32
{
    public static IntPtr AllocConsole()
    {
        var hwnd = AllocConsoleForApp(SystemVersion.BeforeWinNT10, out var phStdIn, out var phStdOut, out var phStdErr);
        Console.SetIn(new StreamReader(new FileStream(new SafeFileHandle(phStdIn, false), FileAccess.Read), Console.InputEncoding));
        Console.SetOut(new StreamWriter(new FileStream(new SafeFileHandle(phStdOut, false), FileAccess.Write), Console.OutputEncoding) { AutoFlush = true });
        Console.SetError(new StreamWriter(new FileStream(new SafeFileHandle(phStdErr, false), FileAccess.Write), Console.OutputEncoding) { AutoFlush = true });
        return hwnd;
    }

    [DllImport(App.NativesDll, EntryPoint = "#41")]
    private static extern IntPtr AllocConsoleForApp(bool fRefresh, out IntPtr phStdIn, out IntPtr phStdOut, out IntPtr phStdErr);

    [DllImport(App.NativesDll, EntryPoint = "#42")]
    public static extern void KillProcessTree(int dwProcessId);

    [DllImport(App.Kernel32Dll)]
    public static extern void ExitProcess(int uExitCode);

    [DllImport(App.Kernel32Dll)]
    public static extern int GetCurrentProcessId();

    [DllImport(App.Kernel32Dll)]
    public static extern int GetCurrentThreadId();
}