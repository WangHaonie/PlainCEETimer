using System.Reflection;
using PlainCEETimer.Modules.Reflection;

namespace System.Windows.Input;

internal class RoutedCommandInternals
{
    internal InputGestureCollection _inputGestureCollection
    {
        get => (InputGestureCollection)m_fiInputGestureCollection.GetValue(m_target);
        set => m_fiInputGestureCollection.SetValue(m_target, value);
    }

    protected readonly RoutedCommand m_target;

    private static readonly FieldInfo m_fiInputGestureCollection;

    protected RoutedCommandInternals(RoutedCommand target)
    {
        m_target = target;
    }

    static RoutedCommandInternals()
    {
        typeof(RoutedCommand).RevealMember(out m_fiInputGestureCollection, nameof(_inputGestureCollection));
    }

    public static RoutedCommandInternals AttachTo(RoutedCommand target)
    {
        return new(target);
    }
}
