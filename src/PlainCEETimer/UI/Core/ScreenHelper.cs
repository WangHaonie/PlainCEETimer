using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.UI.Core;

public class ScreenHelper(IAppWindow window) : IScreenService
{
    public Screen Screen => window == null ? Screen.FromPoint(Cursor.Position) : Screen.FromHandle(window.Handle);

    public Screen[] AllScreens => Screen.AllScreens;

    public TaskbarPosition TaskbarPosition => GetTaskbarPosition(Screen);

    public virtual Rectangle WorkingArea => Screen.WorkingArea;

    private TaskbarPosition GetTaskbarPosition(Screen screen)
    {
        var w = screen.WorkingArea;
        var b = screen.Bounds;

        if (w.Top > b.Top)
        {
            return TaskbarPosition.Top;
        }

        if (w.Left > b.Left)
        {
            return TaskbarPosition.Left;
        }

        if (w.Right < b.Right)
        {
            return TaskbarPosition.Right;
        }

        if (w.Bottom < b.Bottom)
        {
            return TaskbarPosition.Bottom;
        }

        return TaskbarPosition.None;
    }
}