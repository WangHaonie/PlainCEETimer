using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.UI.Forms;

namespace PlainCEETimer.UI.Controls
{
    public class AppForm : Form
    {
        /// <summary>
        /// 获取当前 <see cref="AppForm"/> 的消息框实例。
        /// </summary>
        public MessageBoxHelper MessageX { get; }

        protected event Action LocationRefreshed;

        private bool IsLoading = true;
        private AppFormParam Params;
        private readonly bool Special;

        public static float CurrentDpiRatio;
        private static readonly Font AppFont;
        private static readonly int CurrentFontHeight;

        protected AppForm(AppFormParam param)
        {
            Params = param;
            Special = CheckParam(AppFormParam.Special);
            App.TrayMenuShowAllClicked += AppLauncher_TrayMenuShowAllClicked;
            MessageX = new(this);

            if (!Special)
            {
                MainForm.UniTopMostChanged += MainForm_UniTopMostChanged;
                MainForm_UniTopMostChanged();
            }

            SuspendLayout();
            AutoScaleDimensions = new(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Font = AppFont;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            ShowIcon = false;
            OnInitializing();
            ResumeLayout(true);
        }

        static AppForm()
        {
            AppFont = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            CurrentFontHeight = AppFont.Height;
        }

        public void ReActivate()
        {
            var tmp = TopMost;
            TopMost = true;
            WindowState = FormWindowState.Normal;
            Show();
            Activate();
            TopMost = tmp;
            KeepOnScreen();
        }

        protected sealed override void OnLoad(EventArgs e)
        {
            SuspendLayout();
            StartLayout(CurrentDpiRatio > 1F);
            ResumeLayout(true);
            OnLoad();
            base.OnLoad(e);

            if (CheckParam(AppFormParam.CenterScreen))
            {
                MoveToScreenCenter(GetCurrentScreenRect());
            }
        }

        protected sealed override void OnShown(EventArgs e)
        {
            IsLoading = false;
            OnShown();
            base.OnShown(e);
        }

        protected sealed override void OnFormClosing(FormClosingEventArgs e)
        {
            var reason = e.CloseReason;
            e.Cancel = reason != CloseReason.WindowsShutDown && OnClosing(reason);
            base.OnFormClosing(e);
        }

        protected sealed override void OnClosed(EventArgs e)
        {
            OnClosed();
            base.OnClosed(e);
            Dispose(true);
        }

        #region
        /*
        
        解决窗体因控件较多导致的闪烁问题 参考:

        winform窗体闪烁问题解决 - 就叫我雷人吧 - 博客园
        https://www.cnblogs.com/guosheng/p/7417918.html

         */

        protected sealed override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;

                if (CheckParam(AppFormParam.CompositedStyle))
                {
                    cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                }

                return cp;
            }
        }
        #endregion

        protected override void OnHandleCreated(EventArgs e)
        {
            if (CurrentDpiRatio == 0F)
            {
                var g = CreateGraphics();
                CurrentDpiRatio = g.DpiX / 96F;
                g.Dispose();
            }

            if (ThemeManager.ShouldUseDarkMode)
            {
                if (!Special)
                {
                    ForeColor = ThemeManager.DarkFore;
                    BackColor = ThemeManager.DarkBack;
                }

                ThemeManager.FlushWindow(Handle);
            }

            base.OnHandleCreated(e);
        }

        /// <summary>
        /// 在 <see cref="AppForm"/> 完成设定基本选项时触发，可用于让派生类修改有关窗体的属性，这将覆盖先前设定的选项。
        /// 该方法没有默认实现，可不调用 base.OnInitialize();
        /// </summary>
        protected virtual void OnInitializing() { }

        /// <summary>
        /// 在 <see cref="AppForm"/> OnLoad 之前触发，可用于对控件进行最后的布局。
        /// 该方法没有默认实现，可不调用 base.StartLayout(bool);
        /// </summary>
        protected virtual void StartLayout(bool isHighDpi) { }

        /// <summary>
        /// 在 <see cref="AppForm"/> 加载时触发。该方法没有默认实现，直接派生自 <see cref="AppForm"/> 的类可不调用 base.OnLoad();
        /// </summary>
        protected virtual void OnLoad() { }

        /// <summary>
        /// 在 <see cref="AppForm"/> 已向用户显示时触发。该方法没有默认实现，可不调用 base.OnShown();
        /// </summary>
        protected virtual void OnShown() { }

        /// <summary>
        /// 在 <see cref="AppForm"/> 被关闭时触发。该方法默认返回 <see langword="false"/>，可不调用 base.OnClosing(CloseReason);
        /// </summary>
        /// <returns><see langword="true"/> 则取消关闭窗口, <see langword="false"/> 则允许关闭窗口</returns>
        protected virtual bool OnClosing(CloseReason closeReason) => false;

        /// <summary>
        /// 在 <see cref="AppForm"/> 关闭后触发。该方法没有默认实现，可不调用 base.OnClosed();
        /// </summary>
        protected virtual void OnClosed() { }

