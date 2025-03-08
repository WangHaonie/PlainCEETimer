using System.Drawing;

namespace PlainCEETimer.Modules.Extensions
{
    public static class ColorExtensions
    {
        public static string ToArgbString(this Color color) => color.ToArgbInt().ToString();

        public static int ToArgbInt(this Color color) => -color.ToArgb();
    }
}
