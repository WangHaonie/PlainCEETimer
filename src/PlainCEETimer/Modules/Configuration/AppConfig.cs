using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules.JsonConverters;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Configuration;

public class AppConfig
{
    public GeneralObject General { get; set; } = new();

    public DisplayObject Display { get; set; } = new();

    public int Exam { get; set; }

    public Exam[] Exams
    {
        get;
        set => ConfigValidator.SetValue(ref field, value, ConfigField.ExamInfoArray);
    } = [];

    public CountdownRule[] CustomRules
    {
        get;
        set => ConfigValidator.SetValue(ref field, value, ConfigField.CustomRulesArray);
    }

    [JsonConverter(typeof(GlobalRulesConverter))]
    public CountdownRule[] GlobalRules
    {
        get;
        set => ConfigValidator.SetValue(ref field, value, ConfigField.GlobalRulesArray);
    }

    [JsonProperty("GlobalColors")]
    public ColorPair[] DefaultColors
    {
        get;
        set
        {
            field = value;
            DefaultValues.InitEssentials(false);
        }
    }

    public HotKey[] HotKeys
    {
        get;
        set
        {
            if (ConfigValidator.ValidateNeeded)
            {
                HashSet<HotKey> hkset = new(ConfigValidator.HotKeyCount);

                for (int i = 0; i < ConfigValidator.HotKeyCount; i++)
                {
                    var hk = value[i];

                    if (hk.IsValid && !hkset.Add(hk))
                    {
                        throw ConfigValidator.InvalidTampering(ConfigField.HotKeysArray);
                    }
                }
            }

            field = value;
        }
    }

    public int[] CustomColors { get; set; }

    [JsonConverter(typeof(FontFormatConverter))]
    public Font Font { get; set; } = DefaultValues.CountdownDefaultFont;

    public int NtpServer
    {
        get;
        set => ConfigValidator.SetValue(ref field, value, 3, 0);
    }

    public int Dark
    {
        get;
        set => ConfigValidator.SetValue(ref field, value, 2, 0);
    }

    [JsonConverter(typeof(PointFormatConverter))]
    public Point Location { get; set; }

    public static readonly AppConfig Empty = new();

    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        var value = Exam;
        ConfigValidator.SetValue(ref value, value, Exams.Length, 0);
        Exam = value;

        Display.SeewoPptsvc = ConfigValidator.ValidateBoolean(Display.SeewoPptsvc, (General.TopMost && Display.Position == 0) || Display.Drag);
    }
}