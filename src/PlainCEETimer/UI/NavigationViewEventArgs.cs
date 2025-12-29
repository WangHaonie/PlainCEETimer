using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI;

public class NavigationViewEventArgs(int index, NavigationPage page)
{
    public int Index => index;

    public NavigationPage Page => page;
}