using System;
using System.Diagnostics;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.JsonConverters;
using PlainCEETimer.UI;

namespace PlainCEETimer.Countdown;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
[JsonConverter(typeof(CustomRuleConverter))]
public class CustomRule : IListViewData<CustomRule>
{
    public CountdownPhase Phase { get; set; }

    public TimeSpan Tick { get; set; }

    public string Text { get; set; }

    public ColorPair Colors { get; set; }

    internal bool IsDefault { get; set; }

    public int CompareTo(CustomRule other)
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

    public bool Equals(CustomRule other)
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
        return Equals((CustomRule)obj);
    }

    public override int GetHashCode()
    {
        var phasecode = unchecked(37 * 31 + Phase.GetHashCode() * 31);

        if (IsDefault)
        {
            return phasecode;
        }

        return unchecked(phasecode + Tick.GetHashCode());
    }

    bool IListViewData<CustomRule>.InternalEquals(CustomRule other)
    {
        return !IsDefault && Equals(other) && Colors.Equals(other.Colors) && Text == other.Text;
    }

    private string DebuggerDisplay => $"{Phase}: {Tick.Format()}, {Text}";
}
