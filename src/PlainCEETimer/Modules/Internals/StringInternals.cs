using System.Linq;
using System.Reflection;

namespace System;

internal static class StringInternals
{
    private static String_FastAllocateString m_fnFastAllocateString;

    internal static string FastAllocateString(int length)
    {
        m_fnFastAllocateString ??= (String_FastAllocateString)Delegate.CreateDelegate(
            typeof(String_FastAllocateString),
            typeof(string).GetRuntimeMethods().FirstOrDefault(m => m.Name == nameof(FastAllocateString))
        );

        return m_fnFastAllocateString(length);
    }
}
