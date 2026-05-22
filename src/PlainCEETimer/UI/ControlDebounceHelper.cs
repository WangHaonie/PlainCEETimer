using System.Windows.Forms;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI;

public class ControlDebounceHelper(Control control)
{
    private AppForm parentForm;

    public bool ShouldDebounce => (parentForm ??= control.FindParentForm())?.IsLoaded == true;
}
