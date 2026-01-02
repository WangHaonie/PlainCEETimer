using Newtonsoft.Json;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Countdown;

public class ExamSettings
{
    public bool Enabled { get; set; }

    public int Mode { get; set; }

    public CountdownFormat Format { get; set; }

    public CountdownRule[] Rules
    {
        get;
        set => ConfigValidator.SetValue(ref field, value, ConfigField.PerExamCustomRulesArray);
    }

    [JsonConverter(typeof(GlobalRulesConverter))]
    public CountdownRule[] DefRules
    {
        get;
        set => ConfigValidator.SetValue(ref field, value, ConfigField.PerExamGlobalRulesArray);
    }
}