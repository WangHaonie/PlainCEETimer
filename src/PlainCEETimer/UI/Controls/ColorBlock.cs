using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Annotations.Fody;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Linq;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class ColorBlock : PlainLabel
{
    [NoConstants]
    private sealed class ScreenColorPicker : AppForm
    {
        public Color Result => CurrentColor;

        protected override AppWindowStyle Params => AppWindowStyle.RoundCorner;

        private int InfoHeight;
        private int InfoLineHeight;
        private int WindowHeight;
        private int MouseX;
        private int MouseY;
        private int CrossPosX;
        private int CrossPosY;
        private int ScreenshotSize;
        private int ScreenshotSample;
        private int PreviewSize;
        private int PreviewMargin;
        private string strpos;
        private string strrgb;
        private string strhex;
        private Color CurrentColor;
        private Size ScreenshotSampleSize;
        private Rectangle ScreenshotRect;
        private Rectangle ScreenshotSampleRect;
        private Rectangle PreviewRect;
        private Pen CrossPen;
        private Pen BorderPen;
        private Bitmap Screenshot;
        private Graphics gScreenshot;

        private const int _ScreenshotSize = 72;
        private const int _ScreenshotSample = 16;
        private const int _PreviewSize = 22;
        private const int _PreviewMargin = 4;
        private const int InfoLines = 3;

        public void UpdateFrame(Point mp)
        {
            var screen = Screen.FromPoint(mp).Bounds;
            var mx = mp.X;
            var my = mp.Y;
            int x = mx + CrossPosX;
            int y = my + CrossPosX;

            if (x + Width > screen.Right)
            {
                x = mx - Width - CrossPosX;
            }

            if (y + WindowHeight > screen.Bottom)
            {
                y = my - WindowHeight - CrossPosX;
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
            ScreenshotSize = ScaleToDpi(_ScreenshotSize);
            ScreenshotSample = ScaleToDpi(_ScreenshotSample);
            PreviewSize = ScaleToDpi(_PreviewSize);
            PreviewMargin = ScaleToDpi(_PreviewMargin);
            CrossPen = new(Color.Red, ScaleToDpi(1F));
            BorderPen = new(Color.Black, ScaleToDpi(1F));

            CrossPosY = ScreenshotSize / 2;
            CrossPosX = ScreenshotSize / 4;
            InfoLineHeight = FontHeight;
            InfoHeight = InfoLineHeight * InfoLines;
            WindowHeight = ScreenshotSize + InfoHeight;

            Size = new(ScreenshotSize, WindowHeight);
            Screenshot = new(ScreenshotSample, ScreenshotSample);
            ScreenshotSampleSize = new(ScreenshotSample, ScreenshotSample);
            ScreenshotSampleRect = new(0, 0, ScreenshotSample, ScreenshotSample);
            ScreenshotRect = new(0, 0, ScreenshotSize + 1, ScreenshotSize);
            PreviewRect = new(
                ScreenshotSize - PreviewMargin - PreviewSize,
                ScreenshotSize - PreviewMargin - PreviewSize,
                PreviewSize, PreviewSize);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            gScreenshot = Graphics.FromImage(Screenshot);
            UpdateInfo();
            DrawScreenshot(g);
            DrawPreview(g);
            DrawInfoText(g);
            gScreenshot.Destroy();
        }

        protected override void Dispose(bool disposing)
        {
            Screenshot.Destroy();
            CrossPen.Destroy();
            BorderPen.Destroy();
            base.Dispose(disposing);
        }

        private void UpdateInfo()
        {
            CurrentColor = Screenshot.GetPixel(ScreenshotSample / 2, ScreenshotSample / 2);
            strpos = $"{MouseX},{MouseY}";
            strrgb = $"{CurrentColor.R},{CurrentColor.G},{CurrentColor.B}";
            strhex = $"#{CurrentColor.R:X2}{CurrentColor.G:X2}{CurrentColor.B:X2}";
        }

        private void DrawScreenshot(Graphics g)
        {
            gScreenshot.CopyFromScreen(MouseX - (ScreenshotSample / 2), MouseY - (ScreenshotSample / 2), 0, 0, ScreenshotSampleSize);

            g.DrawImage(Screenshot, ScreenshotRect, ScreenshotSampleRect, GraphicsUnit.Pixel);
            g.DrawLine(CrossPen, CrossPosX, CrossPosY, ScreenshotSize - CrossPosX, CrossPosY);
            g.DrawLine(CrossPen, CrossPosY, CrossPosX, CrossPosY, ScreenshotSize - CrossPosX);
        }

        private void DrawPreview(Graphics g)
        {
            using var b = new SolidBrush(CurrentColor);

            g.FillRectangle(b, PreviewRect);
            g.DrawRectangle(BorderPen, PreviewRect);
        }

        private void DrawInfoText(Graphics g)
        {
            using var b = new SolidBrush(ForeColor);
            var rc = new RectangleF(0, ScreenshotSize, Width, InfoLineHeight);
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

    private AppForm ParentForm => field ??= this.FindParentForm();

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
