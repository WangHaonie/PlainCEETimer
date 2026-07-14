namespace System
{
    /// <summary>
    /// public instance class internal static method.
    /// </summary>
    internal delegate string String_FastAllocateString(int length);
}

namespace System.Windows.Forms
{
    using System.Drawing;

    /// <summary>
    /// internal static class public static method.
    /// </summary>
    internal delegate bool Command_DispatchID(int id);

    /// <summary>
    /// internal static class public static method.
    /// </summary>
    internal delegate int DpiHelper_LogicalToDeviceUnits1(int value, int devicePixels = 0);

    /// <summary>
    /// internal static class public static method.
    /// </summary>
    internal delegate Size DpiHelper_LogicalToDeviceUnits2(Size logicalSize, int deviceDpi = 0);

    /// <summary>
    /// public instance class internal instance method.
    /// </summary>
    internal delegate StringFormat Label_CreateStringFormat();
}

namespace System.Windows.Input
{
    /// <summary>
    /// public static class internal static method.
    /// </summary>
    internal delegate string ApplicationCommands_GetUIText(byte commandId);

    /// <summary>
    /// public static class internal static method.
    /// </summary>
    internal delegate InputGestureCollection ApplicationCommands_LoadDefaultGestureFromResource(byte commandId);
}
