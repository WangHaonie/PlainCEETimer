using System.Collections.Generic;
using System.Reflection;

namespace System.Windows.Forms;

public static class ApplicationInternals
{
    internal sealed class ThreadContext
    {
        [ThreadStatic]
        internal static ThreadContext Instance;

        internal static object currentThreadContext
        {
            get => m_fiCurrentThreadContext.GetValue(null);
            set => m_fiCurrentThreadContext.SetValue(null, value);
        }

        internal IEnumerable<Control> parkingWindows
        {
            get => (IEnumerable<Control>)m_fiParkingWindows.GetValue(currentThreadContext);
            set => m_fiParkingWindows.SetValue(currentThreadContext, value);
        }

        private static readonly FieldInfo m_fiCurrentThreadContext;
        private static readonly FieldInfo m_fiParkingWindows;
        private static readonly Type m_type;

        static ThreadContext()
        {
            m_type = ApplicationInternals.m_type.GetNestedType(nameof(ThreadContext), BindingFlags.NonPublic);
            m_fiCurrentThreadContext = m_type.GetField(nameof(currentThreadContext), BindingFlags.NonPublic | BindingFlags.Static);
            m_fiParkingWindows = m_type.GetField(nameof(parkingWindows), BindingFlags.NonPublic | BindingFlags.Instance);
            Instance = new();
        }
    }

    private static readonly Type m_type;

    static ApplicationInternals()
    {
        m_type = typeof(Application);
    }
}
