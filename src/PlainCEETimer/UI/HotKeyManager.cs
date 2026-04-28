using System;
using System.Collections.Generic;
using PlainCEETimer.Modules.Fody;

namespace PlainCEETimer.UI;

[NoConstants]
internal static class HotKeyManager
{
    public const int HotKeyCount = 3;

    public static HotKey[] CheckHotKeys(HotKey[] value)
    {
        var arr = new HotKey[HotKeyCount];

        if (value != null)
        {
            int length = value.Length;

            if (length == HotKeyCount)
            {
                return value;
            }

            length = Math.Min(HotKeyCount, length);

            for (int i = 0; i < length; i++)
            {
                arr[i] = value[i];
            }
        }

        return arr;
    }

    public static bool HasDuplicates(HotKey[] value)
    {
        HashSet<HotKey> set = new(HotKeyCount);

        foreach (var hotKey in value)
        {
            if (hotKey.IsValid && !set.Add(hotKey))
            {
                return true;
            }
        }

        return false;
    }

    public static bool TryValidate(HotKey[] value, out int failed)
    {
        HashSet<HotKey> set = new(HotKeyCount);
        failed = -1;

        for (int i = 0; i < value.Length; i++)
        {
            var hotKey = value[i];

            if (!hotKey.IsValid)
            {
                continue;
            }

            if (!set.Add(hotKey) || HotKeyService.Test(hotKey) == HotKeyStatus.Failed)
            {
                failed = i;
                return false;
            }
        }

        return true;
    }

    public static string GetHotKeyDescription(int index) => index switch
    {
        0 => "隐藏主窗口",
        1 => "上一个考试",
        2 => "下一个考试",
        _ => null
    };
}
