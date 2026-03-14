using System.Drawing;

namespace PlainCEETimer.UI.Core;

public interface IBorderColorService
{
    void SetBorderColor(bool enabled, Color color);
}