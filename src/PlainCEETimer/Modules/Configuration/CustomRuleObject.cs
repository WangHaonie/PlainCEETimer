using System;
using System.Diagnostics;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.JsonConverters;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Configuration
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [JsonConverter(typeof(CustomRuleConverter))]
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
            return unchecked((37 * 31 + Phase.GetHashCode()) * 31 + Tick.GetHashCode());
        }

        bool IListViewData<CustomRuleObject>.InternalEquals(CustomRuleObject other)
        {
            return Equals(other) && Colors.Equals(other.Colors) && Text == other.Text;
        }

        private string DebuggerDisplay => $"{Phase}: {Tick.Format()}, {Text}";
    }
}
