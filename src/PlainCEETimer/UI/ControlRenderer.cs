using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace PlainCEETimer.UI;

public class ControlRenderer
{
    public static void DrawGroupBox(Graphics g, string text, Font font, Rectangle rcClient, bool useDark, Color fore, Color back, bool rtl, bool cues)
    {
        //
        // 自绘 GroupBox 参考:
        //
        // How to create a Custom group box control with border color - Vb.net @mikecodz2821 - YouTube
        // https://www.youtube.com/watch?v=_7NwVqfNU1g
        //

        var textHeight = font.Height / 2 + 2;
        var rect = new Rectangle(0, textHeight, rcClient.Width, rcClient.Height - textHeight);
        var textRect = Rectangle.Inflate(rcClient, -6, 0);

        ControlPaint.DrawBorder(g, rect, useDark ? Colors.DarkBorder : Colors.LightBorder, ButtonBorderStyle.Solid);

        using var fbrush = new SolidBrush(fore);
        using var bbrush = new SolidBrush(back);
        using var sf = new StringFormat();
        sf.HotkeyPrefix = cues ? HotkeyPrefix.Show : HotkeyPrefix.Hide;
        if (rtl) sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

        var sz = g.MeasureString(text, font);
        g.FillRectangle(bbrush, textRect.X, textRect.Y, sz.Width, sz.Height);
        g.DrawString(text, font, fbrush, textRect, sf);
    }
}
