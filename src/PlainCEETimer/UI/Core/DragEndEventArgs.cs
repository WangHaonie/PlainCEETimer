using System.Windows;

namespace PlainCEETimer.UI.Core;

public class DragEndEventArgs(Point wpOld, Point wpNew)
{
    public Point WindowLocationOld => wpOld;

    public Point WindowLocationNew => wpNew;

    public bool LocationChanged { get; } = wpOld != wpNew;
}