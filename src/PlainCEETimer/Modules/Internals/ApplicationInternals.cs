using System.Collections.Generic;
using System.Reflection;
using PlainCEETimer.Modules.Reflection;

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
            s_type.GetNestedType(nameof(ThreadContext), BindingFlags.NonPublic)
                .RevealMember(out s_fiCurrentThreadContext, nameof(currentThreadContext), isStatic: true)
                .RevealMember(out s_fiParkingWindows, nameof(parkingWindows));

            Instance = new();
        }
    }

    private static readonly Type s_type;

    static ApplicationInternals()
    {
        s_type = typeof(Application);
    }
}
