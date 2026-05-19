using PlainCEETimer.Modules.Reflection;

namespace System.Windows.Forms;

internal static class Command
{
    private static Command_DispatchID fnDispatchID;

    public static bool DispatchID(int id)
    {
        fnDispatchID ??= DelegateHelper.StaticCreateDelegate<Command_DispatchID>(typeof(Control), typeof(Command));
        return fnDispatchID(id);
    }
}