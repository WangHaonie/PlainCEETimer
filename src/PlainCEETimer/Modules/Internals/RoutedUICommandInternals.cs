using System.Reflection;
using PlainCEETimer.Modules.Reflection;

namespace System.Windows.Input;

internal class RoutedUICommandInternals : RoutedCommandInternals
{
    internal string _text
    {
        get => (string)m_fiText.GetValue(m_target);
        set => m_fiText.SetValue(m_target, value);
    }

    private static readonly FieldInfo m_fiText;

    private RoutedUICommandInternals(RoutedUICommand target) : base(target)
    {
        return;
    }

    static RoutedUICommandInternals()
    {
        typeof(RoutedUICommand).RevealMember(out m_fiText, nameof(_text));
    }

    public static RoutedUICommandInternals AttachTo(RoutedUICommand target)
    {
        return new(target);
    }
}
