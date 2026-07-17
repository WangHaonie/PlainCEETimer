using System.Reflection;
using PlainCEETimer.Modules.Reflection;

namespace System.Windows.Forms;

internal static class Command
{
    private static Command_DispatchID s_fnDispatchID;

    public static bool DispatchID(int id)
    {
        ReflectionUtils.StaticCreateDelegate(ref s_fnDispatchID,
            ReflectionUtils.RevealType(typeof(Control), typeof(Command)), BindingFlags.Public);
        return s_fnDispatchID(id);
    }
}