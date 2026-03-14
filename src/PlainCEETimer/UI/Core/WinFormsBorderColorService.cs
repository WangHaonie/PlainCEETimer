using System.Drawing;
using PlainCEETimer.Interop;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Core;

public class WinFormsBorderColorService(AppForm form) : IBorderColorService
{
    public void SetBorderColor(bool enabled, Color color)
    {
        Win32UI.SetBorderColor(form.Handle, color, enabled);
    }
}
