namespace System.Windows.Forms
{
    using System.Drawing;

    internal delegate bool Command_DispatchID(int id);

    internal delegate int DpiHelper_LogicalToDeviceUnits1(int value, int devicePixels = 0);

    internal delegate Size DpiHelper_LogicalToDeviceUnits2(Size logicalSize, int deviceDpi = 0);

    internal delegate string MessageDecoder_MsgToString(int msg);

    internal delegate StringFormat Label_CreateStringFormat();
}
