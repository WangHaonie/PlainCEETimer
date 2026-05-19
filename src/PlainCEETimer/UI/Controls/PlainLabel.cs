using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

public class PlainLabel : Label, IThemeAware
{
    private bool UseDark;
    private readonly ThemeHelper themeHelper;

    public PlainLabel()
    {
        AutoSize = true;
        themeHelper = new(this);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (UseDark && !Enabled)
        {
            TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, Colors.DarkForeTextDisabled, CA2TFF(TextAlign) | TextFormatFlags.WordBreak);
        }
        else
        {
            base.OnPaint(e);
        }
    }

    protected override void Dispose(bool disposing)
    {
        themeHelper.Destroy();
        base.Dispose(disposing);
    }

    private static TextFormatFlags CA2TFF(ContentAlignment alignment) => alignment switch
    {
        ContentAlignment.TopLeft => TextFormatFlags.Top | TextFormatFlags.Left,
        ContentAlignment.TopCenter => TextFormatFlags.Top | TextFormatFlags.HorizontalCenter,
        ContentAlignment.TopRight => TextFormatFlags.Top | TextFormatFlags.Right,
        ContentAlignment.MiddleLeft => TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
        ContentAlignment.MiddleCenter => TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
        ContentAlignment.MiddleRight => TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
        ContentAlignment.BottomLeft => TextFormatFlags.Bottom | TextFormatFlags.Left,
        ContentAlignment.BottomCenter => TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter,
        ContentAlignment.BottomRight => TextFormatFlags.Bottom | TextFormatFlags.Right,
        _ => TextFormatFlags.Default
    };

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        UseDark = useDark;

        if (!init)
        {
            Invalidate();
        }
    }
}
