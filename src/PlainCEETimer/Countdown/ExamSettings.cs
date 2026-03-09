using System;
using Newtonsoft.Json;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.JsonConverters;
using PlainCEETimer.Modules.Linq;

namespace PlainCEETimer.Countdown;

public class ExamSettings : IEquatable<ExamSettings>
{
    public bool Enabled { get; init; }

    public int Mode { get; init; }

    public CountdownFormat Format { get; init; }

    public CountdownRule[] Rules
    {
        get;
        init => ConfigValidator.SetValue(ref field, value, ConfigField.PerExamCustomRulesArray);
    }

    [JsonConverter(typeof(GlobalRulesConverter))]
    public CountdownRule[] DefRules
    {
        get;
        init => ConfigValidator.SetValue(ref field, value, ConfigField.PerExamGlobalRulesArray);
    }

    public bool Equals(ExamSettings other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Enabled == other.Enabled
            && Mode == other.Mode
            && Format == other.Format
            && DefRules.ArrayEquals(other.DefRules, CountdownRule.FullComparer)
            && Rules.ArrayEquals(other.Rules, CountdownRule.NormalComparer);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ExamSettings);
    }

    public override int GetHashCode()
    {
        return new HashCode()
            .Add(Enabled)
            .Add(Mode)
            .Add(Format)
            .Add(DefRules, CountdownRule.FullComparer)
            .Add(Rules, CountdownRule.NormalComparer)
            .Combine();
    }
}