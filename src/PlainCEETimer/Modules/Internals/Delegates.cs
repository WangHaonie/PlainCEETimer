using WFSize = System.Drawing.Size;

namespace System.Windows.Forms
{
    internal delegate bool Command_DispatchID(int id);

    internal delegate int DpiHelper_LogicalToDeviceUnits1(int value, int devicePixels = 0);

    internal delegate WFSize DpiHelper_LogicalToDeviceUnits2(WFSize logicalSize, int deviceDpi = 0);

    internal delegate string MessageDecoder_MsgToString(int msg);
}