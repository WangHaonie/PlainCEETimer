using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlainCEETimer.Countdown;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Extensions;

public static class Extensions
{
    public static bool? AsBoolean(this DialogResult dialogResult) => dialogResult switch
    {
        DialogResult.Yes or DialogResult.OK => true,
        DialogResult.None or DialogResult.Ignore => null,
        _ => false
    };

    public static int ToInt32(this Color color)
        => -color.ToArgb();

    public static Color ToColor(this int value)
        => ColorTranslator.FromOle(value);

    public static string Format(this Font font)
        => $"{font.Name}, {font.Size}pt, {font.Style}";

    public static void Destory(this IDisposable obj)
        => obj?.Dispose();

    public static T Copy<T>(this T obj) where T : ICloneable
        => (T)obj.Clone();

    public static bool IsEnabled(this ExamSettings settings)
        => settings != null && settings.Enabled;

    public static StreamingContext SetContext<T>(this StreamingContext sc, T value, out StreamingContext original)
    {
        original = sc;
        return new(sc.State, value);
    }

    public static bool CheckContext<T>(this StreamingContext sc, T expectation)
    {
        var context = sc.Context;

        if (context != null)
        {
            return context.Equals(expectation);
        }

        return false;
    }

    public static ConsoleColor ToConsoleColor(this UacNotifyLevel level) => level switch
    {
        UacNotifyLevel.AlwaysDimmed => ConsoleColor.Green,
        UacNotifyLevel.AppsOnlyDimmed => ConsoleColor.Green,
        UacNotifyLevel.AppsOnlyNoDimmed => ConsoleColor.Yellow,
        UacNotifyLevel.NeverNotify => ConsoleColor.Yellow,
        UacNotifyLevel.Disabled => ConsoleColor.Red,
        _ => ConsoleColor.Gray,
    };

    public static ConsoleColor ToConsoleColor(this AdminRights ar) => ar switch
    {
        AdminRights.Yes => ConsoleColor.Green,
        AdminRights.No => ConsoleColor.Red,
        _ => ConsoleColor.Gray
    };

    public static JsonReadHelper Load(this JsonReader reader, JsonSerializer serializer)
    {
        return new(JObject.Load(reader), serializer);
    }

#if DEBUG
    public static T Dump<T>(this T obj)
    {
        AppMessageBox.Instance.Info(JsonConvert.SerializeObject(obj, Formatting.Indented));
        return obj;
    }
#endif
}