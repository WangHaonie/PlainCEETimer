using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Forms
{
    public sealed class ScreenColorPicker : AppForm
    {
        private int HeightWidth;
        private int MouseX;
        private int MouseY;
        private int PosOffset;
        private int XY;
        private Pen CrossPen;
        private Rectangle DestRect;
        private const int WH = 64;
        private readonly int ScreenCutWH = 16;
        private readonly Bitmap ScreenCut;
        private readonly Size ScreenCutSize;
        private readonly Size Pixel = new(1, 1);
        private readonly Rectangle SourceRect;

        public Color CurrentPixelColor
        {
            get
            {
                using var bmp = new Bitmap(1, 1);
                using var g = Graphics.FromImage(bmp);
                g.CopyFromScreen(MouseX, MouseY, 0, 0, Pixel);
                return bmp.GetPixel(0, 0);
            }
        }

        public ScreenColorPicker() : base(AppFormParam.RoundCorner)
        {
            ScreenCut = new(ScreenCutWH, ScreenCutWH);
            ScreenCutSize = new(ScreenCutWH, ScreenCutWH);
            SourceRect = new(0, 0, 16, 16);
        }

        protected override void OnInitializing()
        {
            Text = "屏幕拾色器 - 高考倒计时";
            TopMost = true;
        }

        protected override void OnLoad()
        {
            HeightWidth = ScaleToDpi(WH);
            XY = HeightWidth / 2;
            PosOffset = ScaleToDpi(WH / 4);
            CrossPen = new(Color.Red, ScaleToDpi(1));
            Size = new(HeightWidth, HeightWidth);
            DestRect = new(0, 0, HeightWidth + 1, HeightWidth + 1);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using var gg = Graphics.FromImage(ScreenCut);
            var g = e.Graphics;
            gg.CopyFromScreen(MouseX - 8, MouseY - 8, 0, 0, ScreenCutSize);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(ScreenCut, DestRect, SourceRect, GraphicsUnit.Pixel);
            g.DrawLine(CrossPen, PosOffset, XY, HeightWidth - PosOffset, XY);
            g.DrawLine(CrossPen, XY, PosOffset, XY, HeightWidth - PosOffset);
        }

        protected override void OnClosed()
        {
            ScreenCut.Dispose();
            CrossPen.Dispose();
        }

        public void UpdateFrame(Point mp)
        {
            var screen = Screen.FromPoint(mp).Bounds;
            var mx = mp.X;
            var my = mp.Y;
            int x = mx + PosOffset;
            int y = my + PosOffset;

            if (x + HeightWidth > screen.Right)
            {
                x = mx - HeightWidth - PosOffset;
            }

            if (y + HeightWidth > screen.Bottom)
            {
                y = my - HeightWidth - PosOffset;
            }

            SetLocation(x, y);
            MouseX = mx;
            MouseY = my;
            Invalidate();
        }
    }
}
