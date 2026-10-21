using System;
using System.Reflection;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Reflection;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainToolTip : ToolTip, IThemeAwareEx
{
    public IntPtr Handle
    {
        get
        {
            if (m_piHandle == null)
            {
                typeof(ToolTip).RevealMember(out m_piHandle);
            }

            return (IntPtr)m_piHandle.GetValue(this);
        }
    }

    private static PropertyInfo m_piHandle;

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
        ToolTipStyles.ApplyStyle(Handle, useDark, init);
    }
}
