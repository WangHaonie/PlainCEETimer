using System.Reflection;

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
        s_piWindowText = typeof(Control).GetProperty(nameof(WindowText), BindingFlags.Instance | BindingFlags.NonPublic);
    }

    public static ControlInternals AttachTo(Control control)
    {
        return new(control);
    }
}
