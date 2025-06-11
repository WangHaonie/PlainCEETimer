using System;
using System.Diagnostics;
using Newtonsoft.Json;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Modules.Configuration
{
    [DebuggerDisplay("{Phase,nq} {Tick,nq} {Text,nq}")]
    [JsonConverter(typeof(CustomRulesConverter))]
    public class CustomRuleObject : IListViewData<CustomRuleObject>
    {
        public CountdownPhase Phase { get; set; }

        public TimeSpan Tick { get; set; }

        public string Text { get; set; }

        public ColorSetObject Colors { get; set; }

        public int CompareTo(CustomRuleObject other)
        {
            if (other == null)
            {
                return 1;
            }

            int order = Phase.CompareTo(other.Phase);

            if (order != 0)
            {
                return order;
            }

            order = other.Tick.CompareTo(Tick);
            return Phase == CountdownPhase.P3 ? -order : order;
        }

        public bool Equals(CustomRuleObject other)
        {
            return other != null && Phase == other.Phase && Tick == other.Tick;
        }

        public override bool Equals(object obj)
        {
            return Equals((CustomRuleObject)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (37 * 31 + Phase.GetHashCode()) * 31 + Tick.GetHashCode();
            }
        }

        bool IListViewData<CustomRuleObject>.InternalEquals(CustomRuleObject other)
        {
            return Equals(other) && Text == other.Text && Colors.Equals(Colors);
        }
    }
}
