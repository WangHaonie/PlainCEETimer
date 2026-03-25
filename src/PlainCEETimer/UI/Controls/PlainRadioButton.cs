using System.Windows.Forms;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainRadioButton : RadioButton
{
    public PlainRadioButton()
    {
        _ = new PlainButtonBase(this);
    }
}
