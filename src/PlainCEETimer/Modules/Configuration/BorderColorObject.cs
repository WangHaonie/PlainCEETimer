using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Modules.Configuration;

[JsonConverter(typeof(BorderColorConverter))]
public struct BorderColorObject
{
    public BOOL Enabled { get; set; }

    public int Type
    {
        get;
        set => Validator.SetValue(ref field, value, 3, 0);
    }

    public Color Color { get; set; }

    internal readonly int Value => ((byte)Enabled << 28) | ((byte)Type << 24) | Color.ToWin32();

    public BorderColorObject(bool enabled, int selection, Color color)
    {
        Enabled = (BOOL)enabled;
        Type = selection;
        Color = color;
    }

    internal BorderColorObject(int w)
    {
        Enabled = new((byte)((w >> 28) & 0xF));
        Type = (byte)(((w) >> 24) & 0xF);
        Color = (w & 0xFFFFFF).ToColor();
    }
}