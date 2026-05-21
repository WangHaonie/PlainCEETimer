using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainNumericUpDown : NumericUpDown, IThemeAware
{
    private bool initCall = true;
    private ThemeHelper themeHelper;
    private readonly Debouncer<EventArgs> debouncer;

    public PlainNumericUpDown()
    {
        TextAlign = HorizontalAlignment.Right;
        debouncer = new(base.OnValueChanged);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        themeHelper ??= new(this);
        base.OnHandleCreated(e);
    }

    protected override void OnValueChanged(EventArgs e)
    {
        if (initCall)
        {
            base.OnValueChanged(e);
            initCall = false;
            return;
        }

        debouncer.Debounce(e);
    }

    protected override void Dispose(bool disposing)
    {
        themeHelper.Destroy();
        debouncer.Destroy();
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
