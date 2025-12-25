using System.Collections.Generic;
using PlainCEETimer.Countdown;

namespace PlainCEETimer.Modules;

public class CountdownRuleComparer : IEqualityComparer<CountdownRule>
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