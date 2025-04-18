﻿using PlainCEETimer.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using System;
using System.Drawing;
using System.Windows.Forms;

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
        private static readonly float CurrentDpiRatio;

        protected AppForm(AppFormParam param)
        {
            Params = param;
            Special = CheckParam(AppFormParam.Special);
            App.TrayMenuShowAllClicked += AppLauncher_TrayMenuShowAllClicked;
            App.UniTopMostStateChanged += AppLauncher_UniTopMostStateChanged;
            AppLauncher_UniTopMostStateChanged();
            MessageX = new(this);
        }

        static AppForm()
        {
            CurrentDpiRatio = NativeInterop.GetDpiForSystem() / 96F;
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
                Location = GetScreenCenter(GetCurrentScreenRect());
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
            OnClosing(e);
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
            if (ThemeManager.ShouldUseDarkMode)
            {
                if (!Special)
                {
                    ForeColor = ThemeManager.DarkFore;
                    BackColor = ThemeManager.DarkBack;
                }

                ThemeManager.FlushDarkWindow(Handle);
            }

            base.OnHandleCreated(e);
        }

        /// <summary>
        /// 用于计算并调整 UI 控件布局。该方法没有默认实现，可不调用 base.AdjustUI();
        /// </summary>
        protected virtual void AdjustUI()
        {

        }

        /// <summary>
        /// 在 AppForm 加载时触发。该方法没有默认实现，可不调用 base.OnLoad();
        /// </summary>
        protected virtual void OnLoad()
        {

        }

        /// <summary>
        /// 在 AppForm 已向用户显示时触发。该方法没有默认实现，可不调用 base.OnShown();
        /// </summary>
        protected virtual void OnShown()
        {

        }

        /// <summary>
        /// 在 AppForm 被关闭时触发。该方法没有默认实现，可不调用 base.OnClosing(FormClosingEventArgs);
        /// </summary>
        protected virtual void OnClosing(FormClosingEventArgs e)
        {

        }

        /// <summary>
        /// 在 AppForm 关闭后触发。该方法没有默认实现，可不调用 base.OnClosed();
        /// </summary>
        protected virtual void OnClosed()
        {

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

        /// <summary>
        /// 仅当在高 DPI 下才执行指定的代码。
        /// </summary>
        protected void WhenHighDpi(Action Method)
        {
            if (CurrentDpiRatio > 1F)
            {
                Method();
            }
        }

        /// <summary>
        /// 在用户未保存更改并尝试关闭窗体时显示警告。同时防止直接关闭警告时也窗体会随之关闭。
        /// </summary>
        /// <param name="WarningMsg">警告信息</param>
        /// <param name="e"><see cref="FormClosingEventArgs"/></param>
        /// <param name="SaveChanges">执行 保存更改 的代码</param>
        /// <param name="IgnoreChanges">执行 忽略更改 的代码</param>
        protected void ShowUnsavedWarning(string WarningMsg, FormClosingEventArgs e, Func<bool> SaveChanges, ref bool userChanged)
        {
            switch (MessageX.Warn(WarningMsg, Buttons: MessageButtons.YesNo))
            {
                case DialogResult.Yes:
                    e.Cancel = !SaveChanges();
                    break;
                case DialogResult.No:
                    userChanged = false;
                    Close();
                    break;
                default:
                    e.Cancel = true;
                    break;
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

        /// <summary>
        /// 以屏幕宽度为参考使 Label 单行内容达到一定长度时自动换行。
        /// </summary>
        /// <param name="Target">目标 Label 控件</param>
        /// <param name="FullWidth">[可选] 默认 false。true 则按屏幕宽度减10px作为最大长度，false 则屏幕宽度的3/4。</param>
        protected void SetLabelAutoWrap(Label Target, bool FullWidth = false)
        {
            var CurrentScreenWidth = GetCurrentScreenRect().Width;
            SetLabelAutoWrapCore(Target, new(FullWidth ? CurrentScreenWidth - 10 : (int)(CurrentScreenWidth * 0.75), 0));
        }

        /// <summary>
        /// 以父容器宽度为参考使 Label 单行内容达到一定长度时自动换行。
        /// </summary>
        /// <param name="Target">目标 Label</param>
        /// <param name="Parent">该 Label 所在的容器</param>
        protected void SetLabelAutoWrap(Label Target, Control Parent)
        {
            SetLabelAutoWrapCore(Target, new(Parent.Width - Target.Left, 0));
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
            AlignControlsRCore(Btn1, Btn2, Reference, Btn2.Location.Y);
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
            foreach (var Target in Targets)
            {
                AlignControlsX(Target, Reference, Tweak);
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

        protected void ShowContextMenu(Control Target, ContextMenu Menu, ContextMenuStrip Strip, bool IsClassic)
        {
            var Pos = new Point(0, Target.Height);

            if (IsClassic)
            {
                Menu.Show(Target, Pos);
            }
            else
            {
                Strip.Show(Target, Pos);
            }
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

        protected bool CheckParam(AppFormParam param)
        {
            return (Params & param) != 0;
        }

        protected void AddParam(AppFormParam param)
        {
            Params |= param;
        }

        protected int ScaleToDpi(int px)
        {
            return (int)(px * CurrentDpiRatio);
        }

        protected Point GetScreenCenter(Rectangle screenRect)
        {
            return new(screenRect.Left + screenRect.Width / 2 - Width / 2, screenRect.Top + screenRect.Height / 2 - Height / 2);
        }

        protected ContextMenu CreateNew(MenuItem[] Items)
            => new(Items);

        protected MenuItem AddItem(string Text)
            => new(Text);

        protected MenuItem AddItem(string Text, EventHandler OnClickHandler)
            => new(Text, OnClickHandler);

        protected MenuItem AddSubMenu(string Text, MenuItem[] Items)
            => new(Text, Items);

        protected MenuItem AddSeparator()
            => new("-");

        protected ContextMenu Merge(ContextMenu Target, ContextMenu Reference)
        {
            Target.MergeMenu(Reference);
            return Target;
        }

        protected ContextMenuStrip CreateNewStrip(ToolStripItem[] Items)
        {
            var Strip = new ContextMenuStrip();
            Strip.Items.AddRange(Items);
            return Strip;
        }

        protected ToolStripMenuItem AddStripItem(string Text)
            => new(Text);

        protected ToolStripMenuItem AddStripItem(string Text, EventHandler OnClickHandler)
            => new(Text, null, OnClickHandler);

        protected ToolStripMenuItem AddSubStrip(string Text, ToolStripItem[] SubItems)
        {
            var Item = new ToolStripMenuItem(Text);
            Item.DropDownItems.AddRange(SubItems);
            return Item;
        }

        protected ToolStripSeparator AddStripSeparator()
            => new();

        protected ContextMenuStrip MergeStrip(ContextMenuStrip Target, ToolStripItem[] Reference)
        {
            Target.Items.AddRange(Reference);
            return Target;
        }

        private void AppLauncher_TrayMenuShowAllClicked()
        {
            if (!IsDisposed)
            {
                ReActivate();
                KeepOnScreen();
            }
        }

        private void AppLauncher_UniTopMostStateChanged()
        {
            TopMost = !IsDisposed && !Special && MainForm.UniTopMost;
        }

        private void SetLabelAutoWrapCore(Label Target, Size NewSize)
        {
            #region 来自网络
            /*
            
            Label 控件自动换行 参考:

            c# - Word wrap for a label in Windows Forms - Stack Overflow
            https://stackoverflow.com/a/3680595/21094697

            */
            Target.MaximumSize = NewSize;
            Target.AutoSize = true;
            #endregion
        }

        private void AlignControlsRCore(Button Btn1, Button Btn2, Control Main, int yTweak)
        {
            Btn2.Location = new(Main.Location.X + Main.Width - Btn2.Width, yTweak);
            Btn1.Location = new(Btn2.Location.X - Btn1.Width - ScaleToDpi(6), Btn2.Location.Y);
        }

        private Rectangle GetCurrentScreenRect()
        {
            return Special ? Screen.GetWorkingArea(this) : Screen.GetWorkingArea(Cursor.Position);
        }
    }
}
