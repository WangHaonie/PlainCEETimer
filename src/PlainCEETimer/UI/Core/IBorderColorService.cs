using System.Drawing;

namespace PlainCEETimer.UI.Core;

public interface IBorderColorService
{
    bool SetBorderColor(bool enabled, Color color);
}