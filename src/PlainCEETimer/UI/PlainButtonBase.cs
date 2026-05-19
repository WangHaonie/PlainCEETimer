using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI;

public class PlainButtonBase : IThemeAware
{
    private readonly ThemeHelper themeHelper;
    private readonly ButtonBase Target;
    private bool UseDark;

    public PlainButtonBase(ButtonBase target)
    {
        Target = target;
        target.UseVisualStyleBackColor = true;
        target.FlatStyle = FlatStyle.System;
        themeHelper = new(this);
    }

    private void UpdateStyle()
    {
        Target.FlatStyle = UseDark && Target.Enabled ? FlatStyle.Standard : FlatStyle.System;
        ThemeManager.EnableDarkModeForControl(Target, UseDark ? SystemStyle.ExplorerDark : SystemStyle.Explorer);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        UseDark = useDark;

        if (!init)
        {
            if (ThemeManager.NewThemeAvailable)
            {
                ThemeManager.EnableDarkModeForControl(Target.Handle, useDark ? SystemStyle.DarkTheme : SystemStyle.Explorer);
            }
            else
            {
                UpdateStyle();
            }
        }

        if (init)
        {
            if (ThemeManager.NewThemeAvailable)
            {
                Target.HandleCreated += (_, _) => ((IThemeAware)this).UpdateTheme(useDark, !init);
            }
            else
            {
                Target.EnabledChanged += (_, _) => ((IThemeAware)this).UpdateTheme(useDark, !init);
                UpdateStyle();
            }

            Target.Disposed += (_, _) => themeHelper.Destroy();
        }
    }
}
