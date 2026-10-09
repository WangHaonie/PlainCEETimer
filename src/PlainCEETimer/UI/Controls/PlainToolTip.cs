using System;
using System.Reflection;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainToolTip : ToolTip, IThemeAwareEx
{
    public IntPtr Handle
    {
        get
        {
            m_piHandle ??= typeof(ToolTip).GetProperty(nameof(Handle), BindingFlags.NonPublic | BindingFlags.Instance);
            return (IntPtr)m_piHandle.GetValue(this);
        }
    }

    private PropertyInfo m_piHandle;

    public void InitStyle()
    {
        var hwnd = Handle;

        if (SystemVersion.IsWindows11)
        {
            Win32UI.PnSetRoundCornerEx(hwnd, true);
        }

        ThemeHelper.Attach(this);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        ThemeManager.ApplyControlTheme(Handle, useDark ? SystemStyle.ExplorerDark : SystemStyle.Explorer);
    }
}
