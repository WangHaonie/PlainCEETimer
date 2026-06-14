using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Modules.Reflection;

namespace PlainCEETimer.Modules.Internals;

internal class LabelInternals
{
    private readonly Label m_target;
    private Label_CreateStringFormat m_fnCreateStringFormat;

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
        m_fnCreateStringFormat ??= DelegateHelper.CreateDelegate<Label_CreateStringFormat>(m_target, typeof(Label));
        return m_fnCreateStringFormat();
    }
}