using System;
using System.Drawing;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Core;

public class WinFormsWindowBounds(AppForm form) : IWindowBounds
{
    public int X
    {
        get => form.Left;
        set => form.Left = value;
    }

    public int Y
    {
        get => form.Top;
        set => form.Top = value;
    }

    public int Width
    {
        get => form.Width;
        set => form.Width = value;
    }

    public int Height
    {
        get => form.Height;
        set => form.Height = value;
    }

    public Point Location
    {
        get => form.Location;
        set => form.Location = value;
    }

    public Size Size
    {
        get => form.Size;
        set => form.Size = value;
    }

    public event EventHandler SizeChanged
    {
        add => form.SizeChanged += value;
        remove => form.SizeChanged -= value;
    }

    public Point KeepOnScreen()
    {
        return form.KeepOnScreen();
    }
}