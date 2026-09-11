using System;
using System.Runtime.InteropServices;
using System.Security;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.Fody;

namespace PlainCEETimer.Interop;

[NoConstants]
[SuppressUnmanagedCodeSecurity]
internal unsafe static class Win32Controls
{
    public const string WC_PLAINTIMESPANPICK = "PlainTimeSpanPick";

    [DllImport(App.NativesDll, EntryPoint = "#57")]
    public static extern ushort PlainTimeSpanPick_InitClass();

    [DllImport(App.NativesDll, EntryPoint = "#58", CharSet = CharSet.Unicode)]
    public static extern bool PlainTimeSpanPick_ValidateFormat(string pszFormat);

    /// <summary>
    /// 解析适用于 <see cref="TimeSpan"/> 对象的格式字符串。
    /// </summary>
    /// <param name="pszFormat">格式字符串</param>
    /// <returns>格式字符串解析结果</returns>
    [DllImport(App.NativesDll, EntryPoint = "#59", CharSet = CharSet.Unicode)]
    public static extern IntPtr PlainTimeSpanPick_ParseFormat(string pszFormat);

    /// <summary>
    /// 格式化一个 <see cref="TimeSpan"/> 对象；
    /// 当执行操作时 (in)，<paramref name="lpcchBuffer"/> 需要包含末尾空字符；
    /// 仅探测时 (out)，<paramref name="lpcchBuffer"/> 不包含末尾空字符。
    /// </summary>
    /// <param name="hFormat">格式字符串解析结果</param>
    /// <param name="lptsValue">要执行格式化的 <see cref="TimeSpan"/> 对象</param>
    /// <param name="lpBuffer">缓冲区</param>
    /// <param name="lpcchBuffer">缓冲区大小</param>
    /// <returns>操作是否成功</returns>
    [DllImport(App.NativesDll, EntryPoint = "#60", CharSet = CharSet.Unicode)]
    public static extern bool PlainTimeSpanPick_Format(IntPtr hFormat, ref TimeSpan lptsValue, char* lpBuffer, ref int lpcchBuffer);

    /// <summary>
    /// 销毁适用于 <see cref="TimeSpan"/> 对象的格式字符串解析结果
    /// </summary>
    /// <param name="hFormat">格式字符串解析结果</param>
    /// <returns>操作是否成功</returns>
    [DllImport(App.NativesDll, EntryPoint = "#61")]
    public static extern bool PlainTimeSpanPick_FreeMemory(IntPtr hFormat);

    [DllImport(App.NativesDll, EntryPoint = "#62")]
    public static extern bool PlainTimeSpanPick_SuggestValue(ref TimeSpan lptsValue);

    [DllImport(App.NativesDll, EntryPoint = "#17")]
    public static extern int CDCCM_WmContextMenu(IntPtr hWnd, IntPtr wParam, IntPtr lParam);
}