using System;
using System.Reflection;
using System.Windows.Controls;
using PlainCEETimer.Modules.Reflection;

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
        s_type = ReflectionUtils.RevealType(typeof(Control), typeof(FrameworkAppContextSwitches));
    }

    private static void EnsureAccess()
    {
        if (s_fiUseAdornerForTextboxSelectionRendering == null)
        {
            s_type.RevealMember(out s_fiUseAdornerForTextboxSelectionRendering, nameof(_useAdornerForTextboxSelectionRendering), isStatic: true);
        }
    }
}