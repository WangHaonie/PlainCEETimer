using System;
using System.Runtime.InteropServices;
using System.Security;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

[SuppressUnmanagedCodeSecurity]
internal unsafe static class Win32
{
    [DllImport(App.NativesDll, EntryPoint = "#54")]
    public static extern IntPtr PnAllocConsole(bool bRefresh, out IntPtr phStdIn, out IntPtr phStdOut, out IntPtr phStdErr);

    [DllImport(App.NativesDll, EntryPoint = "#55")]
    public static extern void PnKillProcessTree(int dwProcessId);

    [DllImport(App.NativesDll, EntryPoint = "#56")]
    public static extern int PnLoadStringInternal(uint uID, out IntPtr ppBuffer);

    [DllImport(App.NativesDll, EntryPoint = "#63")]
    public static extern bool PnGetConsoleColor(IntPtr hConsoleHandle, COLORREF color, out ConsoleColor lpdwConsoleColor);

    [DllImport(App.NativesDll, EntryPoint = "#64")]
    public static extern bool PnSetConsoleMode(IntPtr hConsoleHandle, int dwFlags);

    [DllImport(App.NativesDll, EntryPoint = "#65")]
    public static extern bool PnBuildAnsiColorString(char* lpBuffer, int dwCchBuffer, int* lpData);

    [DllImport(App.NativesDll, EntryPoint = "#66")]
    public static extern uint PnConsoleGetCursor(IntPtr hConsoleHandle);

    [DllImport(App.NativesDll, EntryPoint = "#67")]
    public static extern bool PnConsoleClear(IntPtr hConsoleHandle, uint coFrom, uint coTo);

    [DllImport(App.Kernel32Dll, CharSet = CharSet.Unicode)]
    public static extern bool SetConsoleTitle(string lpConsoleTitle);

    [DllImport(App.Kernel32Dll)]
    public static extern ulong GetTickCount64();

    [DllImport(App.Kernel32Dll)]
    public static extern void ExitProcess(int uExitCode);

    [DllImport(App.Kernel32Dll)]
    public static extern int GetCurrentProcessId();

    [DllImport(App.Kernel32Dll)]
    public static extern int GetCurrentThreadId();

    [DllImport(App.Kernel32Dll, CharSet = CharSet.Unicode)]
    public static extern int lstrlen(char* lpString);

    [DllImport(App.Kernel32Dll, CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern bool SetThreadPreferredUILanguages(int dwFlags, PCZZWSTR pwszLanguagesBuffer, IntPtr pulNumLanguages);

    [DllImport(App.Kernel32Dll, CharSet = CharSet.Unicode)]
    public static extern int GetShortPathName(char* lpszLongPath, char* lpszShortPath, int cchBuffer);

    [DllImport(App.Shell32Dll, CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern bool PathYetAnotherMakeUniqueName(char* pszUniqueName, string pszPath, nint pszShort, string pszFileSpec);

    [DllImport(App.ShlwapiDll, CharSet = CharSet.Unicode)]
    public static extern bool PathCompactPathEx(char* pszOut, string pszSrc, int cchMax, int dwFlags);

    [DllImport(App.User32Dll)]
    public static extern IntPtr SetWinEventHook(int eventMin, int eventMax, IntPtr hmodWinEventProc, WINEVENTPROC pfnWinEventProc, int idProcess, int idThread, int dwFlags);

    [DllImport(App.User32Dll)]
    public static extern bool UnhookWinEvent(IntPtr hWinEventHook);
}
