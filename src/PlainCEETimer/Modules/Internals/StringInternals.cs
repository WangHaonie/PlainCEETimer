using System.Reflection;
using PlainCEETimer.Modules.Reflection;

namespace System;

internal static class StringInternals
{
    private static String_FastAllocateString s_fnFastAllocateString;

    /// <summary>
    /// 创建并返回一个空的，指定长度的 <see cref="string"/>；指定的 <paramref name="length"/> 通常不包含末尾空字符。
    /// </summary>
    internal static string FastAllocateString(int length)
    {
        ReflectionUtils.StaticCreateDelegate(ref s_fnFastAllocateString, typeof(string), BindingFlags.NonPublic);
        return s_fnFastAllocateString(length);
    }
}
