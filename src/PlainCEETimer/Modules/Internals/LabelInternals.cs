using System.Drawing;
using System.Reflection;
using PlainCEETimer.Modules.Reflection;

namespace System.Windows.Forms;

internal class LabelInternals
{
    private Label_CreateStringFormat m_fnCreateStringFormat;
    private static MethodInfo s_miCreateStringFormat;
    private readonly Label m_target;

    private LabelInternals(Label target)
    {
        m_target = target;
    }

    public static LabelInternals AttachTo(Label target)
    {
        return new(target);
    }

    internal StringFormat CreateStringFormat()
    {
        s_miCreateStringFormat ??= typeof(Label).GetMethod(nameof(CreateStringFormat), BindingFlags.Instance | BindingFlags.NonPublic);
        m_fnCreateStringFormat ??= DelegateHelper.CreateDelegate<Label_CreateStringFormat>(m_target, s_miCreateStringFormat);
        return m_fnCreateStringFormat();
    }
}