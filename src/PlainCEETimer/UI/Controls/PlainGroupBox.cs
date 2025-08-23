using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI.Controls
{
    public sealed class PlainGroupBox : GroupBox
    {
        public PlainGroupBox()
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                ForeColor = Colors.DarkForeText;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (ThemeManager.ShouldUseDarkMode)
            {
                //
                // 深色模式下 GroupBox 应使用灰色边框 参考:
                //
                // How to create a Custom group box control with border color - Vb.net @mikecodz2821 - YouTube
                // https://www.youtube.com/watch?v=_7NwVqfNU1g
                //

                var g = e.Graphics;
                var client = ClientSize;
                var text = Text;
                var font = Font;
                var textHeight = Font.Height / 2 + 2;
                var rect = new Rectangle(0, textHeight, client.Width, client.Height - textHeight);
                var textRect = Rectangle.Inflate(ClientRectangle, -4, 0);

                ControlPaint.DrawBorder(g, rect, Colors.DarkBorder, ButtonBorderStyle.Solid);
                TextRenderer.DrawText(g, text, font, textRect, ForeColor, BackColor, TextFormatFlags.Top | TextFormatFlags.Left | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.EndEllipsis);
            }
            else
            {
                base.OnPaint(e);
            }
        }
    }
}
