using System;
using System.Reflection;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainToolTip : ToolTip, IThemeAware
{
    public IntPtr Handle
    {
        get
        {
            m_piHandle ??= typeof(ToolTip).GetProperty(nameof(Handle), BindingFlags.NonPublic | BindingFlags.Instance);
            return (IntPtr)m_piHandle.GetValue(this);
        }
    }

    private ThemeHelper themeHelper;
    private PropertyInfo m_piHandle;

    public void InitStyle()
    {
        var hwnd = Handle;

        if (SystemVersion.IsWindows11)
        {
            Win32UI.SetRoundCornerEx(hwnd, true);
        }

        themeHelper ??= new(this);
    }

    protected override void Dispose(bool disposing)
    {
        themeHelper.Destroy();
        base.Dispose(disposing);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        ThemeManager.EnableDarkModeForControl(Handle, useDark ? SystemStyle.ExplorerDark : SystemStyle.Explorer);
    }
}
