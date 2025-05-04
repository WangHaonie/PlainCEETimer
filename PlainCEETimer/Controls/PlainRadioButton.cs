using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Controls
{
    public sealed class PlainRadioButton : RadioButton
    {
        public PlainRadioButton() => new PlainButtonBase(this);
    }
}
