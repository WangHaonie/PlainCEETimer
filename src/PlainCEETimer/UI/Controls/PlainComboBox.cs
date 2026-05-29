using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainComboBox : ComboBox, IThemeAware
{
    private bool Calculated;
    private ThemeHelper themeHelper;
    private readonly Debouncer debouncer;
    private readonly ControlDebounceHelper debounceHelper;

    public PlainComboBox()
    {
        DropDownStyle = ComboBoxStyle.DropDownList;
        FlatStyle = FlatStyle.System;
        debounceHelper = new(this);
        debouncer = new(base.OnSelectedIndexChanged);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        themeHelper ??= new(this);
        base.OnHandleCreated(e);
    }

    protected override void OnDropDown(EventArgs e)
    {
        /*

        DropDown 自适应大小 参考:

        c# - Auto-width of ComboBox's content - Stack Overflow
        https://stackoverflow.com/a/16435431/21094697

        c# - ComboBox auto DropDownWidth regardless of DataSource type - Stack Overflow
        https://stackoverflow.com/a/69350288/21094697

         */

        if (!Calculated)
        {
            var w = 0;
            var vsbw = SystemInformation.GetVerticalScrollBarWidthForDpi(DeviceDpi);

            foreach (var item in Items)
            {
                w = Math.Max(w, TextRenderer.MeasureText(GetItemText(item), Font).Width);
            }

            DropDownWidth = w + vsbw;
            Calculated = true;
        }

        base.OnDropDown(e);
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        Calculated = false;
        base.OnDpiChangedAfterParent(e);
    }

    protected override void OnSelectedIndexChanged(EventArgs e)
    {
        if (!debounceHelper.ShouldDebounce)
        {
            base.OnSelectedIndexChanged(e);
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
        ThemeManager.EnableDarkModeForControl(this, useDark ? SystemStyle.CfdDark : SystemStyle.Cfd);
    }
}
