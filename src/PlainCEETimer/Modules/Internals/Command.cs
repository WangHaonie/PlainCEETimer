using System;
using System.Reflection;
using System.Windows.Forms;

namespace PlainCEETimer.Modules.Internals;

internal static class Command
{
    private static FnDispatchID fnDispatchID;

    public static bool DispatchID(int id)
    {
        if (fnDispatchID == null)
        {
            var type = typeof(Control).Assembly.GetType("System.Windows.Forms.Command");

            if (type != null)
            {
                var target = type.GetMethod(nameof(DispatchID), BindingFlags.Public | BindingFlags.Static);

                if (target != null)
                {
                    fnDispatchID = (FnDispatchID)Delegate.CreateDelegate(typeof(FnDispatchID), target);
                }
            }
        }

        return fnDispatchID(id);
    }
}