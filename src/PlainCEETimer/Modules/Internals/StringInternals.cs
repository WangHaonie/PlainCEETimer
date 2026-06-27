using PlainCEETimer.Modules.Reflection;

namespace System;

internal static class StringInternals
{
    private static String_FastAllocateString m_fnFastAllocateString;

    internal static string FastAllocateString(int length)
    {
        m_fnFastAllocateString ??= DelegateHelper.StaticCreateDelegate<String_FastAllocateString>(typeof(string));
        return m_fnFastAllocateString(length);
    }
}
