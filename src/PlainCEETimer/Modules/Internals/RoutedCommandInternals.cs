using System.Reflection;

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
        m_fiInputGestureCollection = typeof(RoutedCommand).GetField(nameof(_inputGestureCollection), BindingFlags.Instance | BindingFlags.NonPublic);
    }

    public static RoutedCommandInternals AttachTo(RoutedCommand target)
    {
        return new(target);
    }
}
