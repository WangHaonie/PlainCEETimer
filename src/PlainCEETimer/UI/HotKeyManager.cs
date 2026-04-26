using System;
using System.Collections.Generic;
using PlainCEETimer.Modules.Configuration;

namespace PlainCEETimer.UI;

internal static class HotKeyManager
{
    public static HotKey[] CheckHotKeys(HotKey[] value)
    {
        var arr = new HotKey[ConfigValidator.HotKeyCount];

        if (value != null)
        {
            int length = value.Length;

            if (length == ConfigValidator.HotKeyCount)
            {
                return value;
            }

            length = Math.Min(ConfigValidator.HotKeyCount, length);

            for (int i = 0; i < length; i++)
            {
                arr[i] = value[i];
            }
        }

        return arr;
    }

    public static bool HasDuplicates(HotKey[] value)
    {
        HashSet<HotKey> set = new(ConfigValidator.HotKeyCount);

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
        HashSet<HotKey> set = new(ConfigValidator.HotKeyCount);
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
}
