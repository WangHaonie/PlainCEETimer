using System.Drawing;

namespace PlainCEETimer.UI.Core;

public class WPFScreenHelper(IAppWindow window = null) : ScreenHelper(window)
{
    public override Rectangle GetWorkingArea()
    {
        var rect = base.GetWorkingArea();
        rect.Height -= 1;
        return rect;
    }
}