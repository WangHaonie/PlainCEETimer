using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PlainCEETimer.Modules.Reflection;

internal static class DelegateHelper
{
    public static TDelegate StaticCreateDelegate<TDelegate>(Type referenceType, Type classType, BindingFlags flags = BindingFlags.Public | BindingFlags.Static, [CallerMemberName] string methodName = null)
        where TDelegate : Delegate
    {
        return StaticCreateDelegate<TDelegate>(referenceType, classType, null, flags, methodName);
    }

    public static TDelegate StaticCreateDelegate<TDelegate>(Type referenceType, Type classType, Type returnType, BindingFlags flags = BindingFlags.Public | BindingFlags.Static, [CallerMemberName] string methodName = null)
        where TDelegate : Delegate
    {
        var type = referenceType.Assembly.GetType(classType.FullName);

        if (type != null)
        {
            var methods = type.GetMethods(flags).Where(m => m.Name == methodName);

            MethodInfo method = returnType == null
                ? methods.FirstOrDefault()
                : methods.FirstOrDefault(x => x.ReturnType == returnType);

            if (method != null)
            {
                return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), method);
            }
        }

        return null;
    }
}