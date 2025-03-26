using System.Drawing;

namespace PlainCEETimer.Modules.Extensions
{
    public static class DpiExtensions
    {
        public static double DpiRatio { get; set; } = 0D;

        public static int ScaleToDpi(this int px)
            => (int)(px * DpiRatio);

        public static Size ScaleToDpi(this Size size)
            => new(size.Width.ScaleToDpi(), size.Height.ScaleToDpi());
    }
}
