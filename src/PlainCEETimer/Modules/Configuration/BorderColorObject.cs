using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Modules.Configuration;

[JsonConverter(typeof(BorderColorConverter))]
public struct BorderColorObject
{
    public bool Enabled { get; set; }

    public int Type
    {
        get;
        set => ConfigValidator.SetValue(ref field, value, 3, 0);
    }

    public Color Color { get; set; }

    internal readonly int Value => value;

    private readonly int value;

    public BorderColorObject(bool enabled, int selection, Color color)
    {
        Enabled = enabled;
        Type = selection;
        Color = color;
        value = ((byte)enabled.ToWin32() << 28) | ((byte)selection << 24) | color.ToWin32();
    }

    internal BorderColorObject(int dw)
    {
        Enabled = ((dw >> 28) & 0xF).ToBool();
        Type = ((dw) >> 24) & 0xF;
        Color = (dw & COLORREF.EmptyValue).ToColor();
        value = dw;
    }
}