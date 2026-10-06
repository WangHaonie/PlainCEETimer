using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Modules.Configuration;

[NoConstants]
public class ImmersiveObject
{
    public bool PrevNext { get; set; } = true;

    public double SidebarWidth
    {
        get;
        set => field = value < MinSidebarWidth ? MinSidebarWidth : value > MaxSidebarWidth ? MaxSidebarWidth : value;
    } = DefaultSidebarWidth;

    public bool Maximize { get; set; }

    public bool FullScreen { get; set; }

    [JsonConverter(typeof(SizeFormatConverter))]
    public Size Size { get; set; }

    public const double MinSidebarWidth = 160D;

    public const double MaxSidebarWidth = 1200D;

    public const double DefaultSidebarWidth = 320D;
}
