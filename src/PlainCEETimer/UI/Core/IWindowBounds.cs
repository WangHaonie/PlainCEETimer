using System;
using System.Drawing;

namespace PlainCEETimer.UI.Core;

public interface IWindowBounds
{
    int X { get; set; }

    int Y { get; set; }

    int Width { get; set; }

    int Height { get; set; }

    Point Location { get; set; }

    Size Size { get; set; }

    event EventHandler SizeChanged;

    Point KeepOnScreen();
}