using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Fody;

namespace PlainCEETimer.UI.Controls;

[NoConstants]
public class PlainLabel : Label, IThemeAware
{
    private const int TextExtraPadding = 2;

    private bool UseDark;
    private bool canResize;
    private ThemeHelper themeHelper;

    public PlainLabel()
    {
        AutoSize = true;

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
        using var sf = CA2SF();
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

    private StringFormat CA2SF()
    {
        var sf = new StringFormat();
        var align = TextAlign;
        sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
        sf.Trimming = StringTrimming.None;

        sf.HotkeyPrefix = UseMnemonic
            ? ShowKeyboardCues ? HotkeyPrefix.Show : HotkeyPrefix.Hide
            : HotkeyPrefix.None;

        sf.Alignment = align switch
        {
            ContentAlignment.TopCenter or ContentAlignment.MiddleCenter or ContentAlignment.BottomCenter
                => StringAlignment.Center,
            ContentAlignment.TopRight or ContentAlignment.MiddleRight or ContentAlignment.BottomRight
                => StringAlignment.Far,
            _
                => StringAlignment.Near,
        };

        sf.LineAlignment = align switch
        {
            ContentAlignment.MiddleLeft or ContentAlignment.MiddleCenter or ContentAlignment.MiddleRight
                => StringAlignment.Center,
            ContentAlignment.BottomLeft or ContentAlignment.BottomCenter or ContentAlignment.BottomRight
                => StringAlignment.Far,
            _
                => StringAlignment.Near,
        };

        if (RightToLeft == RightToLeft.Yes)
        {
            sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
        }

        if (AutoEllipsis)
        {
            sf.Trimming = StringTrimming.EllipsisCharacter;
        }

        if (!AutoSize && MaximumSize.Width <= 0)
        {
            sf.FormatFlags |= StringFormatFlags.NoWrap;
        }

        return sf;
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
        using var sf = CA2SF();
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
        var rc = DeflateBorder(ClientRectangle);
        return new RectangleF(
            rc.X + Padding.Left,
            rc.Y + Padding.Top,
            Math.Max(0, rc.Width - Padding.Horizontal),
            Math.Max(0, rc.Height - Padding.Vertical)
        );
    }

    private Rectangle DeflateBorder(Rectangle rc)
    {
        var bp = GetBorderPadding();
        return new Rectangle(
            rc.X + bp.Width,
            rc.Y + bp.Height,
            Math.Max(0, rc.Width - bp.Width * 2),
            Math.Max(0, rc.Height - bp.Height * 2)
        );
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
            BorderStyle.FixedSingle => new Size(1, 1),
            BorderStyle.Fixed3D => SystemInformation.Border3DSize,
            _ => Size.Empty,
        };
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        UpdateTheme(useDark, init);
    }
}
