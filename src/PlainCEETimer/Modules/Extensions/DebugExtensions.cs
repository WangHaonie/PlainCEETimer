#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Extensions;

internal static class DebugExtensions
{
    [Obsolete]
    public static T Out<T, TMember>(this T obj, Func<T, TMember> selector, out TMember value)
    {
        Out(selector(obj), out value);
        return obj;
    }

    [Obsolete]
    public static T Out<T>(this T obj, out T value)
    {
        value = obj;
        return obj;
    }

    [Obsolete]
    public static T Execute<T>(this T obj, Action<T> action, int repeat = 1)
    {
        for (int i = 0; i < repeat; i++)
        {
            action(obj);
        }

        return obj;
    }

    [Obsolete]
    public static T PassIf<T>(this T obj, bool condition)
    {
        if (condition)
        {
            return obj;
        }

        return default;
    }

    [Obsolete]
    public static T CastTo<T>(this object obj)
    {
        return (T)obj;
    }

    [Obsolete]
    public static T Dump<T, TMember>(this T obj, Func<T, TMember> selector, bool dumpExp = false, [CallerArgumentExpression(nameof(selector))] string name = "")
    {
        Dump(selector(obj), dumpExp, name);
        return obj;
    }

    [Obsolete]
    public static T Dump<T>(this T obj, bool dumpExp = false, [CallerArgumentExpression(nameof(obj))] string name = "")
    {
        var json = obj is string s ? s : JsonConvert.SerializeObject(obj);

        if (App.DebugShouldDumpToConsole)
        {
            var tmp = ConsoleHelper.Instance
                .Write("[").Write(DateTime.Now.ToString("yyyy/MM/dd ddd HH:mm:ss.ffffff")).Write("] ");

            if (dumpExp)
            {
                tmp.Write(name).Write(": ");
            }

            tmp.WriteLine(json);
        }
        else
        {
            AppMessageBox.Instance.Info(name + ": " + json);
        }

        return obj;
    }

    [Obsolete]
    public static void ForEachAll<TObject, TCollection>(this TObject obj, Func<TObject, TCollection> selector, Predicate<TObject> move, Action<TObject> action)
        where TCollection : System.Collections.IEnumerable
    {
        ForEachAllEx(obj, o => selector(o).Cast<TObject>(), move, action);
    }

    [Obsolete]
    public static void ForEachAllEx<TObject, TCollection>(this TObject obj, Func<TObject, TCollection> selector, Predicate<TObject> move, Action<TObject> action)
        where TCollection : IEnumerable<TObject>
    {
        action(obj);

        if (move(obj))
        {
            var collection = selector(obj);

            foreach (var item in collection)
            {
                ForEachAllEx(item, selector, move, action);
            }
        }
    }
}
#endif