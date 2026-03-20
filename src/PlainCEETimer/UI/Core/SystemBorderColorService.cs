using System.Drawing;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Core;

public class SystemBorderColorService(IAppWindow window) : IBorderColorService
{
    private readonly bool Allowed = SystemVersion.IsWindows11;

    public bool SetBorderColor(bool enabled, Color color)
    {
        if (Allowed)
        {
            Win32UI.SetBorderColor(window.Handle, color, enabled);
            return true;
        }

        return false;
    }
}
