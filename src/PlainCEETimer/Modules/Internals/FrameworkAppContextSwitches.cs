using System;
using System.Reflection;
using System.Windows.Controls;

namespace MS.Internal;

internal static class FrameworkAppContextSwitches
{
    public static int _useAdornerForTextboxSelectionRendering
    {
        get
        {
            EnsureAccess();
            return (int)s_fiUseAdornerForTextboxSelectionRendering.GetValue(null);
        }
        set
        {
            EnsureAccess();
            s_fiUseAdornerForTextboxSelectionRendering.SetValue(null, value);
        }
    }

    private static FieldInfo s_fiUseAdornerForTextboxSelectionRendering;
    private static readonly Type s_type;

    static FrameworkAppContextSwitches()
    {
        s_type = typeof(Control).Assembly.GetType(typeof(FrameworkAppContextSwitches).FullName);
    }

    private static void EnsureAccess()
    {
        s_fiUseAdornerForTextboxSelectionRendering ??= s_type.GetField(nameof(_useAdornerForTextboxSelectionRendering), BindingFlags.NonPublic | BindingFlags.Static);
    }
}