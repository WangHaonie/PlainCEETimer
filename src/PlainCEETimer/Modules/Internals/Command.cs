using System.Reflection;
using PlainCEETimer.Modules.Reflection;

namespace System.Windows.Forms;

internal static class Command
{
    private static Command_DispatchID s_fnDispatchID;

    public static bool DispatchID(int id)
    {
        DelegateHelper.StaticCreateDelegate(ref s_fnDispatchID,
            ReflectionHelper.RevealType(typeof(Control), typeof(Command)), BindingFlags.Public);
        return s_fnDispatchID(id);
    }
}