using System;
using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainButton : Button
{
    public PlainButton()
    {
        FlatStyle = FlatStyle.System;
        UseVisualStyleBackColor = true;
        DoubleBuffered = true;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        if (ThemeManager.ShouldUseDarkMode)
        {
            ThemeManager.EnableDarkMode(this, NativeStyle.ExplorerDark);
        }

        base.OnHandleCreated(e);
    }

    protected override void OnClick(EventArgs e)
    {
        ContextMenu?.Show(this, new(0, Height));
        base.OnClick(e);
    }
}
