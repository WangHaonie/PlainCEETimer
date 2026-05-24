using System.Windows.Forms;

namespace PlainCEETimer.UI.Core;

public class ScreenChangedEventArgs(Screen oldScreen, Screen newScreen)
{
    public Screen ScreenOld => oldScreen;

    public Screen ScreenNew => newScreen;
}
