using System;
using System.Diagnostics;
using Newtonsoft.Json;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.JsonConverters;
using PlainCEETimer.UI;

namespace PlainCEETimer.Countdown;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
[JsonConverter(typeof(CountdownRuleConverter))]
public class CountdownRule : IListViewData<CountdownRule>
{
    public CountdownPhase Phase { get; set; }

    public TimeSpan Tick { get; set; }

    public string Text { get; set; }

    public ColorPair Colors { get; set; }

    internal bool IsDefault { get; set; }

    public int CompareTo(CountdownRule other)
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

    public bool Equals(CountdownRule other)
    {
        if (other == null)
        {
            return false;
        }

        var flag = Phase == other.Phase;

        if (IsDefault || other.IsDefault)
        {
            return flag;
        }

        return flag && Tick == other.Tick;
    }

    public override bool Equals(object obj)
    {
        if (obj is CountdownRule r)
        {
            return Equals(r);
        }

        return false;
    }

    public override int GetHashCode()
    {
        var h = new HashCode().Add(Phase);

        if (IsDefault)
        {
            return h.Combine();
        }

        return h.Add(Tick).Combine();
    }

    bool IListViewData<CountdownRule>.InternalEquals(CountdownRule other)
    {
        return !IsDefault && Equals(other) && Colors.Equals(other.Colors) && Text == other.Text;
    }

    private string DebuggerDisplay => $"{Phase}: {Tick.Format()}, {Text}";
}
