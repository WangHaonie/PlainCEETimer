using System.Drawing;
using PlainCEETimer.Interop;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules;

public static class SystemInformationEx
{
    public static Size GetBorder3DSizeForDpi(int? dpi = null)
    {
        return new(
            DpiHelperEx.GetSystemMetricsForDpi(SystemMetric.CXEDGE, dpi),
            DpiHelperEx.GetSystemMetricsForDpi(SystemMetric.CYEDGE, dpi)
        );
    }
}