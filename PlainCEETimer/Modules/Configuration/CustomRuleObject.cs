using System;
using Newtonsoft.Json;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Modules.Configuration
{
    [JsonConverter(typeof(CustomRulesConverter))]
    public class CustomRuleObject : IListViewObject<CustomRuleObject>
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

            int PhaseOrder = Phase.CompareTo(other.Phase);

            if (PhaseOrder != 0)
            {
                return PhaseOrder;
            }

            int TickOrder = other.Tick.CompareTo(Tick);

            return Phase == CountdownPhase.P3 ? -TickOrder : TickOrder;
        }

        public bool Equals(CustomRuleObject other)
        {
            if (other == null)
            {
                return false;
            }

            return Phase == other.Phase && Tick == other.Tick;
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
    }
}
