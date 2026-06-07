using System;
using System.Collections.Generic;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules;

public class RandomUID
{
    private readonly int min;
    private readonly int max;
    private readonly int total;
    private readonly HashSet<int> idset;
    private readonly Random ids;

    public RandomUID(int minValue, int maxValue)
    {
        if (minValue == maxValue)
        {
            throw new InvalidOperationException();
        }

        idset = [];
        ids = new();
        min = minValue;
        max = maxValue;
        int.SwapIf(min > max, ref min, ref max);
        total = max - min;
    }

    public int Next()
    {
        if (idset.Count == total)
        {
            throw new InvalidOperationException("榨干啦！");
        }

        var id = ids.Next(min, max);

        if (idset.Add(id))
        {
            return id;
        }

        return Next();
    }

    public void Remove(int id)
    {
        idset.Remove(id);
    }
}