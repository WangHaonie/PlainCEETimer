using System.Drawing;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Core;

public class WinFormsBorderColorService(AppForm form) : IBorderColorService
{
    private readonly bool Allowed = SystemVersion.IsWindows11;

    public void SetBorderColor(bool enabled, Color color)
    {
        if (Allowed)
        {
            Win32UI.SetBorderColor(form.Handle, color, enabled);
        }
    }
}
