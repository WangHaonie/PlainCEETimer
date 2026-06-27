using System.Collections.Generic;
using System.Reflection;

namespace System.Windows.Forms;

internal static class ApplicationInternals
{
    internal sealed class ThreadContext
    {
        [ThreadStatic]
        internal static ThreadContext Instance;

        internal static object currentThreadContext
        {
            get => s_fiCurrentThreadContext.GetValue(null);
            set => s_fiCurrentThreadContext.SetValue(null, value);
        }

        internal IEnumerable<Control> parkingWindows
        {
            get => (IEnumerable<Control>)s_fiParkingWindows.GetValue(currentThreadContext);
            set => s_fiParkingWindows.SetValue(currentThreadContext, value);
        }

        private static readonly FieldInfo s_fiCurrentThreadContext;
        private static readonly FieldInfo s_fiParkingWindows;

        static ThreadContext()
        {
            var type = s_type.GetNestedType(nameof(ThreadContext), BindingFlags.NonPublic);
            s_fiCurrentThreadContext = type.GetField(nameof(currentThreadContext), BindingFlags.NonPublic | BindingFlags.Static);
            s_fiParkingWindows = type.GetField(nameof(parkingWindows), BindingFlags.NonPublic | BindingFlags.Instance);
            Instance = new();
        }
    }

    private static readonly Type s_type;

    static ApplicationInternals()
    {
        s_type = typeof(Application);
    }
}
