using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainNumericUpDown : NumericUpDown, IThemeAware
{
    private ThemeHelper themeHelper;

    public PlainNumericUpDown()
    {
        TextAlign = HorizontalAlignment.Right;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        themeHelper ??= new(this);
        base.OnHandleCreated(e);
    }

    protected override void Dispose(bool disposing)
    {
        themeHelper.Destroy();
        base.Dispose(disposing);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        ForeColor = useDark ? Colors.DarkForeText : SystemColors.WindowText;
        BackColor = useDark ? Colors.DarkBackText : SystemColors.Window;

        var ctrls = Controls;
        var count = ctrls.Count;

        for (int i = 0; i < count; i++)
        {
            ThemeManager.EnableDarkModeForControl(ctrls[i], useDark ? SystemStyle.ExplorerDark : SystemStyle.Explorer);
        }
    }
}
