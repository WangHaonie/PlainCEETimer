using PlainCEETimer.Countdown;

namespace PlainCEETimer.UI;

public class PreferredColors(ColorPair cpLight, ColorPair cpDark)
{
    public ColorPair Light => cpLight;

    public ColorPair Dark => cpDark;
}