using System.ComponentModel;
using PlainCEETimer.Countdown;
using PlainCEETimer.UI;

namespace PlainCEETimer.Modules.Configuration;

public class DisplayObject
{
    [DefaultValue(2)]
    public int EndIndex
    {
        get;
        set => Validator.SetValue(ref field, value, 2, 0, 2);
    } = 2;

    public CountdownFormat Format { get; set; }

    public int ScreenIndex { get; set; }

    [DefaultValue(CountdownPosition.TopCenter)]
    public CountdownPosition Position { get; set; } = CountdownPosition.TopCenter;

    public bool Draggable { get; set; }

    public bool SeewoPptsvc { get; set; }
}