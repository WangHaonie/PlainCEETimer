using System;
using System.Collections.Generic;
using PlainCEETimer.Modules.Fody;
using PlainCEETimer.Modules.Linq;

namespace PlainCEETimer.Modules;

[NoConstants]
public class ArrayEqualityComparer<T>(IEqualityComparer<T> comparer) : IEqualityComparer<T[]>
{
    private const int SampleSize = 16;
    private static readonly Dictionary<int, int[]> samplingIndices = [];

    public bool Equals(T[] x, T[] y)
    {
        if (x == null || y == null)
        {
            return false;
        }

        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x.Length != y.Length)
        {
            return false;
        }

        var length = x.Length;

        if (length <= SampleSize)
        {
            for (int i = 0; i < length; i++)
            {
                if (!comparer.Equals(x[i], y[i]))
                {
                    return false;
                }
            }
        }
        else
        {
            var sample = EnsureIndices(length);

            for (int i = 0; i < SampleSize; i++)
            {
                var current = sample[i];

                if (!comparer.Equals(x[current], y[current]))
                {
                    return false;
                }
            }

            for (int i = 0; i < length; i++)
            {
                if (!sample.ArrayContains(i) && !comparer.Equals(x[i], y[i]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public int GetHashCode(T[] obj)
    {
        if (obj.IsNullOrEmpty())
        {
            return 0;
        }

        var hashCode = new HashCode();
        var length = obj.Length;

        if (length <= SampleSize)
        {
            for (int i = 0; i < length; i++)
            {
                hashCode.Add(comparer.GetHashCode(obj[i]));
            }
        }
        else
        {
            var sample = EnsureIndices(length);

            for (int i = 0; i < SampleSize; i++)
            {
                hashCode.Add(comparer.GetHashCode(obj[sample[i]]));
            }
        }

        return hashCode.Combine();
    }

    private int[] EnsureIndices(int total)
    {
        if (!samplingIndices.TryGetValue(total, out var result))
        {
            result = CreateSampleIndices(0, total, SampleSize);
            samplingIndices[total] = result;
        }

        return result;
    }

    private int[] CreateSampleIndices(int start, int end, int count)
    {
        var indices = new int[count];
        var mid = start + (end - start) / 2;
        var remaining = count - 3;
        var leftCount = remaining / 2;
        var rightCount = remaining - leftCount;

        indices[0] = start;

        var leftStep = (double)(mid - start) / (leftCount + 1);

        for (int i = 1; i <= leftCount; i++)
        {
            indices[i] = (int)Math.Round(start + i * leftStep);
        }

        indices[leftCount + 1] = mid;

        var rightStep = (double)(end - mid) / (rightCount + 1);

        for (int i = 1; i <= rightCount; i++)
        {
            indices[leftCount + 1 + i] = (int)Math.Round(mid + i * rightStep);
        }

        indices[count - 1] = end;
        return indices;
    }
}