        protected bool CheckParam(AppFormParam param)
        {
            return (Params & param) == param;
        }

        /// <summary>
        /// 在用户未保存更改并尝试关闭窗体时显示警告。同时防止直接关闭警告时也窗体会随之关闭。
        /// </summary>
        protected bool ShowUnsavedWarning(string WarningMsg, Func<bool> SaveChanges, ref bool flagUserChanged)
        {
            switch (MessageX.Warn(WarningMsg, buttons: MessageButtons.YesNo))
            {
                case DialogResult.Yes:
                    return !SaveChanges();
                case DialogResult.No:
                    flagUserChanged = false;
                    Close();
                    return false;
                default:
                    return true;
            }
        }

        protected int ScaleToDpi(int px)
        {
            return (int)(px * CurrentDpiRatio);
        }

        protected Rectangle GetCurrentScreenRect()
        {
            return Special ? Screen.GetWorkingArea(this) : Screen.GetWorkingArea(Cursor.Position);
        }

        /// <summary>
        /// 参考指定控件，在 X 方向上水平排列目标控件
        /// </summary>
        protected void ArrangeControlX(Control target, Control reference, int xOffset = 0)
        {
            target.Left = reference.Right + ScaleToDpi(xOffset);
        }

        /// <summary>
        /// 参考指定控件，在 X 方向上水平排列目标控件，并在 Y 方向上与指定控件的上边缘对齐
        /// </summary>
        protected void ArrangeControlXTop(Control target, Control reference, int xOffset = 0, int yOffset = 0)
        {
            target.Left = reference.Right + ScaleToDpi(xOffset);
            target.Top = reference.Top + ScaleToDpi(yOffset);
        }
        /// <summary>
        /// (从右向左) 参考指定控件，在 X 方向上水平排列目标控件，并在 Y 方向上与指定控件的上边缘对齐
        /// </summary>
        protected void ArrangeControlXTopRtl(Control target, Control reference, int xOffset = 0)
        {
            target.Left = reference.Left - target.Width + ScaleToDpi(xOffset);
            target.Top = reference.Top;
        }

        /// <summary>
        /// 参考指定控件，在 X 方向上水平排列目标控件，与 <paramref name="reference1"/> 右边缘对齐，并在 Y 方向上与 <paramref name="reference2"/> 上边缘对齐。
        /// </summary>
        protected void ArrangeControlXRightTop(Control target, Control reference1, Control reference2, int xOffset = 0, int yOffset = 0)
        {
            target.Left = reference1.Right + ScaleToDpi(xOffset);
            target.Top = reference2.Top + ScaleToDpi(yOffset);
        }

        /// <summary>
        /// (从右向左) 参考指定控件，在 X 方向上水平排列目标控件，与 <paramref name="reference1"/> 右边缘对齐，并在 Y 方向上与 <paramref name="reference2"/> 上边缘对齐。
        /// </summary>
        protected void ArrangeControlXRightTopRtl(Control target, Control reference1, Control reference2, int xOffset = 0, int yOffset = 0)
        {
            target.Left = reference1.Right - target.Width + ScaleToDpi(xOffset);
            target.Top = reference2.Top + ScaleToDpi(yOffset);
        }

        /// <summary>
        /// 参考指定控件，在 X 方向上水平排列目标控件，与 <paramref name="reference1"/> 左边缘对齐，并在 Y 方向上与 <paramref name="reference2"/> 上边缘对齐。
        /// </summary>
        protected void ArrangeControlXLeftTop(Control target, Control reference1, Control reference2, int xOffset = 0, int yOffset = 0)
        {
            target.Left = reference1.Left + ScaleToDpi(xOffset);
            target.Top = reference2.Top + ScaleToDpi(yOffset);
        }

        /// <summary>
        /// (从右向左) 参考指定控件，在 X 方向上水平排列目标控件，与 <paramref name="reference1"/> 左边缘对齐，并在 Y 方向上与 <paramref name="reference2"/> 上边缘对齐。
        /// </summary>
        protected void ArrangeControlXLeftTopRtl(Control target, Control reference1, Control reference2, int xOffset = 0, int yOffset = 0)
        {
            target.Left = reference1.Left - target.Width + ScaleToDpi(xOffset);
            target.Top = reference2.Top + ScaleToDpi(yOffset);
        }

        /// <summary>
        /// 参考指定控件，在 Y 方向上竖直排列目标控件，并在 X 方向上与指定控件的左边缘对齐
        /// </summary>
        protected void ArrangeControlYLeft(Control target, Control reference, int xOffset = 0, int yOffset = 0)
        {
            target.Left = reference.Left + ScaleToDpi(xOffset);
            target.Top = reference.Bottom + ScaleToDpi(yOffset);
        }

        /// <summary>
        /// 参考指定控件，在 Y 方向上竖直排列目标控件，并在 X 方向上与指定控件的右边缘对齐
        /// </summary>
        protected void ArrangeControlYRight(Control target, Control reference, int xOffset = 0, int yOffset = 0)
        {
            target.Left = reference.Right - target.Width + ScaleToDpi(xOffset);
            target.Top = reference.Bottom + ScaleToDpi(yOffset);
        }

