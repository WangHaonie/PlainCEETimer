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

    [JsonIgnore]
    public bool Default { get; set; }

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

        if (Default || other.Default)
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

        if (Default)
        {
            return h.Combine();
        }

        return h.Add(Tick).Combine();
    }

    public object Clone()
    {
        return new CountdownRule
        {
            Phase = Phase,
            Tick = Tick,
            Text = Text,
            Colors = Colors,
            Default = Default
        };
    }

    bool IListViewData<CountdownRule>.Excluded { get; set; }

    bool IListViewData<CountdownRule>.InternalEquals(CountdownRule other)
    {
        return !Default && Equals(other) && Colors.Equals(other.Colors) && Text == other.Text;
    }

    private string DebuggerDisplay => $"{Phase}: {Tick.Format()}, {Text}";
}
