using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed partial class ColorBlock : PlainLabel
{
    private sealed class ScreenColorPicker : AppForm
    {
        private int HeightWidth;
        private int MouseX;
        private int MouseY;
        private int PosOffset;
        private int XY;
        private Bitmap ScreenCut;
        private Pen CrossPen;
        private Rectangle DestRect;
        private const int HW = 64;
        private const int ScreenCutHW = 16;
        private readonly Size ScreenCutSize;
        private readonly Rectangle SourceRect;

        public Color CurrentPixelColor
        {
            get
            {
                if (ScreenCut != null)
                {
                    return ScreenCut.GetPixel(ScreenCutHW / 2, ScreenCutHW / 2);
                }

                return Color.Empty;
            }
        }

        protected override AppFormParam Params => AppFormParam.RoundCorner;

        public ScreenColorPicker()
        {
            ScreenCut = new(ScreenCutHW, ScreenCutHW);
            ScreenCutSize = new(ScreenCutHW, ScreenCutHW);
            SourceRect = new(0, 0, ScreenCutHW, ScreenCutHW);
        }

        public void UpdateFrame(Point mp)
        {
            var screen = Screen.FromPoint(mp).Bounds;
            var mx = mp.X;
            var my = mp.Y;
            int x = mx + PosOffset;
            int y = my + PosOffset;

            if (x + HeightWidth > screen.Right)
            {
                x = mx - HeightWidth - PosOffset;
            }

            if (y + HeightWidth > screen.Bottom)
            {
                y = my - HeightWidth - PosOffset;
            }

            SetLocation(x, y);
            MouseX = mx;
            MouseY = my;
            Invalidate();
        }

        protected override void OnInitializing()
        {
            Text = "屏幕拾色器 - 高考倒计时";
            TopMost = true;
        }

        protected override void OnLoad()
        {
            HeightWidth = ScaleToDpi(HW);
            XY = HeightWidth / 2;
            PosOffset = ScaleToDpi(HW / 4);
            CrossPen = new(Color.Red, ScaleToDpi(1));
            Size = new(HeightWidth, HeightWidth);
            DestRect = new(0, 0, HeightWidth + 2, HeightWidth + 2);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using var gg = Graphics.FromImage(ScreenCut);
            var g = e.Graphics;
            gg.CopyFromScreen(MouseX - (ScreenCutHW / 2), MouseY - (ScreenCutHW / 2), 0, 0, ScreenCutSize);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(ScreenCut, DestRect, SourceRect, GraphicsUnit.Pixel);
            g.DrawLine(CrossPen, PosOffset, XY, HeightWidth - PosOffset, XY);
            g.DrawLine(CrossPen, XY, PosOffset, XY, HeightWidth - PosOffset);
        }

        protected override void OnClosed()
        {
            ScreenCut.Dispose();
            CrossPen.Dispose();
            ScreenCut = null;
        }
    }

    private class CancellationMessageFilter(ColorBlock ctrl) : IMessageFilter
    {
        private static readonly IntPtr EscKey = new((int)Keys.Escape);

        public bool PreFilterMessage(ref Message m)
        {
            const int WM_KEYDOWN = 0x0100;

            if (m.Msg == WM_KEYDOWN && m.WParam == EscKey)
            {
                ctrl.CancelScreenColorPicker();
            }

            return false;
        }
    }

    private AppForm ParentForm => field ??= this.FindParentForm();

    public ColorBlock[] Fellows { get; set; } = [];

    public Color Color
    {
        get => BackColor;
        set
        {
            if (value != BackColor)
            {
                BackColor = value;

                if (PreviewBlock != null)
                {
                    if (IsFore)
                    {
                        PreviewBlock.ForeColor = value;
                    }
                    else if (!IsPreview)
                    {
                        PreviewBlock.BackColor = value;
                    }
                }

                ColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler ColorChanged;

    private bool IsDragging;
    private bool IsPicking;
    private bool IsPickingCancelled;
    private Point MouseLocation;
    private Rectangle ParentBounds;
    private ScreenColorPicker ColorPicker;
    private readonly bool IsPreview;
    private readonly bool IsFore;
    private readonly ColorBlock PreviewBlock;
    private static CancellationMessageFilter MsgFilter;

    public ColorBlock(bool isPreview, bool isFore, ColorBlock preview) : base("          ")
    {
        AutoSize = true;
        BorderStyle = BorderStyle.FixedSingle;
        IsPreview = isPreview;
        IsFore = isFore;
        PreviewBlock = preview;
    }

    protected override void OnClick(EventArgs e)
    {
        if (!IsPreview)
        {
            var dialog = new PlainColorDialog(ParentForm, Color);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Color = dialog.Color;
            }
        }

        base.OnClick(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (!IsPreview)
        {
            var mButtom = e.Button;

            if (!IsDragging && mButtom == MouseButtons.Left)
            {
                IsDragging = true;
                Capture = true;
                ParentBounds = ParentForm.Bounds;
            }
            else if (IsPicking && mButtom == MouseButtons.Right)
            {
                CancelScreenColorPicker();
            }
        }

        base.OnMouseDown(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (!IsPreview && IsDragging)
        {
            Cursor = Cursors.Cross;
            MouseLocation = Cursor.Position;

            if (!IsPicking && !ParentBounds.Contains(MouseLocation))
            {
                IsPicking = true;
                MsgFilter ??= new(this);
                Application.AddMessageFilter(MsgFilter);
                ColorPicker = new();
                HideParentForm();
                ColorPicker.Show();
            }

            if (IsPicking)
            {
                ColorPicker.UpdateFrame(MouseLocation);
            }
        }

        base.OnMouseMove(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        EndPicking();
        base.OnMouseUp(e);
    }

    private void EndPicking()
    {
        if (!IsPreview && IsDragging)
        {
            IsDragging = false;
            Cursor = Cursors.Default;
            Capture = false;

            var parent = base.Parent;
            var target = parent.GetChildAtPoint(parent.PointToClient(MouseLocation));

            if (target != null && target is ColorBlock block && Fellows.Contains(block) && block != this)
            {
                block.Color = Color;
            }

            if (IsPicking)
            {
                if (IsPickingCancelled)
                {
                    IsPickingCancelled = false;
                }
                else
                {
                    Color = ColorPicker.CurrentPixelColor;
                }

                Application.RemoveMessageFilter(MsgFilter);
                HideParentForm(false);
                ColorPicker.Close();
                IsPicking = false;
            }
        }
    }

    private void CancelScreenColorPicker()
    {
        IsPickingCancelled = true;
        EndPicking();
    }

    private void HideParentForm(bool hide = true)
    {
        ParentForm.Opacity = hide ? 0 : 1;
    }
}
