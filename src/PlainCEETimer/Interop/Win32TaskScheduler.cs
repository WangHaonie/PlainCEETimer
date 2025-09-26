using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

public static class Win32TaskScheduler
{
    [DllImport(App.NativesDll, EntryPoint = "#16")]
    public static extern void Initialize();

    [DllImport(App.NativesDll, EntryPoint = "#17", CharSet = CharSet.Unicode)]
    public static extern void Import(string path, string xmlText, TaskLogonType logonType);

    [DllImport(App.NativesDll, EntryPoint = "#18", CharSet = CharSet.Unicode)]
    public static extern void Export(string path, [MarshalAs(UnmanagedType.BStr)] out string pXml);

    [DllImport(App.NativesDll, EntryPoint = "#19", CharSet = CharSet.Unicode)]
    public static extern void Enable(string path);

    [DllImport(App.NativesDll, EntryPoint = "#20", CharSet = CharSet.Unicode)]
    public static extern void Delete(string path);

    [DllImport(App.NativesDll, EntryPoint = "#21")]
    public static extern void Release();
}
