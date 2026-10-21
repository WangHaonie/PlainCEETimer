using System.Drawing;
using System.Reflection;
using PlainCEETimer.Modules.Reflection;

namespace System.Windows.Forms;

internal class LabelInternals
{
    private Label_CreateStringFormat m_fnCreateStringFormat;
    private readonly Label m_target;
    private static MethodInfo s_miCreateStringFormat;

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
        if (s_miCreateStringFormat == null)
        {
            typeof(Label).RevealMember(out s_miCreateStringFormat);
        }

        if (m_fnCreateStringFormat == null)
        {
            ReflectionUtils.CreateDelegate(out m_fnCreateStringFormat, m_target, s_miCreateStringFormat);
        }

        return m_fnCreateStringFormat();
    }
}