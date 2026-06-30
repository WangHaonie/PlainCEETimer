using System;

namespace PlainCEETimer.Modules.Reflection;

public static class ReflectionHelper
{
    public static Type RevealType(Type refType, Type pseudoType)
    {
        return refType.Assembly.GetType(pseudoType.FullName);
    }
}
