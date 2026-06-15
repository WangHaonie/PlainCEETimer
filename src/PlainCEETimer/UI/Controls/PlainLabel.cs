using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Fody;
using PlainCEETimer.Modules.Internals;

namespace PlainCEETimer.UI.Controls;

[NoConstants]
public class PlainLabel : Label, IThemeAware
{
    private const int TextExtraPadding = 2;

    private bool UseDark;
    private bool canResize;
    private ThemeHelper themeHelper;
    private readonly LabelInternals internals;

    public PlainLabel()
    {
        AutoSize = true;
        UseCompatibleTextRendering = false;
        internals = LabelInternals.AttachTo(this);

        SetStyle(ControlStyles.UserPaint
            | ControlStyles.AllPaintingInWmPaint
            | ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.SupportsTransparentBackColor, true);
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        var sz = MeasureText(proposedSize);
        var bsz = GetBorderSize();
        var p = Padding;

        return new Size(
            sz.Width + p.Horizontal + bsz.Width + TextExtraPadding,
            sz.Height + p.Vertical + bsz.Height
        );
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        themeHelper ??= new(this);
        base.OnHandleCreated(e);
        UpdateAutoSize();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        using var sf = internals.CreateStringFormat();
        using var b = new SolidBrush(GetForeColor());
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        g.DrawString(Text, Font, b, GetTextRectangle(), sf);
    }

    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
        if (BackColor == Color.Transparent)
        {
            base.OnPaintBackground(pevent);
            return;
        }

        using var brush = new SolidBrush(BackColor);
        pevent.Graphics.FillRectangle(brush, ClientRectangle);
    }

    protected override void Dispose(bool disposing)
    {
        themeHelper.Destroy();
        base.Dispose(disposing);
    }

    protected virtual void UpdateTheme(bool useDark, bool init)
    {
        UseDark = useDark;

        if (!init && !Enabled)
        {
            Invalidate();
        }
    }

    private void UpdateAutoSize()
    {
        if (!AutoSize || canResize || !IsHandleCreated)
        {
            Invalidate();
            return;
        }

        canResize = true;

        try
        {
            Size = GetPreferredSize(Size.Empty);
        }
        finally
        {
            canResize = false;
        }
    }

    private Size MeasureText(Size proposedSize)
    {
        using var g = CreateGraphics();
        using var sf = internals.CreateStringFormat();
        var sz = g.MeasureString(Text, Font, new SizeF(GetLayoutWidth(proposedSize.Width), float.MaxValue), sf);
        return new Size((int)Math.Ceiling(sz.Width), (int)Math.Ceiling(sz.Height));
    }

    private Color GetForeColor()
    {
        if (Enabled)
        {
            return ForeColor;
        }

        return UseDark ? Colors.DarkForeTextDisabled : SystemColors.GrayText;
    }

    private int GetLayoutWidth(int proposedWidth)
    {
        var horzw = Padding.Horizontal + GetBorderSize().Width;

        if (MaximumSize.Width > 0)
        {
            return Math.Max(1, MaximumSize.Width - horzw);
        }

        if (!AutoSize && proposedWidth > 0)
        {
            return Math.Max(1, proposedWidth - horzw);
        }

        return int.MaxValue / 4;
    }

    private RectangleF GetTextRectangle()
    {
        var rc = ClientRectangle;
        var p = Padding;
        var bp = GetBorderPadding();

        float x = rc.X + bp.Width + p.Left;
        float y = rc.Y + bp.Height + p.Top;
        float cx = Math.Max(0, rc.Width - bp.Width * 2 - p.Horizontal);
        float cy = Math.Max(0, rc.Height - bp.Height * 2 - p.Vertical);

        if (BorderStyle == BorderStyle.None)
        {
            x += DeviceDpi / 96F > 1F ? 3F : 1F;
            y += 1F;
        }

        return new(x, y, cx, cy);
    }

    private Size GetBorderSize()
    {
        var bp = GetBorderPadding();
        return new Size(bp.Width * 2, bp.Height * 2);
    }

    private Size GetBorderPadding()
    {
        return BorderStyle switch
        {
            BorderStyle.FixedSingle => SystemInformation.GetBorderSizeForDpi(DeviceDpi),
            BorderStyle.Fixed3D => SystemInformationEx.GetBorder3DSizeForDpi(DeviceDpi),
            _ => Size.Empty,
        };
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        UpdateTheme(useDark, init);
    }
}
