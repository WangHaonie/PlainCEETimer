using System.Windows.Forms;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI;

public class TextBoxFlyoutEventArgs(int textLength, PlainTextBox target, KeyEventArgs e)
{
    public int TextLength => textLength;
    public PlainTextBox Target => target;
    public KeyEventArgs KeyEventArgs => e;
}
