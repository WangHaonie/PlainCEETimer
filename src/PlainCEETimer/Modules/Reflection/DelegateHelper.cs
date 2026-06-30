using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PlainCEETimer.Modules.Reflection;

internal static class DelegateHelper
{
    public static TDelegate CreateDelegate<TDelegate>(object instance, MethodInfo methodInfo)
        where TDelegate : Delegate
    {
        return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), instance, methodInfo);
    }

    public static TDelegate StaticCreateDelegate<TDelegate>(Type type, BindingFlags methodMod, [CallerMemberName] string methodName = "")
        where TDelegate : Delegate
    {
        return StaticCreateDelegate<TDelegate>(type, null, methodMod, methodName);
    }

    public static TDelegate StaticCreateDelegate<TDelegate>(Type type, Type returnType, BindingFlags methodMod, [CallerMemberName] string methodName = "")
        where TDelegate : Delegate
    {
        return StaticCreateDelegateCore<TDelegate>(type, returnType, methodMod, methodName);
    }

    private static TDelegate StaticCreateDelegateCore<TDelegate>(Type type, Type returnType, BindingFlags methodMod, string methodName)
        where TDelegate : Delegate
    {
        if (type != null)
        {
            methodMod |= BindingFlags.Static;

            var method = returnType == null
                ? type.GetMethod(methodName, methodMod)
                : GetMethodByReturnType(type, returnType, methodMod, methodName);

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