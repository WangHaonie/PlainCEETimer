using System.Drawing;
using Newtonsoft.Json;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.Modules.JsonConverters;

namespace PlainCEETimer.Modules.Configuration;

[NoConstants]
public class ImmersiveObject
{
    public bool PrevNext { get; set; }

    public bool Loop { get; set; }

    public bool SkipExams { get; set; }

    public bool NoAnimate { get; set; }

    public double SidebarWidth
    {
        get;
        set => field = value < MinSidebarWidth ? MinSidebarWidth : value > MaxSidebarWidth ? MaxSidebarWidth : value;
    } = DefaultSidebarWidth;

    public bool Maximize { get; set; }

    public bool FullScreen { get; set; }

    public bool TopMost { get; set; }

    public bool UnTopMost { get; set; }

    [JsonConverter(typeof(SizeFormatConverter))]
    public Size Size { get; set; }

    public const double MinSidebarWidth = 160D;

    public const double MaxSidebarWidth = 1200D;

    public const double DefaultSidebarWidth = 320D;

    public ImmersiveObject CreateCopy()
    {
        return new()
        {
            SidebarWidth = SidebarWidth,
            Maximize = Maximize,
            FullScreen = FullScreen,
            TopMost = TopMost,
            Size = Size
        };
    }
}
