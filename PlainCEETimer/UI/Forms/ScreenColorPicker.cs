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
        private Bitmap ScreenCut;
        private Pen CrossPen;
        private Rectangle DestRect;
        private const int HW = 64;
        private const int ScreenCutHW = 16;
        private readonly Size ScreenCutSize;
        private readonly Rectangle SourceRect;

        public Color CurrentPixelColor
        {
            get
            {
                if (ScreenCut != null)
                {
                    return ScreenCut.GetPixel(ScreenCutHW / 2, ScreenCutHW / 2);
                }

                return Color.Empty;
            }
        }

        public ScreenColorPicker() : base(AppFormParam.RoundCorner)
        {
            ScreenCut = new(ScreenCutHW, ScreenCutHW);
            ScreenCutSize = new(ScreenCutHW, ScreenCutHW);
            SourceRect = new(0, 0, ScreenCutHW, ScreenCutHW);
        }

        protected override void OnInitializing()
        {
            Text = "屏幕拾色器 - 高考倒计时";
            TopMost = true;
        }

        protected override void OnLoad()
        {
            HeightWidth = ScaleToDpi(HW);
            XY = HeightWidth / 2;
            PosOffset = ScaleToDpi(HW / 4);
            CrossPen = new(Color.Red, ScaleToDpi(1));
            Size = new(HeightWidth, HeightWidth);
            DestRect = new(0, 0, HeightWidth + 2, HeightWidth + 2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using var gg = Graphics.FromImage(ScreenCut);
            var g = e.Graphics;
            gg.CopyFromScreen(MouseX - (ScreenCutHW / 2), MouseY - (ScreenCutHW / 2), 0, 0, ScreenCutSize);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(ScreenCut, DestRect, SourceRect, GraphicsUnit.Pixel);
            g.DrawLine(CrossPen, PosOffset, XY, HeightWidth - PosOffset, XY);
            g.DrawLine(CrossPen, XY, PosOffset, XY, HeightWidth - PosOffset);
        }

        protected override void OnClosed()
        {
            ScreenCut.Dispose();
            CrossPen.Dispose();
            ScreenCut = null;
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
