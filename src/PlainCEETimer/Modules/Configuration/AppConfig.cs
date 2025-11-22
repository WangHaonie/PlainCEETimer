using System.Drawing;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PlainCEETimer.Countdown;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Modules.Configuration;

public class AppConfig
{
    public GeneralObject General { get; set; } = new();

    public DisplayObject Display { get; set; } = new();

    public int Exam { get; set; }

    public Exam[] Exams
    {
        get;
        set => Validator.SetValue(ref field, value, ConfigField.ExamInfoArray);
    } = [];

    public CustomRule[] CustomRules
    {
        get;
        set => Validator.SetValue(ref field, value, ConfigField.CustomRulesArray);
    } = [];

    public CustomRule[] GlobalRules
    {
        get;
        set => Validator.SetValue(ref field, value, ConfigField.GlobalRulesArray);
    }

    public int[] CustomColors { get; set; }

    [JsonConverter(typeof(FontFormatConverter))]
    public Font Font { get; set; } = DefaultValues.CountdownDefaultFont;

    public int NtpServer
    {
        get;
        set => Validator.SetValue(ref field, value, 3, 0);
    }

    public int Dark
    {
        get;
        set => Validator.SetValue(ref field, value, 2, 0);
    }

    [JsonConverter(typeof(PointFormatConverter))]
    public Point Location { get; set; }

    public static readonly AppConfig Empty = new();

    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        var arr = GlobalRules;

        if (arr.IsNullOrEmpty() || arr.Length < 3)
        {
            GlobalRules = null;
        }

        var value = Exam;
        Validator.SetValue(ref value, value, Exams.Length, 0);
        Exam = value;
        Display.SeewoPptsvc = Validator.ValidateBoolean(Display.SeewoPptsvc, (General.TopMost && Display.X == 0) || Display.Draggable);
    }
}