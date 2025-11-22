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
                target.HandleCreated += (_, _) => ThemeManager.EnableDarkModeForControl(Target.Handle, NativeStyle.DarkTheme);
            }
            else
            {
                target.EnabledChanged += (_, _) => UpdateStyle();
                UpdateStyle();
            }
        }
    }

    private void UpdateStyle()
    {
        Target.FlatStyle = Target.Enabled ? FlatStyle.Standard : FlatStyle.System;
        ThemeManager.EnableDarkModeForControl(Target, NativeStyle.ExplorerDark);
    }
}
