using System.Drawing;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Core;

public class WPFScreenHelper(IAppWindow window) : ScreenHelper(window)
{
    public override Rectangle WorkingArea
    {
        get
        {
            var rect = base.WorkingArea;

            if (SystemVersion.IsWindows11)
            {
                switch (TaskbarPosition)
                {
                    case TaskbarPosition.Left:
                        rect.X += 1;
                        rect.Width -= 1;
                        break;
                    case TaskbarPosition.Top:
                        rect.Y += 1;
                        rect.Height -= 1;
                        break;
                    case TaskbarPosition.Right:
                        rect.Width -= 1;
                        break;
                    case TaskbarPosition.Bottom:
                        rect.Height -= 1;
                        break;
                }
            }

            return rect;
        }
    }
}