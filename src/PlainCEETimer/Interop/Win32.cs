using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

[SuppressUnmanagedCodeSecurity]
internal static class Win32
{
    public static IntPtr AllocConsole()
    {
        var hwnd = PnAllocConsole(SystemVersion.BeforeNT10, out var phStdIn, out var phStdOut, out var phStdErr);
        Console.SetIn(new StreamReader(new FileStream(new SafeFileHandle(phStdIn, false), FileAccess.Read), Console.InputEncoding));
        Console.SetOut(new StreamWriter(new FileStream(new SafeFileHandle(phStdOut, false), FileAccess.Write), Console.OutputEncoding) { AutoFlush = true });
        Console.SetError(new StreamWriter(new FileStream(new SafeFileHandle(phStdErr, false), FileAccess.Write), Console.OutputEncoding) { AutoFlush = true });
        return hwnd;
    }

    [DllImport(App.NativesDll, EntryPoint = "#54")]
    private static extern IntPtr PnAllocConsole(bool bRefresh, out IntPtr phStdIn, out IntPtr phStdOut, out IntPtr phStdErr);

    [DllImport(App.NativesDll, EntryPoint = "#55")]
    public static extern void PnKillProcessTree(int dwProcessId);

    [DllImport(App.NativesDll, EntryPoint = "#56")]
    public static extern int PnLoadStringInternal(uint uID, out IntPtr ppBuffer);

    [DllImport(App.Kernel32Dll)]
    public static extern ulong GetTickCount64();

    [DllImport(App.Kernel32Dll)]
    public static extern void ExitProcess(int uExitCode);

    [DllImport(App.Kernel32Dll)]
    public static extern int GetCurrentProcessId();

    [DllImport(App.Kernel32Dll)]
    public static extern int GetCurrentThreadId();

    [DllImport(App.Kernel32Dll, CharSet = CharSet.Unicode)]
    public unsafe static extern int lstrlen(char* lpString);

    [DllImport(App.Kernel32Dll, CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern bool SetThreadPreferredUILanguages(int dwFlags, PCZZWSTR pwszLanguagesBuffer, IntPtr pulNumLanguages);

    [DllImport(App.User32Dll)]
    public static extern IntPtr SetWinEventHook(int eventMin, int eventMax, IntPtr hmodWinEventProc, WINEVENTPROC pfnWinEventProc, int idProcess, int idThread, int dwFlags);

    [DllImport(App.User32Dll)]
    public static extern bool UnhookWinEvent(IntPtr hWinEventHook);
}
