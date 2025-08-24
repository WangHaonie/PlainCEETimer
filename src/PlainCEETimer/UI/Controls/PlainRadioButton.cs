using System.Windows.Forms;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainRadioButton : RadioButton
{
    public PlainRadioButton()
    {
        new PlainButtonBase(this);
    }
}
