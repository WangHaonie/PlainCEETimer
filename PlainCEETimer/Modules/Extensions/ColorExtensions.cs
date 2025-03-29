using System.Drawing;

namespace PlainCEETimer.Modules.Extensions
{
    public static class ColorExtensions
    {
        public static int ToArgbInt(this Color color)
            => -color.ToArgb();
    }
}
