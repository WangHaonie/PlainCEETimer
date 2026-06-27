using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PlainCEETimer.Modules.Reflection;

internal static class DelegateHelper
{
    public static TDelegate CreateDelegate<TDelegate>(object instance, Type type, BindingFlags flags = BindingFlags.NonPublic, [CallerMemberName] string methodName = "")
        where TDelegate : Delegate
    {
        return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), instance, type.GetMethod(methodName, flags | BindingFlags.Instance));
    }

    public static TDelegate StaticCreateDelegate<TDelegate>(Type type, BindingFlags flags = BindingFlags.NonPublic, [CallerMemberName] string methodName = "")
        where TDelegate : Delegate
    {
        return StaticCreateDelegateCore<TDelegate>(type, null, flags, methodName);
    }

    public static TDelegate StaticCreateDelegate<TDelegate>(Type referenceType, Type pseudoType, BindingFlags flags = BindingFlags.Public, [CallerMemberName] string methodName = "")
        where TDelegate : Delegate
    {
        return StaticCreateDelegateCore<TDelegate>(referenceType.Assembly.GetType(pseudoType.FullName), null, flags, methodName);
    }

    public static TDelegate StaticCreateDelegateCore<TDelegate>(Type type, Type returnType, BindingFlags flags = BindingFlags.Public, [CallerMemberName] string methodName = "")
        where TDelegate : Delegate
    {
        if (type != null)
        {
            MethodInfo method;
            flags |= BindingFlags.Static;

            if (returnType == null)
            {
                method = type.GetMethod(methodName, flags);
            }
            else
            {
                method = GetMethodByReturnType(type, returnType, flags, methodName);
            }

            if (method != null)
            {
                return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), method);
            }
        }

        return null;
    }

    private static MethodInfo GetMethodByReturnType(Type type, Type returnType, BindingFlags flags, string methodName)
    {
        return type.GetMethods(flags)
            .Where(m => m.Name == methodName)
            .FirstOrDefault(x => x.ReturnType == returnType);
    }
}