        protected void GroupBoxArrageFirst(Control target, int xOffset = 0, int yOffset = 0)
        {
            target.Left = 6 + ScaleToDpi(xOffset);
            target.Top = CurrentFontHeight + ScaleToDpi(yOffset);
        }

        protected void GroupBoxAutoAdjustHeight(PlainGroupBox groupBox, Control yLast, int yOffset = 0)
        {
            groupBox.Height = yLast.Bottom + ScaleToDpi(yOffset);
        }

        protected void GroupBoxAlignControlRight(PlainGroupBox groupBox, Control target, Control reference, int xOffset = 0, int yOffset = 0)
        {
            target.Left = groupBox.Width - target.Width + ScaleToDpi(xOffset);
            target.Top = reference.Top + ScaleToDpi(yOffset);
        }

        /// <summary>
        /// 将目标控件的左边缘在 X 方向上与参考控件的左边缘对齐
        /// </summary>
        protected void AlignControlLeft(Control target, Control reference, int xOffset = 0)
        {
            target.Left = reference.Left + ScaleToDpi(xOffset);
        }

        /// <summary>
        /// 将目标控件的上边缘在 Y 方向上与参考控件的上边缘对齐
        /// </summary>
        protected void AlignControlTop(Control target, Control reference, int yOffset = 0)
        {
            target.Top = reference.Top + ScaleToDpi(yOffset);
        }

        protected void AlignControlYRight(Control target, Control reference, int xOffset = 0)
        {
            target.Left = reference.Right - target.Width + ScaleToDpi(xOffset);
        }

        /// <summary>
        /// 将目标控件在 Y 方向上与参考控件居中
        /// </summary>
        protected void CenterControlY(Control target, Control reference, int yOffset = 0)
        {
            target.Top = reference.Top + (reference.Height - target.Height) / 2 + ScaleToDpi(yOffset);
        }

        /// <summary>
        /// 将目标控件在 X 方向上与参考控件保持紧凑
        /// </summary>
        protected void CompactControlX(Control target, Control reference)
        {
            target.Left = reference.Right;
        }

        /// <summary>
        /// 将目标控件在 Y 方向上与参考控件保持紧凑
        /// </summary>
        protected void CompactControlY(Control target, Control reference, int yOffset = 0)
        {
            target.Top = reference.Bottom + ScaleToDpi(yOffset);
        }

        /// <summary>
        /// 仅当窗体加载完成再执行指定的代码。
        /// </summary>
        protected void WhenLoaded(Action Method)
        {
            if (!IsLoading)
            {
                Method();
            }
        }

        protected void SetLocation(int x, int y)
        {
            SetBoundsCore(x, y, Width, Height, BoundsSpecified.Location);
        }

        /// <summary>
        /// 以父容器宽度为参考使 Label 单行内容达到一定长度时自动换行。
        /// </summary>
        /// <param name="Target">目标 Label</param>
        protected void SetLabelAutoWrap(Label Target)
        {
            SetLabelAutoWrap(Target, Target.Parent.Width - Target.Left);
        }

        protected void ShowBottonMenu(ContextMenu menu, object sender)
        {
            var target = (PlainButton)sender;
            menu.Show(target, new(0, target.Height));
        }

        protected void SetLabelAutoWrap(Label Target, int MaxWidth)
        {
            #region 来自网络
            /*
            
            Label 控件自动换行 参考:

            c# - Word wrap for a label in Windows Forms - Stack Overflow
            https://stackoverflow.com/a/3680595/21094697

            */
            Target.MaximumSize = new(MaxWidth, 0);
            Target.AutoSize = true;
            #endregion
        }

        protected void KeepOnScreen()
        {
            var ValidArea = GetCurrentScreenRect();
            bool b = false;

            if (Left < ValidArea.Left)
            {
                Left = ValidArea.Left;
                b = true;
            }

            if (Top < ValidArea.Top)
            {
                Top = ValidArea.Top;
                b = true;
            }

            if (Right > ValidArea.Right)
            {
                Left = ValidArea.Right - Width;
                b = true;
            }

            if (Bottom > ValidArea.Bottom)
            {
                Top = ValidArea.Bottom - Height;
                b = true;
            }

            if (Special && b)
            {
                LocationRefreshed?.Invoke();
            }
        }

        protected void AddParam(AppFormParam param)
        {
            Params |= param;
        }

        protected void MoveToScreenCenter(Rectangle screenRect)
        {
            SetLocation(screenRect.X + (screenRect.Width - Width) / 2, screenRect.Y + (screenRect.Height - Height) / 2);
        }

        private void AppLauncher_TrayMenuShowAllClicked()
        {
            if (!IsDisposed)
            {
                ReActivate();
                KeepOnScreen();
            }
        }

        private void MainForm_UniTopMostChanged()
        {
            TopMost = !IsDisposed && MainForm.UniTopMost;
        }
    }
}
