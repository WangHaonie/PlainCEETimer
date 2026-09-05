#if DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Extensions;

internal static class DebugExtensions
{
    private static class MemoryLayoutViewer
    {
        private class FieldAndOffset(FieldInfo info, int offset)
        {
            public FieldInfo Field => info;

            public int Offset => offset;
        }

        internal static void PrintLayout<T>()
        {
            static int OffsetOf(Type type, string name)
            {
                try { return Marshal.OffsetOf(type, name).ToInt32(); }
                catch { return -1; }
            }

            static int SizeOf(Type type)
            {
                try { return Marshal.SizeOf(type); }
                catch { return Unsafe.SizeOf<T>(); }
            }

            var type = typeof(T);

            if (type.IsValueType)
            {
                var data = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Select(f => new FieldAndOffset(f, OffsetOf(type, f.Name)))
                    .OrderBy(x => x.Offset).ToList();

                var c = ConsoleHelper.Instance;
                c.Write("----- Memory Layout of ").Write(type.FullName, ConsoleColor.DarkGreen).Write(" (").Write(SizeOf(type)).WriteLine(" bytes) -----");

                foreach (var item in data)
                {
                    c.Write("[Offset: ").Write(item.Offset).Write(", Size: ")
                        .Write(SizeOf(item.Field.FieldType)).Write("] ")
                        .Write(item.Field.FieldType.FullName).Write(" ").WriteLine(item.Field.Name);
                }

                c.WriteLine();
            }
        }
    }

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
            if (Debugger.IsAttached)
            {
                Debug.WriteLine(json);
                goto ret;
            }

            var tmp = ConsoleHelper.Instance
                .Write("[").Write(DateTime.Now.LogFormat()).Write("] ");

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

    ret:
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

    [Obsolete]
    public static void DumpLayout<T>(this T obj)
        where T : struct
    {
        MemoryLayoutViewer.PrintLayout<T>();
    }

    public static string LogFormat(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy/MM/dd ddd HH:mm:ss.ffffff");
    }
}
#endif