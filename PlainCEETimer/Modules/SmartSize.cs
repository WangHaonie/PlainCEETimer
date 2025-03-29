using PlainCEETimer.Modules.Extensions;
using System.Drawing;

namespace PlainCEETimer.Modules
{
    public readonly struct SmartSize(int width, int height)
    {
        private readonly int Width = width;
        private readonly int Height = height;

        public static implicit operator Size(SmartSize smart)
            => new(smart.Width.ScaleToDpi(), smart.Height.ScaleToDpi());
    }
}
