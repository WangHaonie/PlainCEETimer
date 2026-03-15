using System.Drawing;

namespace PlainCEETimer.UI.Core;

public class WPFScreenHelper(IAppWindow window) : ScreenHelper(window)
{
    public override Rectangle WorkingArea
    {
        get
        {
            var rect = base.WorkingArea;
            rect.Height -= 1;
            return rect;
        }
    }
}