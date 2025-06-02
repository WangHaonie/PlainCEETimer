using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Controls
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

        private static float CurrentDpiRatio;
        private static bool IsHighDpi;

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
            OnLoad();
            AdjustUI();
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
                IsHighDpi = (CurrentDpiRatio = g.DpiX / 96F) > 1F;
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
        /// 用于计算并调整 UI 控件布局。该方法没有默认实现，可不调用 base.AdjustUI();
        /// </summary>
        protected virtual void AdjustUI() { }

        /// <summary>
        /// 在 AppForm 加载时触发。该方法没有默认实现，可不调用 base.OnLoad();
        /// </summary>
        protected virtual void OnLoad() { }

        /// <summary>
        /// 在 AppForm 已向用户显示时触发。该方法没有默认实现，可不调用 base.OnShown();
        /// </summary>
        protected virtual void OnShown() { }

        /// <summary>
        /// 在 AppForm 被关闭时触发。该方法默认返回 <see langword="false"/>，可不调用 base.OnClosing(CloseReason);
        /// </summary>
        /// <returns><see langword="true"/> 则取消关闭窗口, <see langword="false"/> 则允许关闭窗口</returns>
        protected virtual bool OnClosing(CloseReason closeReason) => false;

        /// <summary>
        /// 在 AppForm 关闭后触发。该方法没有默认实现，可不调用 base.OnClosed();
        /// </summary>
        protected virtual void OnClosed() { }

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

        /// <summary>
        /// 仅当在高 DPI 下才执行指定的代码。
        /// </summary>
        protected void WhenHighDpi(Action Method)
        {
            if (IsHighDpi)
            {
                Method();
            }
        }

        /// <summary>
        /// 在用户未保存更改并尝试关闭窗体时显示警告。同时防止直接关闭警告时也窗体会随之关闭。
        /// </summary>
        protected bool ShowUnsavedWarning(string WarningMsg, Func<bool> SaveChanges, ref bool flagUserChanged)
        {
            switch (MessageX.Warn(WarningMsg, Buttons: MessageButtons.YesNo))
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

        /// <summary>
        /// 为 ComboBox 绑定统一类型的 DataSource, DisplayMember 和 ValueMember。
        /// </summary>
        /// <param name="Target">目标 ComboBox 控件</param>
        /// <param name="Data">DataSource</param>
        protected void BindComboData(ComboBoxEx Target, ComboData[] Data)
        {
            Target.DataSource = Data;
            Target.DisplayMember = nameof(ComboData.Display);
            Target.ValueMember = nameof(ComboData.Value);
        }

        protected Rectangle GetCurrentScreenRect()
            => Special ? Screen.GetWorkingArea(this) : Screen.GetWorkingArea(Cursor.Position);

        protected void SetLocation(int x, int y)
        {
            SetBoundsCore(x, y, Width, Height, BoundsSpecified.Location);
        }

        /// <summary>
        /// 以父容器宽度为参考使 Label 单行内容达到一定长度时自动换行。
        /// </summary>
        /// <param name="Target">目标 Label</param>
        /// <param name="Parent">该 Label 所在的容器</param>
        protected void SetLabelAutoWrap(Label Target, Control Parent)
        {
            SetLabelAutoWrap(Target, Parent.Width - Target.Left);
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

        /// <summary>
        /// 将一个特殊控件 (通常位于窗体左下角) 与指定控件的左边缘对齐。
        /// </summary>
        /// <param name="Target">目标按钮</param>
        /// <param name="RightButton">右下角的按钮 (通常是确定或取消)</param>
        /// <param name="Reference">指定控件</param>
        protected void AlignControlsL(Control Target, Button RightButton, Control Reference)
        {
            Target.Left = Reference.Left;
            Target.Top = RightButton.Top;
        }

        /// <summary>
        /// 将目标控件与指定的控件的右边缘对齐。
        /// </summary>
        /// <param name="Target">目标控件</param>
        /// <param name="Reference">指定控件</param>
        protected void AlignControlsR(Control Target, Control Reference)
        {
            Target.Left = Reference.Left + Reference.Width - Target.Width;
        }

        /// <summary>
        /// 将一对按钮 (通常是确定和取消) 与某容器类控件的右边缘对齐。
        /// </summary>
        /// <param name="Btn1">按钮1</param>
        /// <param name="Btn2">按钮2</param>
        /// <param name="Container">容器类控件</param>
        protected void AlignControlsR(Button Btn1, Button Btn2, Control Container)
        {
            AlignControlsRCore(Btn1, Btn2, Container, Container.Height + ScaleToDpi(6));
        }

        /// <summary>
        /// 将一对按钮 (通常是确定和取消) 与指定控件的右边缘对齐。
        /// </summary>
        /// <param name="Btn1">按钮1</param>
        /// <param name="Btn2">按钮2</param>
        /// <param name="Reference">指定控件</param>
        protected void AlignControlsREx(Button Btn1, Button Btn2, Control Reference)
        {
            AlignControlsRCore(Btn1, Btn2, Reference, Btn2.Top);
        }

        /// <summary>
        /// 将目标控件与指定控件在水平方向上对齐。
        /// </summary>
        /// <param name="Target">目标控件</param>
        /// <param name="Reference">指定控件</param>
        /// <param name="Tweak">[可选] 微调</param>
        protected void AlignControlsX(Control Target, Control Reference, int Tweak = 0)
        {
            Target.Top = Reference.Top + Reference.Height / 2 - Target.Height / 2 + ScaleToDpi(Tweak);
        }

        /// <summary>
        /// 将一堆控件与指定控件在水平方向上对齐。
        /// </summary>
        /// <param name="Targets">控件</param>
        /// <param name="Reference">指定控件</param>
        /// <param name="Tweak">[可选] 微调</param>
        protected void AlignControlsX(Control[] Targets, Control Reference, int Tweak = 0)
        {
            foreach (var target in Targets)
            {
                AlignControlsX(target, Reference, Tweak);
            }
        }

        /// <summary>
        /// 使目标控件在水平方向上与指定控件变得更紧凑。
        /// </summary>
        /// <param name="Target">目标控件</param>
        /// <param name="Reference">指定控件</param>
        /// <param name="Tweak">[可选] 微调</param>
        protected void CompactControlsX(Control Target, Control Reference, int Tweak = 0)
        {
            Target.Left = Reference.Left + Reference.Width + ScaleToDpi(Tweak);
        }

        /// <summary>
        /// 使目标控件在垂直方向上与指定控件变得更紧凑。
        /// </summary>
        /// <param name="Target">目标控件</param>
        /// <param name="Reference">指定控件</param>
        /// <param name="Tweak">[可选] 微调</param>
        protected void CompactControlsY(Control Target, Control Reference, int Tweak = 0)
        {
            Target.Top = Reference.Top + Reference.Height + ScaleToDpi(Tweak);
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

        protected bool CheckParam(AppFormParam param) => (Params & param) == param;

        protected void AddParam(AppFormParam param) => Params |= param;

        protected int ScaleToDpi(int px) => (int)(px * CurrentDpiRatio);

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

        private void AlignControlsRCore(Button Btn1, Button Btn2, Control Main, int yTweak)
        {
            Btn2.Location = new(Main.Left + Main.Width - Btn2.Width, yTweak);
            Btn1.Location = new(Btn2.Left - Btn1.Width - ScaleToDpi(6), Btn2.Top);
        }
    }
}
