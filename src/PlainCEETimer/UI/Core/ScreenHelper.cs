using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.UI.Core;

public class ScreenHelper(IAppWindow window = null) : IScreenService
{
    public Screen[] AllScreens => Screen.AllScreens;

    public virtual Rectangle GetWorkingArea()
    {
        return window == null ? Screen.GetWorkingArea(Cursor.Position) : Screen.FromHandle(window.Handle).WorkingArea;
    }
}