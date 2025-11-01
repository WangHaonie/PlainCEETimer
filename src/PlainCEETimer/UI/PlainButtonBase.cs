using System;
using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI;

public class PlainButtonBase
{
    private readonly ButtonBase Target;

    public PlainButtonBase(ButtonBase target)
    {
        Target = target;
        target.UseVisualStyleBackColor = true;
        target.FlatStyle = FlatStyle.System;

        if (ThemeManager.ShouldUseDarkMode)
        {
            if (ThemeManager.NewThemeAvailable)
            {
                target.HandleCreated += Button_HandleCreated;
            }
            else
            {
                target.EnabledChanged += Button_EnabledChanged;
                UpdateStyle();
            }
        }
    }

    private void Button_HandleCreated(object sender, EventArgs e)
    {
        ThemeManager.EnableDarkModeForControl(Target.Handle, NativeStyle.DarkTheme);
    }

    private void Button_EnabledChanged(object sender, EventArgs e)
    {
        UpdateStyle();
    }

    private void UpdateStyle()
    {
        Target.FlatStyle = Target.Enabled ? FlatStyle.Standard : FlatStyle.System;
        ThemeManager.EnableDarkModeForControl(Target, NativeStyle.ExplorerDark);
    }
}
