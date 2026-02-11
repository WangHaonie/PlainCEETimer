using System.Diagnostics;
using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Modules.Configuration;

[DebuggerDisplay("{Maximize}, {Size}")]
public class WindowSizeObject(bool maximized, Size size)
{
    public bool Maximize { get; set; } = maximized;

    [JsonConverter(typeof(SizeFormatConverter))]
    public Size Size { get; set; } = size;
}