using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI;

public class AppMessageBox(AppForm parent = null)
{
    private sealed class MessageBox(AppForm owner, MessageLevel level, string message, MessageButtons buttons, bool autoClose) : AppDialog
    {
        protected override AppFormParam Params
        {
            get
            {
                var param = AppFormParam.KeyPreview | AppFormParam.OnEscClosing;

                if (owner == null)
                {
                    param |= AppFormParam.CenterScreen;
                }

                return param;
            }
        }

        private DialogResult Result;
        private PlainLabel LabelMessage;
        private PictureBox ImageIcon;

        protected override void OnInitializing()
        {
            Text = level.Description;

            this.AddControls(b =>
            [
                ImageIcon = b.Image(level.Icon),
                LabelMessage = b.Label(message).With(c => SetLabelAutoWrap(c, (int)(GetCurrentScreenRect().Width * 0.75)))
            ]);

            base.OnInitializing();
            ButtonA.Enabled = true;
        }

        protected override void RunLayout(bool isHighDpi)
        {
            ArrangeControlXT(LabelMessage, ImageIcon, 2);
            ArrangeCommonButtonsR(ButtonA, ButtonB, LabelMessage, -3, 3);

            if (ButtonA.Left < ImageIcon.Right)
            {
                AlignControlXL(ButtonA, LabelMessage, 3);
                ArrangeControlXT(ButtonB, ButtonA, 3);
            }

            if (ButtonA.Top < ImageIcon.Bottom)
            {
                CompactControlY(ButtonA, ImageIcon);
                ArrangeControlXT(ButtonB, ButtonA, 3);
            }
        }

        protected override void OnLoad()
        {
            switch (buttons)
            {
                case MessageButtons.YesNo:
                    ButtonA.Text = "是(&Y)";
                    ButtonB.Text = "否(&N)";
                    break;
                case MessageButtons.OK:
                    ButtonA.Visible = ButtonA.Enabled = false;
                    ButtonB.Text = "确定(&O)";
                    break;
            }
        }

        protected override void OnShown()
        {
            level.Sound.Play();

            if (autoClose)
            {
                3200.AsDelay(Close, this);
            }
        }

        protected override bool OnClickButtonA()
        {
            Result = buttons == MessageButtons.YesNo ? DialogResult.Yes : DialogResult.None;
            Close();
            return true;
        }

        protected override void OnClickButtonB()
        {
            Result = buttons == MessageButtons.YesNo ? DialogResult.No : DialogResult.OK;
            Close();
        }

        public DialogResult ShowCore()
        {
            ShowDialog(owner);
            return Result;
        }
    }

    private struct MessageLevel
    {
        public static MessageLevel Info => field.init ? field : field = new("提示 - 高考倒计时", ref InfoIcon, 76, SystemSounds.Asterisk);
        public static MessageLevel Warning => field.init ? field : field = new("警告 - 高考倒计时", ref WarningIcon, 79, SystemSounds.Exclamation);
        public static MessageLevel Error => field.init ? field : field = new("错误 - 高考倒计时", ref ErrorIcon, 93, SystemSounds.Hand);

        private static Bitmap InfoIcon;
        private static Bitmap WarningIcon;
        private static Bitmap ErrorIcon;

        public readonly string Description;
        public readonly Bitmap Icon;
        public readonly SystemSound Sound;

        private readonly bool init;

        /*

        获取 imageres.dll 里的 Info、Warning、Error 图标的索引参考:

        Icons in imageres.dll
        https://renenyffenegger.ch/development/Windows/PowerShell/examples/WinAPI/ExtractIconEx/imageres.html


        播放与 MessageBox 同款音效参考:

        c# - Selecting sounds from Windows and playing them - Stack Overflow
        https://stackoverflow.com/a/5194223/21094697

        */

        private MessageLevel(string description, ref Bitmap icon, int iconIndex, SystemSound sound)
        {
            Description = description;
            icon ??= HICON.FromFile("imageres.dll", iconIndex).ToIcon().ToBitmap();
            Icon = icon;
            Sound = sound;
            init = true;
        }
    }

    /// <summary>
    /// 获取不指定父窗体的消息框实例
    /// </summary>
    public static AppMessageBox Instance { get; } = new();

    public DialogResult Info(string message, MessageButtons buttons = MessageButtons.OK, bool autoClose = false)
    {
        return Popup(message, MessageLevel.Info, buttons, autoClose);
    }

    public DialogResult Warn(string message, MessageButtons buttons = MessageButtons.OK, bool autoClose = false)
    {
        return Popup(message, MessageLevel.Warning, buttons, autoClose);
    }

    public DialogResult Error(string message, Exception ex = null, MessageButtons buttons = MessageButtons.OK, bool autoClose = false)
    {
        return Popup(GetExMessage(message, ex), MessageLevel.Error, buttons, autoClose);
    }

    private DialogResult Popup(string message, MessageLevel level, MessageButtons buttons, bool autoClose)
    {
        if (parent != null && parent.InvokeRequired)
        {
            /*

            在 Invoke 方法内部获取到 DialogResult 返回值 参考:

            c# - Return Ivoke message DialogResult - Stack Overflow
            https://stackoverflow.com/a/29256646/21094697

            */
            return (DialogResult)parent.Invoke(ShowPopup); // 等效于 Func<DialogResult>
        }

        return ShowPopup();


        DialogResult ShowPopup()
        {
            parent?.ReActivate();
            return new MessageBox(parent, level, message, buttons, autoClose).ShowCore();
        }
    }

    private string GetExMessage(string msg, Exception ex)
    {
        return ex == null ? msg : $"{msg}\n\n错误信息: \n{ex.Message}\n\n错误详情: \n{ex}";
    }
}
