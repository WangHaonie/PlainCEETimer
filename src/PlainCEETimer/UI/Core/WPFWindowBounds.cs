using System;
using System.Drawing;
using PlainCEETimer.WPF.Controls;

namespace PlainCEETimer.UI.Core;

public class WPFWindowBounds : IWindowBounds
{
    public int X
    {
        get => w.Dip2PxX(w.Left);
        set => w.Left = w.Px2DipX(value);
    }

    public int Y
    {
        get => w.Dip2PxY(w.Top);
        set => w.Top = w.Px2DipY(value);
    }

    public int Width
    {
        get => w.Dip2PxX(w.ActualWidth);
        set => w.Width = w.Px2DipX(value);
    }

    public int Height
    {
        get => w.Dip2PxY(w.ActualHeight);
        set => w.Height = w.Px2DipY(value);
    }

    public Point Location
    {
        get => w.Dip2Px(w.Location);
        set => w.Location = w.Px2Dip(value);
    }

    public Size Size
    {
        get => w.Dip2Px(w.Size);
        set => w.Px2Dip(value);
    }

    public event EventHandler SizeChanged;

    private readonly AppWindow w;

    public WPFWindowBounds(AppWindow window)
    {
        w = window;
        w.SizeChanged += (_, _) => SizeChanged?.Invoke(this, EventArgs.Empty);
    }

    public Point KeepOnScreen()
    {
        return w.Dip2Px(w.KeepOnScreen());
    }
}