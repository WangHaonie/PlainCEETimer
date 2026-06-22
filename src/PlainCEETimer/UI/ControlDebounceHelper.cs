using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI;

public class ControlDebounceHelper(Control control) : IDebounceState
{
    private AppForm parentForm;

    public bool ShouldDebounce
    {
        get
        {
            if (parentForm == null || parentForm.IsDisposed)
            {
                parentForm = control.FindParentForm();
            }

            if (parentForm != null)
            {
                return parentForm.Loaded;
            }

            return false;
        }
    }
}
