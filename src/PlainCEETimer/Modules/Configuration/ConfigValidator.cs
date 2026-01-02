using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules.Linq;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Configuration;

internal static class ConfigValidator
{
    internal sealed class InternalAccessScope : IDisposable
    {
        private readonly bool oldValue;

        public InternalAccessScope()
        {
            oldValue = validateNeeded;
            validateNeeded = false;
        }

        void IDisposable.Dispose()
        {
            validateNeeded = oldValue;
        }
    }

    public const int MaxExamNameLength = 15;
    public const int MinExamNameLength = 2;
    public const int MaxFontSize = 36;
    public const int MinFontSize = 10;
    public const float MinFontSizeError = 0.25F;
    public const int MaxOpacity = 100;
    public const int MinOpacity = 20;
    public const int MinCpp = MinFontSize;
    public const int DefCpp = 30;
    public const int MaxCpp = 300;
    public const int MaxCustomTextLength = 800;
    public const int HotKeyCount = 3;
    public const int DefaultCountdownRuleFlag = 1469529003; // hashcode of "spr_flag"
    public const long MaxTick = 56623103990000000L; // 65535d 23h 59m 59s
    public const long MinTick = TimeSpan.TicksPerSecond; // 1s
    public const long MaxDate = 3155378975999999999L; // DateTime.Max
    public const long MinDate = 552877920000000000L; // 1753-01-01 00:00:00
    public const long MinDateSeconds = MinDate / MinTick;
    public const char ValueSeparator = ',';
    public const string ValueSeparatorString = ", ";
    public const string RegexPhPatterns = @"\{(x|d|dd|cd|h|th|dh|m|tm|s|ts|ht)\}";
    public const string DateTimeFormat = "yyyy'/'M'/'d ddd H':'mm':'ss";
    public const string DTPFormat = "yyyy'/'MM'/'dd dddd HH':'mm':'ss";

    public static bool ValidateNeeded
    {
        get => validateNeeded;
        set
        {
            validateNeeded = value;
        }
    }

    private static bool canSaveConfig;
    private static bool validateNeeded = true;

    private static readonly JsonSerializerSettings Settings = new()
    {
        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
        NullValueHandling = NullValueHandling.Ignore,
        TypeNameHandling = TypeNameHandling.Auto,
#if DEBUG
        Formatting = Formatting.Indented
#endif
    };

    static ConfigValidator()
    {
        App.AppExit += SaveConfig;
    }

    public static void DemandConfig()
    {
        canSaveConfig = true;
    }

    public static void SaveConfig()
    {
        try
        {
            if (canSaveConfig)
            {
                File.WriteAllText(App.ConfigFilePath, JsonConvert.SerializeObject(App.AppConfig, Settings));
                canSaveConfig = false;
            }
        }
        catch { }
    }

    public static AppConfig ReadConfig()
    {
        try
        {
            var config = App.ConfigFilePath;

            if (File.Exists(config))
            {
                return JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(config)) ?? AppConfig.Empty;
            }

            return AppConfig.Empty;
        }
        catch (Exception ex)
        {
            App.PopupAbortRetryIgnore($"无法加载配置文件，详细信息已写入{App.WriteException(ex)}\n\n" + ex.Message, App.AppName);
            return AppConfig.Empty;
        }
    }

    public static void SetValue(ref int field, int value, int max, int min, int defvalue = 0)
    {
        field = (ValidateNeeded && (value > max || value < min)) ? defvalue : value;
    }

    public static void SetValue<T>(ref T[] field, T[] value, ConfigField type)
        where T : IListViewData<T>
    {
        if (value != null)
        {
            if (ValidateNeeded)
            {
                HashSet<T> set = [];

                foreach (var item in value)
                {
                    if (!set.Add(item))
                    {
                        throw InvalidTampering(type);
                    }
                }
            }

            value.ArrayOrder();
            field = value;
        }
    }

    public static bool ValidateBoolean(bool value, bool condition)
    {
        return ValidateNeeded ? value && condition : value;
    }

    public static bool VerifyCustomText(string custom)
    {
        if (string.IsNullOrWhiteSpace(custom))
        {
            return false;
        }

        var matches = Regex.Matches(custom, RegexPhPatterns);
        var count = matches.Count;
        var isValid = false;

        if (matches.Count != 0)
        {
            for (int j = 0; j < count; j++)
            {
                if (GetPhIndex(matches[j].Value) >= 0)
                {
                    isValid = true;
                    break;
                }
            }
        }

        return isValid;
    }

    public static bool IsValidExamLength(int length)
    {
        return length is >= MinExamNameLength and <= MaxExamNameLength;
    }

    public static bool IsInvalidCustomLength(int length)
    {
        return length is 0 or > MaxCustomTextLength;
    }

    public static int GetPhIndex(string ph) => ph switch
    {
        Ph.ExamName => 0,
        Ph.Days => 1,
        Ph.CeilingDays => 2,
        Ph.DecimalDays => 3,
        Ph.Hours => 4,
        Ph.TotalHours => 5,
        Ph.DecimalHours => 6,
        Ph.Minutes => 7,
        Ph.TotalMinutes => 8,
        Ph.Seconds => 9,
        Ph.TotalSeconds => 10,
        "{ht}" => 11,
        _ => -1
    };

    public static string GetHokKeyDescription(int index) => index switch
    {
        0 => "隐藏主窗口",
        1 => "上一个考试",
        2 => "下一个考试",
        _ => null
    };

    public static void EnsureExamDate(DateTime time)
    {
        if (time.Ticks is < MinDate or > MaxDate)
        {
            throw InvalidTampering(ConfigField.DateTimeLength);
        }
    }

    public static void EnsureCustomText(string custom)
    {
        if (IsInvalidCustomLength(custom.Length))
        {
            throw InvalidTampering(ConfigField.CustomTextLength);
        }

        if (!VerifyCustomText(custom))
        {
            throw InvalidTampering(ConfigField.CustomTextFormat);
        }
    }

    public static ColorPair ParseColorPairFromConfig(int fore, int back)
    {
        var f = GetColorFromInt32(fore);
        var b = GetColorFromInt32(back);

        var colors = new ColorPair(f, b);

        if (colors.Readable)
        {
            return colors;
        }

        throw InvalidTampering(ConfigField.ColorSetContrast);
    }

    private static Color GetColorFromInt32(int c)
    {
        var rgb = c;

        if (rgb > 0)
        {
            rgb = -rgb;
        }

        return Color.FromArgb(rgb);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static InvalidTamperingException InvalidTampering(ConfigField config)
    {
        return new InvalidTamperingException(config);
    }
}
