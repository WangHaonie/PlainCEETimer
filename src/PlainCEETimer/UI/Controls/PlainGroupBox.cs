using System;
using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainGroupBox : GroupBox, IThemeAwareEx
{
    private bool UseDark;

    public PlainGroupBox()
    {
        if (ThemeManager.NewThemeAvailable)
        {
            FlatStyle = FlatStyle.System;
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        ThemeHelper.Attach(this);
        base.OnHandleCreated(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (!ThemeManager.NewThemeAvailable)
        {
            ControlRenderer.DrawGroupBox(e.Graphics,
                Text, Font, ClientRectangle,
                UseDark, ForeColor, BackColor,
                RightToLeft == RightToLeft.Yes, ShowKeyboardCues);
        }
        else
        {
            base.OnPaint(e);
        }
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        UseDark = useDark;
        ForeColor = useDark ? Colors.DarkForeText : DefaultForeColor;

        if (ThemeManager.NewThemeAvailable)
        {
            ThemeManager.ApplyControlTheme(this, useDark ? SystemStyle.DarkTheme : SystemStyle.Explorer);
        }
        else if (!init)
        {
            Invalidate();
        }
    }
}
