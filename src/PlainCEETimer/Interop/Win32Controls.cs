using System;
using System.Runtime.InteropServices;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Annotations.Fody;

namespace PlainCEETimer.Interop;

[NoConstants]
internal static class Win32Controls
{
    public const string WC_PLAINTIMESPANPICK = "PlainTimeSpanPick";

    [DllImport(App.NativesDll, EntryPoint = "#47", CharSet = CharSet.Unicode)]
    public static extern ushort PlainTimeSpanPick_RegisterWC();

    [DllImport(App.NativesDll, EntryPoint = "#52", CharSet = CharSet.Unicode)]
    public static extern bool PlainTimeSpanPick_ValidateFormat(string pszFormat);

    [DllImport(App.NativesDll, EntryPoint = "#53", CharSet = CharSet.Unicode)]
    public static extern IntPtr PlainTimeSpanPick_ParseFormat(string pszFormat);

    [DllImport(App.NativesDll, EntryPoint = "#54", CharSet = CharSet.Unicode)]
    public unsafe static extern bool PlainTimeSpanPick_Format(IntPtr hFormat, ref TimeSpan lptsValue, char* lpBuffer, ref int lpcchBuffer);

    [DllImport(App.NativesDll, EntryPoint = "#55")]
    public static extern bool PlainTimeSpanPick_FreeMemory(IntPtr hFormat);

    [DllImport(App.NativesDll, EntryPoint = "#57")]
    public static extern bool PlainTimeSpanPick_SuggestValue(ref TimeSpan lptsValue);
}