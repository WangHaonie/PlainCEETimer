using System;
using System.Collections.Generic;
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
    internal static readonly CountdownRuleFullComparer FullComparer = new();

    internal static readonly CountdownRulePhaseOnlyComparer PhaseOnlyComparer = new();

    internal static readonly CountdownRuleNormalComparer NormalComparer = new();

    internal class CountdownRuleFullComparer : IEqualityComparer<CountdownRule>
    {
        public bool Equals(CountdownRule x, CountdownRule y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            if (ReferenceEquals(x, y))
            {
                return true;
            }

            return x.Phase == y.Phase
                && x.Tick == y.Tick
                && x.Colors.Equals(y.Colors)
                && x.Text == y.Text;
        }

        public int GetHashCode(CountdownRule obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return new HashCode()
                .Add(obj.Phase)
                .Add(obj.Tick)
                .Add(obj.Colors)
                .Add(obj.Text)
                .Combine();
        }
    }

    internal class CountdownRulePhaseOnlyComparer : IEqualityComparer<CountdownRule>
    {
        public bool Equals(CountdownRule x, CountdownRule y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            if (ReferenceEquals(x, y))
            {
                return true;
            }

            return x.Phase == y.Phase;
        }

        public int GetHashCode(CountdownRule obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return obj.Phase.GetHashCode();
        }
    }

    internal class CountdownRuleNormalComparer : IEqualityComparer<CountdownRule>
    {
        public bool Equals(CountdownRule x, CountdownRule y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            if (ReferenceEquals(x, y))
            {
                return true;
            }

            return x.Phase == y.Phase
                && x.Tick == y.Tick;
        }

        public int GetHashCode(CountdownRule obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return new HashCode()
                .Add(obj.Phase)
                .Add(obj.Tick)
                .Combine();
        }
    }

    public CountdownPhase Phase { get; init; }

    public TimeSpan Tick { get; init; }

    public string Text { get; init; }

    public ColorPair Colors { get; init; }

    [JsonIgnore]
    public bool Default { get; init; }

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

        if (Default || other.Default)
        {
            return PhaseOnlyComparer.Equals(this, other);
        }

        return NormalComparer.Equals(this, other);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as CountdownRule);
    }

    public override int GetHashCode()
    {
        if (Default)
        {
            return PhaseOnlyComparer.GetHashCode(this);
        }

        return NormalComparer.GetHashCode(this);
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
        return !Default && FullComparer.Equals(this, other);
    }

    private string DebuggerDisplay => $"{Phase}: {Tick.Format()}, {Text}";
}
