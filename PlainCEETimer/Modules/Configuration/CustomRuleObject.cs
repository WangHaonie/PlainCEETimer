using Newtonsoft.Json;
using PlainCEETimer.Modules.JsonConverters;
using System;
using System.Drawing;

namespace PlainCEETimer.Modules.Configuration
{
    [JsonConverter(typeof(CustomRulesConverter))]
    public sealed class CustomRuleObject : IComparable<CustomRuleObject>, IEquatable<CustomRuleObject>
    {
        public CountdownPhase Phase { get; set; }

        public TimeSpan Tick { get; set; }

        public Color Fore { get; set; }

        public Color Back { get; set; }

        public string Text { get; set; }

        int IComparable<CustomRuleObject>.CompareTo(CustomRuleObject other)
        {
            if (other == null)
            {
                return 1;
            }

            var PhaseComparer = Phase.CompareTo(other.Phase);

            if (PhaseComparer != 0)
            {
                return PhaseComparer;
            }

            return Tick.CompareTo(other.Tick);
        }

        bool IEquatable<CustomRuleObject>.Equals(CustomRuleObject other)
        {
            if (other == null)
            {
                return false;
            }

            return Phase == other.Phase && Tick == other.Tick;
        }
    }
}
