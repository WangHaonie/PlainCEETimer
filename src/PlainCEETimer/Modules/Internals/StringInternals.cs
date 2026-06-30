using System.Reflection;
using PlainCEETimer.Modules.Reflection;

namespace System;

internal static class StringInternals
{
    private static String_FastAllocateString s_fnFastAllocateString;

    internal static string FastAllocateString(int length)
    {
        s_fnFastAllocateString ??= DelegateHelper.StaticCreateDelegate<String_FastAllocateString>(typeof(string), BindingFlags.NonPublic);
        return s_fnFastAllocateString(length);
    }
}
