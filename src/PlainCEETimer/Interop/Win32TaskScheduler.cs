using System.Runtime.InteropServices;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Interop;

/// <summary>
/// 为 Windows 任务计划程序 (<a href="https://learn.microsoft.com/zh-cn/windows/win32/api/taskschd/nn-taskschd-itaskservice">ITaskService</a>) 提供低级封装。
/// </summary>
public static class Win32TaskScheduler
{
    [DllImport(App.NativesDll, EntryPoint = "#10")]
    public static extern void Initialize();

    [DllImport(App.NativesDll, EntryPoint = "#11", CharSet = CharSet.Unicode)]
    public static extern void Import(string path, string xmlText, TaskLogonType logonType);

    [DllImport(App.NativesDll, EntryPoint = "#12", CharSet = CharSet.Unicode)]
    public static extern void Export(string path, [MarshalAs(UnmanagedType.BStr)] out string pXml);

    [DllImport(App.NativesDll, EntryPoint = "#13", CharSet = CharSet.Unicode)]
    public static extern void Enable(string path);

    [DllImport(App.NativesDll, EntryPoint = "#14", CharSet = CharSet.Unicode)]
    public static extern void Delete(string path);

    [DllImport(App.NativesDll, EntryPoint = "#15")]
    public static extern void Release();
}
