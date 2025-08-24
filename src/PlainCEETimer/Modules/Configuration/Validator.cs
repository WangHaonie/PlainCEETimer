using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Configuration;

public static class Validator
{
    public const int MaxExamNameLength = 15;
    public const int MinExamNameLength = 2;
    public const int MaxFontSize = 36;
    public const int MinFontSize = 10;
    public const int MaxOpacity = 100;
    public const int MinOpacity = 20;
    public const int MaxCustomTextLength = 800;
    public const long MaxTick = 56623103990000000L; // 65535d 23h 59m 59s
    public const long MinTick = TimeSpan.TicksPerSecond; // 1s
    public const long MaxDate = 3155378975999999999L; // DateTime.Max
    public const long MinDate = 552877920000000000L; // 1753-01-01 00:00:00
    public const char ValueSeparator = ',';
    public const string ValueSeparatorString = ", ";
    public const string RegexPhPatterns = @"\{(\w+)\}";
    private static readonly JsonSerializerSettings Settings;

    internal static bool ValidateNeeded { get; set; } = true;

    static Validator()
    {
        Settings = new JsonSerializerSettings()
        {
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto
        };
    }

    public static void Save()
    {
        File.WriteAllText(App.ConfigFilePath, JsonConvert.SerializeObject(App.AppConfig, Settings));
    }

    public static AppConfig Read()
    {
        try
        {
            return JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(App.ConfigFilePath)) ?? AppConfig.Empty;
        }
        catch
        {
            return AppConfig.Empty;
        }
    }

    public static void SetValue(ref int field, int value, int max, int min, int defvalue = 0)
    {
        field = (ValidateNeeded && (value > max || value < min)) ? defvalue : value;
    }

    public static void SetValue<T>(ref T[] field, T[] value)
        where T : IListViewData<T>
    {
        if (ValidateNeeded)
        {
            HashSet<T> set = [];

            foreach (var item in value)
            {
                if (!set.Add(item))
                {
                    throw new Exception();
                }
            }

            Array.Sort(value);
        }

        field = value;
    }

    public static bool ValidateBoolean(bool value, bool condition)
    {
        return ValidateNeeded ? value && condition : value;
    }

    public static bool VerifyCustomText(string custom, out string warning, int index = 0)
    {
        var i = index != 0 ? $"第{index}个自定义文本" : "自定义文本";

        if (string.IsNullOrWhiteSpace(custom))
        {
            warning = $"{i}不能为空白！";
            return false;
        }

        var matches = Regex.Matches(custom, RegexPhPatterns);
        var count = matches.Count;
        var isEmpty = true;

        if (matches.Count != 0)
        {
            for (int j = 0; j < count; j++)
            {
                if (Constants.AllPHs.Contains(matches[j].Value))
                {
                    isEmpty = false;
                    break;
                }
            }
        }

        if (isEmpty)
        {
            warning = $"请在{i}中至少使用一个占位符！";
            return false;
        }

        warning = null;
        return true;
    }

    public static bool IsNiceContrast(Color fore, Color back)
    {
        //
        // 对比度判断 参考:
        //
        // Guidance on Applying WCAG 2 to Non-Web Information and ...
        // https://www.w3.org/TR/wcag2ict/#dfn-contrast-ratio
        //

        var L1 = GetRelativeLuminance(fore);
        var L2 = GetRelativeLuminance(back);

        if (L1 < L2)
        {
            (L1, L2) = (L2, L1);
        }

        return (L1 + 0.05) / (L2 + 0.05) >= 3;
    }

    public static bool IsValidExamLength(int length)
    {
        return length is >= MinExamNameLength and <= MaxExamNameLength;
    }

    public static bool IsInvalidCustomLength(int length)
    {
        return length is 0 or > MaxCustomTextLength;
    }

    public static void EnsureExamDate(DateTime time)
    {
        if (time.Ticks is < MinDate or > MaxDate)
        {
            throw new Exception();
        }
    }

    public static void EnsureCustomText(string custom)
    {
        if (IsInvalidCustomLength(custom.Length) || !VerifyCustomText(custom, out var _))
        {
            throw new Exception();
        }
    }

    public static Color GetColor(object obj)
    {
        int rgb;

        if (obj is int tmp)
        {
            rgb = tmp;
        }
        else
        {
            rgb = int.Parse(obj.ToString());
        }

        if (rgb > 0)
        {
            rgb = -rgb;
        }

        return Color.FromArgb(rgb);
    }

    private static double GetRelativeLuminance(Color color)
    {
        //
        // 亮度计算 参考:
        //
        // Guidance on Applying WCAG 2 to Non-Web Information and ...
        // https://www.w3.org/TR/wcag2ict/#dfn-relative-luminance
        //

        double RsRGB = color.R / 255.0;
        double GsRGB = color.G / 255.0;
        double BsRGB = color.B / 255.0;

        double R = RsRGB <= 0.03928 ? RsRGB / 12.92 : Math.Pow((RsRGB + 0.055) / 1.055, 2.4);
        double G = GsRGB <= 0.03928 ? GsRGB / 12.92 : Math.Pow((GsRGB + 0.055) / 1.055, 2.4);
        double B = BsRGB <= 0.03928 ? BsRGB / 12.92 : Math.Pow((BsRGB + 0.055) / 1.055, 2.4);

        return 0.2126 * R + 0.7152 * G + 0.0722 * B;
    }
}
