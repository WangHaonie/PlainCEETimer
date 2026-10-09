using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainNumericUpDown : NumericUpDown, IThemeAwareEx
{
    private readonly Debouncer debouncer;
    private readonly ActionInvoker<EventArgs> OnValueChangedInvoker;

    public PlainNumericUpDown()
    {
        TextAlign = HorizontalAlignment.Right;
        debouncer = new(new ControlDebounceHelper(this));
        OnValueChangedInvoker = new(base.OnValueChanged);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        ThemeHelper.Attach(this);
        base.OnHandleCreated(e);
    }

    protected override void OnValueChanged(EventArgs e)
    {
        debouncer.Debounce(OnValueChangedInvoker.WithArgs(e));
    }

    protected override void Dispose(bool disposing)
    {
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
            ThemeManager.ApplyControlTheme(ctrls[i], useDark ? SystemStyle.ExplorerDark : SystemStyle.Explorer);
        }
    }
}
