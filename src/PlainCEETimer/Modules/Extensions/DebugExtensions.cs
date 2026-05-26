#if DEBUG
using System;
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
    public static T Dump<T, TMember>(this T obj, Func<T, TMember> selector, [CallerArgumentExpression(nameof(selector))] string name = "")
    {
        Dump(selector(obj), name);
        return obj;
    }

    [Obsolete]
    public static T Dump<T>(this T obj, [CallerArgumentExpression(nameof(obj))] string name = "")
    {
        var json = obj is string s ? s : JsonConvert.SerializeObject(obj);

        if (App.DebugShouldDumpToConsole)
        {
            var tmp = ConsoleHelper.Instance
                .Write("[").Write(DateTime.Now.ToString("yyyy/MM/dd ddd HH:mm:ss.ffffff")).Write("] ");

            if (App.DebugShouldDumpExpression)
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
}
#endif