using System;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainGroupBox : GroupBox, IThemeAware
{
    private bool UseDark;
    private ThemeHelper themeHelper;

    public PlainGroupBox()
    {
        if (ThemeManager.NewThemeAvailable)
        {
            FlatStyle = FlatStyle.System;
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        themeHelper ??= new(this);
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

    protected override void Dispose(bool disposing)
    {
        themeHelper.Destroy();
        base.Dispose(disposing);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        UseDark = useDark;
        ForeColor = useDark ? Colors.DarkForeText : DefaultForeColor;

        if (ThemeManager.NewThemeAvailable)
        {
            ThemeManager.EnableDarkModeForControl(this, useDark ? SystemStyle.DarkTheme : SystemStyle.Explorer);
        }
        else if (!init)
        {
            Invalidate();
        }
    }
}
