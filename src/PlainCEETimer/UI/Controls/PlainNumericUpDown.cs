using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainNumericUpDown : NumericUpDown, IThemeAware
{
    private ThemeHelper themeHelper;
    private readonly Debouncer debouncer;
    private readonly ControlDebounceHelper debounceHelper;
    private readonly ActionInvoker<EventArgs> OnValueChangedInvoker;

    public PlainNumericUpDown()
    {
        TextAlign = HorizontalAlignment.Right;
        debouncer = new();
        debounceHelper = new(this);
        OnValueChangedInvoker = new(base.OnValueChanged);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        themeHelper ??= new(this);
        base.OnHandleCreated(e);
    }

    protected override void OnValueChanged(EventArgs e)
    {
        if (!debounceHelper.ShouldDebounce)
        {
            base.OnValueChanged(e);
            return;
        }

        debouncer.Debounce(OnValueChangedInvoker.WithArgs(e));
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
