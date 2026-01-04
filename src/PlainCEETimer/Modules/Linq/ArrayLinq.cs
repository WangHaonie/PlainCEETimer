using System;
using System.Collections.Generic;
using PlainCEETimer.Modules.Extensions;

namespace PlainCEETimer.Modules.Linq;

/*

此类包含的扩展方法旨在尽可能不使用 System.Linq 而使用 System.Array 的情况下来对数组进行简单 LINQ。

    * 优点：
        · 更接近原生，跟 System.Linq 相比略快;
        · 部分方法会直接在原数组上操作而不是返回新数组 (谨慎使用);

    * 缺点:
        · 没有延迟执行 (Deferred Execution)，过长的链式调用反倒不如 System.Linq;
        · 其他未知的缺点;

*/
public static class ArrayLinq
{
    public static T[] ArrayWhere<T>(this T[] array, Predicate<T> match)
    {
        return Array.FindAll(array, match);
    }

    public static TResult[] ArraySelect<TInput, TResult>(this TInput[] array, Converter<TInput, TResult> converter)
    {
        return Array.ConvertAll(array, converter);
    }

    public static TResult[] ArrayWhereSelect<TInput, TResult>(this TInput[] array, Func<TInput, bool> match, Func<TInput, TResult> converter)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (match == null)
        {
            throw new ArgumentNullException(nameof(match));
        }

        if (converter == null)
        {
            throw new ArgumentNullException(nameof(converter));
        }

        var list = new List<TResult>();

        foreach (var obj in array)
        {
            if (match(obj))
            {
                list.Add(converter(obj));
            }
        }

        return [.. list];
    }

    public static T[] ArrayOrder<T>(this T[] array)
    {
        Array.Sort(array);
        return array;
    }

    public static T[] ArrayOrderDescending<T>(this T[] array, bool isSorted = false)
    {
        if (!isSorted)
        {
            Array.Sort(array);
        }

        Array.Reverse(array);
        return array;
    }

    public static T[] ArrayTake<T>(this T[] array, int count)
    {
        if (count >= array.Length)
        {
            return array.Copy();
        }

        if (count <= 0)
        {
            return [];
        }

        var arr = new T[count];
        Array.Copy(array, 0, arr, 0, count);
        return arr;
    }

    public static T[] ArraySkip<T>(this T[] array, int count)
    {
        var length = array.Length;

        if (count >= length)
        {
            return [];
        }

        if (count <= 0)
        {
            return array.Copy();
        }

        length -= count;
        var arr = new T[length];
        Array.Copy(array, count, arr, 0, length);
        return arr;
    }

    public static T[] PopulateWith<T>(this T[] destination, T[] source)
    {
        if (source != null)
        {
            Array.Copy(source, destination, Math.Min(source.Length, destination.Length));
        }

        return destination;
    }

    public static bool IsNullOrEmpty<T>(this T[] array)
    {
        return array == null || array.Length == 0;
    }

    public static bool ArrayEquals<T>(this T[] array1, T[] array2, IEqualityComparer<T> comparer = null)
    {
        if (array1 == null || array2 == null || array1.Length != array2.Length)
        {
            return false;
        }

        if (ReferenceEquals(array1, array2))
        {
            return true;
        }

        comparer ??= EqualityComparer<T>.Default;

        for (int i = 0; i < array1.Length; i++)
        {
            if (!comparer.Equals(array1[i], array2[i]))
            {
                return false;
            }
        }

        return true;
    }

    public static bool ArrayContains<T>(this T[] array, T value)
    {
        return Array.IndexOf(array, value) >= 0;
    }
}