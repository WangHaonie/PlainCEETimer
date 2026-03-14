using System;
using System.Drawing;

namespace PlainCEETimer.UI.Core;

public interface IWindowBounds
{
    public int X { get; set; }

    public int Y { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public Point Location { get; set; }

    public Size Size { get; set; }

    event EventHandler SizeChanged;

    public Point KeepOnScreen();
}