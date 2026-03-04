using System.Windows.Media;
using WFColor = System.Drawing.Color;

namespace PlainCEETimer.WPF.Extensions;

public static class Extensions
{
    public static Color ToColor(this WFColor color)
    {
        return Color.FromArgb(color.A, color.R, color.G, color.B);
    }
}