using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.UI.Core;

public interface IScreenService
{
    Screen[] AllScreens { get; }

    Rectangle GetWorkingArea();
}
