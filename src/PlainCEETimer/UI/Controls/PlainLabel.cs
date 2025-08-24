using System.Windows.Forms;

namespace PlainCEETimer.UI.Controls;

public class PlainLabel : Label
{
    public PlainLabel(string text)
    {
        Text = text;
        AutoSize = true;
    }
}
