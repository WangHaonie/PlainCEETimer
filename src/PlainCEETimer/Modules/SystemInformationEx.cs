using System.Drawing;
using PlainCEETimer.Interop;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules;

public static class SystemInformationEx
{
    public static Size GetBorder3DSizeForDpi(int? dpi = null)
    {
        return new(
            DpiHelperEx.GetSystemMetricsForDpi(WinUser.SM_CXEDGE, dpi),
            DpiHelperEx.GetSystemMetricsForDpi(WinUser.SM_CYEDGE, dpi)
        );
    }
}