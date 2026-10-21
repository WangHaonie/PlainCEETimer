using System.Reflection;
using PlainCEETimer.Modules.Reflection;

namespace System.Windows.Forms;

internal class ControlInternals
{
    internal string WindowText
    {
        get => (string)s_piWindowText.GetValue(m_target);
        set => s_piWindowText.SetValue(m_target, value);
    }

    private readonly Control m_target;
    private static readonly PropertyInfo s_piWindowText;

    private ControlInternals(Control target)
    {
        m_target = target;
    }

    static ControlInternals()
    {
        typeof(Control).RevealMember(out s_piWindowText, nameof(WindowText));
    }

    public static ControlInternals AttachTo(Control control)
    {
        return new(control);
    }
}
