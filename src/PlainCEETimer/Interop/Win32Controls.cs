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
}