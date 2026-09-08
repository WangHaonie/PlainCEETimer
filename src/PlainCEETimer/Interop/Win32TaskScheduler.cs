using System.Runtime.InteropServices;
using System.Security;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

/// <summary>
/// 为 Windows 任务计划程序 (<a href="https://learn.microsoft.com/zh-cn/windows/win32/api/taskschd/nn-taskschd-itaskservice">ITaskService</a>) 提供低级封装。
/// </summary>
[SuppressUnmanagedCodeSecurity]
internal static class Win32TaskScheduler
{
    [DllImport(App.NativesDll, EntryPoint = "#30")]
    public static extern void Initialize();

    [DllImport(App.NativesDll, EntryPoint = "#31", CharSet = CharSet.Unicode)]
    public static extern void Import(string path, string xmlText, TaskLogonType logonType);

    [DllImport(App.NativesDll, EntryPoint = "#32", CharSet = CharSet.Unicode)]
    public static extern bool Export(string path, [MarshalAs(UnmanagedType.BStr)] out string pXml);

    [DllImport(App.NativesDll, EntryPoint = "#33", CharSet = CharSet.Unicode)]
    public static extern bool Exists(string path);

    [DllImport(App.NativesDll, EntryPoint = "#34", CharSet = CharSet.Unicode)]
    public static extern void Enable(string path);

    [DllImport(App.NativesDll, EntryPoint = "#35", CharSet = CharSet.Unicode)]
    public static extern void Delete(string path);

    [DllImport(App.NativesDll, EntryPoint = "#36")]
    public static extern void Release();
}
