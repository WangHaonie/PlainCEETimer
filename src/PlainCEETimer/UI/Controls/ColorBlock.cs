using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Fody;
using PlainCEETimer.Modules.Linq;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class ColorBlock : PlainLabel
{
    [NoConstants]
    private sealed class ScreenColorPicker : AppForm
    {
        public Color Result => color;

        protected override AppWindowStyle Params => AppWindowStyle.RoundCorner;

        private int InfoHeight;
        private int InfoLineHeight;
        private int WindowHeight;
        private int MouseX;
        private int MouseY;
        private int PosOffset;
        private int XY;
        private string strpos;
        private string strrgb;
        private string strhex;
        private Pen CrossPen;
        private Pen BorderPen;
        private Bitmap ScreenCut;
        private Graphics gScreenCut;
        private Color color;
        private Size ScreenCutSize;
        private Rectangle DestRect;
        private Rectangle PreviewRect;
        private Rectangle SourceRect;

        private int HW = 72;
        private int ScreenCutHW = 16;
        private int PreviewHW = 22;
        private int PreviewMargin = 4;

        private const int InfoLines = 3;

        public void UpdateFrame(Point mp)
        {
            var screen = Screen.FromPoint(mp).Bounds;
            var mx = mp.X;
            var my = mp.Y;
            int x = mx + PosOffset;
            int y = my + PosOffset;

            if (x + Width > screen.Right)
            {
                x = mx - Width - PosOffset;
            }

            if (y + WindowHeight > screen.Bottom)
            {
                y = my - WindowHeight - PosOffset;
            }

            SetLocation(x, y);
            MouseX = mx;
            MouseY = my;
            Invalidate();
        }

        public string GetInfoLine(int line) => line switch
        {
            1 => strpos,
            2 => strrgb,
            3 => strhex,
            _ => null
        };

        protected override void OnInitializing()
        {
            Text = "屏幕拾色器 - 高考倒计时";
            TopMost = true;
        }

        protected override void RunLayout(bool isHighDpi)
        {
            HW = ScaleToDpi(HW);
            ScreenCutHW = ScaleToDpi(ScreenCutHW);
            PreviewHW = ScaleToDpi(PreviewHW);
            PreviewMargin = ScaleToDpi(PreviewMargin);
            CrossPen = new(Color.Red, ScaleToDpi(1F));
            BorderPen = new(Color.Black, ScaleToDpi(1F));

            XY = HW / 2;
            PosOffset = HW / 4;
            InfoLineHeight = FontHeight;
            InfoHeight = InfoLineHeight * InfoLines;
            WindowHeight = HW + InfoHeight;

            Size = new(HW, WindowHeight);
            ScreenCut = new(ScreenCutHW, ScreenCutHW);
            ScreenCutSize = new(ScreenCutHW, ScreenCutHW);
            SourceRect = new(0, 0, ScreenCutHW, ScreenCutHW);
            DestRect = new(0, 0, HW, HW);
            PreviewRect = new(
                HW - PreviewMargin - PreviewHW,
                HW - PreviewMargin - PreviewHW,
                PreviewHW, PreviewHW);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            gScreenCut = Graphics.FromImage(ScreenCut);
            UpdateInfo();
            DrawScreenshot(g);
            DrawPreview(g);
            DrawInfoText(g);
            gScreenCut.Destroy();
        }

        protected override void Dispose(bool disposing)
        {
            ScreenCut.Destroy();
            CrossPen.Destroy();
            BorderPen.Destroy();
            base.Dispose(disposing);
        }

        private void UpdateInfo()
        {
            color = ScreenCut.GetPixel(ScreenCutHW / 2, ScreenCutHW / 2);
            strpos = $"{MouseX},{MouseY}";
            strrgb = $"{color.R},{color.G},{color.B}";
            strhex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        private void DrawScreenshot(Graphics g)
        {
            gScreenCut.CopyFromScreen(MouseX - (ScreenCutHW / 2), MouseY - (ScreenCutHW / 2), 0, 0, ScreenCutSize);

            g.DrawImage(ScreenCut, DestRect, SourceRect, GraphicsUnit.Pixel);
            g.DrawLine(CrossPen, PosOffset, XY, HW - PosOffset, XY);
            g.DrawLine(CrossPen, XY, PosOffset, XY, HW - PosOffset);
        }

        private void DrawPreview(Graphics g)
        {
            using var fillBrush = new SolidBrush(color);

            g.FillRectangle(fillBrush, PreviewRect);
            g.DrawRectangle(BorderPen, PreviewRect);
        }

        private void DrawInfoText(Graphics g)
        {
            using var b = new SolidBrush(ForeColor);
            var rc = new RectangleF(0, HW, Width, InfoLineHeight);
            var font = Font;

            g.DrawString(strpos, font, b, rc);
            rc.Y += InfoLineHeight;
            g.DrawString(strrgb, font, b, rc);
            rc.Y += InfoLineHeight;
            g.DrawString(strhex, font, b, rc);
        }
    }

    private class CancellationMessageFilter(ColorBlock ctrl) : IAppMessageFilter
    {
        public unsafe bool OnMessage(MSG* lpMsg)
        {
            return lpMsg->message == WM.KEYDOWN && ctrl.WmKeyDown((Keys)(int)lpMsg->wParam);
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
    private readonly CancellationMessageFilter MsgFilter;

    public ColorBlock(bool isPreview, bool isFore, ColorBlock preview)
    {
        Text = "          ";
        BorderStyle = BorderStyle.FixedSingle;
        IsPreview = isPreview;
        IsFore = isFore;
        PreviewBlock = preview;
        MsgFilter = new(this);
    }

    protected override void OnClick(EventArgs e)
    {
        if (!IsPreview)
        {
            var dialog = new PlainColorDialog(ParentForm, Color);

            if (dialog.ShowDialog() == true)
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
                AppMessageFilter.AddMessageFilter(MsgFilter);
                ColorPicker = new();
                HideParentForm();
                ColorPicker.Show();
                ColorPicker.Activate();
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

            var parent = Parent;

            if (parent != null)
            {
                var target = parent.GetChildAtPoint(parent.PointToClient(MouseLocation));

                if (target != null && target is ColorBlock block && Fellows.ArrayContains(block) && block != this)
                {
                    block.Color = Color;
                }
            }

            if (IsPicking)
            {
                if (IsPickingCancelled)
                {
                    IsPickingCancelled = false;
                }
                else
                {
                    Color = ColorPicker.Result;
                }

                AppMessageFilter.RemoveMessageFilter(MsgFilter);
                HideParentForm(false);
                ColorPicker.Close();
                ColorPicker = null;
                IsPicking = false;
            }
        }
    }

    private void CancelScreenColorPicker()
    {
        IsPickingCancelled = true;
        EndPicking();
    }

    private bool WmKeyDown(Keys vk)
    {
        switch (vk)
        {
            case Keys.Escape:
                CancelScreenColorPicker();
                return true;
            case Keys.D1 or Keys.NumPad1:
                CopyInfoToClipboard(1);
                return true;
            case Keys.D2 or Keys.NumPad2:
                CopyInfoToClipboard(2);
                return true;
            case Keys.D3 or Keys.NumPad3:
                CopyInfoToClipboard(3);
                return true;
            default:
                return false;
        }
    }

    private void CopyInfoToClipboard(int id)
    {
        if (ColorPicker != null)
        {
            var info = ColorPicker.GetInfoLine(id);

            if (info != null)
            {
                Clipboard.SetText(info);
            }
        }
    }

    private void HideParentForm(bool hide = true)
    {
        ParentForm.Hide(hide);
    }
}
