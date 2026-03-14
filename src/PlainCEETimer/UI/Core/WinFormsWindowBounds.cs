using System;
using System.Drawing;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Core;

public class WinFormsWindowBounds : IWindowBounds
{
    public int X
    {
        get => f.Left;
        set => f.Left = value;
    }

    public int Y
    {
        get => f.Top;
        set => f.Top = value;
    }

    public int Width
    {
        get => f.Width;
        set => f.Width = value;
    }

    public int Height
    {
        get => f.Height;
        set => f.Height = value;
    }

    public Point Location
    {
        get => f.Location;
        set => f.Location = value;
    }

    public Size Size
    {
        get => f.Size;
        set => f.Size = value;
    }

    public event EventHandler SizeChanged;
    private readonly AppForm f;

    public WinFormsWindowBounds(AppForm form)
    {
        f = form;
        f.SizeChanged += (_, _) => SizeChanged?.Invoke(this, EventArgs.Empty);
    }

    public Point KeepOnScreen()
    {
        return f.KeepOnScreen();
    }
}