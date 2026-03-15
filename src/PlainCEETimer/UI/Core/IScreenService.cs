using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.UI.Core;

public interface IScreenService
{
    Screen Screen { get; }

    Screen[] AllScreens { get; }

    TaskbarPosition TaskbarPosition { get; }

    Rectangle WorkingArea { get; }
}
