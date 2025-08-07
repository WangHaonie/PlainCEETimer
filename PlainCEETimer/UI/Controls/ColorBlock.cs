using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PlainCEETimer.UI.Forms;

namespace PlainCEETimer.UI.Controls
{
    public sealed class ColorBlock : PlainLabel
    {
        private class CancellationMessageFilter(Action onCancel) : IMessageFilter
        {
            private const int WM_KEYDOWN = 0x0100;
            private static readonly IntPtr EscKey = new((int)Keys.Escape);

            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == WM_KEYDOWN && m.WParam == EscKey)
                {
                    onCancel?.Invoke();
                }

                return false;
            }
        }

        public new AppForm Parent { get; set; }

        public ColorBlock[] Fellows { get; set; }

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
        private CancellationMessageFilter MsgFilter;

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
                var dialog = new PlainColorDialog();

                if (dialog.ShowDialog(Color, Parent) == DialogResult.OK)
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

                if (mButtom == MouseButtons.Left)
                {
                    IsDragging = true;
                    Capture = true;
                    ParentBounds = Parent.Bounds;
                }
                else if (IsDragging && IsPicking && mButtom == MouseButtons.Right)
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
                    MsgFilter ??= new(CancelScreenColorPicker);
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

            base.OnMouseUp(e);
        }

        private void CancelScreenColorPicker()
        {
            IsPickingCancelled = true;
            OnMouseUp(null);
        }

        private void HideParentForm(bool hide = true)
        {
            Parent.Opacity = hide ? 0 : 1;
        }
    }
}
