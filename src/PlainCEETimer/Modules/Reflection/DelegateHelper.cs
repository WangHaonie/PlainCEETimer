using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules.Reflection;

internal static class DelegateHelper
{
    public static TDelegate StaticCreateDelegate<TDelegate>(Type referenceType, Type classType, BindingFlags flags = BindingFlags.Public | BindingFlags.Static, [CallerMemberName] string methodName = null)
        where TDelegate : Delegate
    {
        var type = referenceType.Assembly.GetType(classType.FullName);

        if (type != null)
        {
            var method = type.GetMethod(methodName, flags);

            if (method != null)
            {
                return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), method);
            }
        }

        return null;
    }
}