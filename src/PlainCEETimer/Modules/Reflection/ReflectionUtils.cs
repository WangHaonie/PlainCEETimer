using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PlainCEETimer.Modules.Reflection;

public static class ReflectionUtils
{
    public static Type RevealType(Type refType, Type pseudoType)
    {
        return refType.Assembly.GetType(pseudoType.FullName);
    }

    public static void CreateDelegate<TDelegate>(ref TDelegate fn, object instance, MethodInfo methodInfo)
        where TDelegate : Delegate
    {
        fn ??= (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), instance, methodInfo);
    }

    public static void StaticCreateDelegate<TDelegate>(ref TDelegate fn, Type type, BindingFlags methodMod, [CallerMemberName] string methodName = "")
        where TDelegate : Delegate
    {
        StaticCreateDelegate(ref fn, type, null, methodMod, methodName);
    }

    public static void StaticCreateDelegate<TDelegate>(ref TDelegate fn, Type type, Type returnType, BindingFlags methodMod, [CallerMemberName] string methodName = "")
        where TDelegate : Delegate
    {
        fn ??= StaticCreateDelegateCore<TDelegate>(type, returnType, methodMod, methodName);
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
