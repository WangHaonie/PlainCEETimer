using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
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

    public BorderColorObject(int words)
    {
        Enabled = new((byte)((words >> 28) & 0xF));
        Type = (byte)(((words) >> 24) & 0xF);
        Color = (words & 0xFFFFFF).ToColor();
    }

    public BorderColorObject(bool enabled, int selection, Color color)
    {
        Enabled = (BOOL)enabled;
        Type = selection;
        Color = color;
    }

    internal readonly int MakeLong()
    {
        return ((byte)Enabled << 28) | ((byte)Type << 24) | Color.ToWin32();
    }
}