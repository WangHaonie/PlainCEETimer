using PlainCEETimer.Interop;
using System.Windows.Forms;

namespace PlainCEETimer.Modules.Extensions
{
    public static class DpiExtensions
    {
        public static double DpiRatio { get; private set; } = 0;

        public static int ScaleToDpi(this int px, Control ctrl)
        {
            int pxScaled;
            DpiRatio = NativeInterop.GetDpiForWindow(ctrl.Handle) / 96D;
            pxScaled = (int)(px * DpiRatio);
            return pxScaled;
        }
    }
}
