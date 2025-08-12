using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.UI.Controls;
using PlainCEETimer.UI.Dialogs;

namespace PlainCEETimer.UI
{
    public class MessageBoxHelper(AppForm parent = null)
    {
        private class MessageLevel
        {
            public readonly string Description;
            public readonly Bitmap Icon;
            public readonly SystemSound Sound;

            public static MessageLevel Info => field ??= new("提示 - 高考倒计时", ref InfoIcon, 76, SystemSounds.Asterisk);
            public static MessageLevel Warning => field ??= new("警告 - 高考倒计时", ref WarningIcon, 79, SystemSounds.Exclamation);
            public static MessageLevel Error => field ??= new("错误 - 高考倒计时", ref ErrorIcon, 93, SystemSounds.Hand);

            private static Bitmap InfoIcon;
            private static Bitmap WarningIcon;
            private static Bitmap ErrorIcon;

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
                icon ??= IconHelper.GetIcon("imageres.dll", iconIndex).ToBitmap();
                Icon = icon;
                Sound = sound;
            }
        }

        /// <summary>
        /// 获取不指定父窗体的消息框实例
        /// </summary>
        public static MessageBoxHelper Instance { get; } = new();

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
                return new AppMessageBox(parent, message, level.Description, autoClose, buttons, level.Sound, level.Icon).ShowCore();
            }
        }

        private string GetExMessage(string msg, Exception ex)
        {
            return ex == null ? msg : $"{msg}\n\n错误信息: \n{ex.Message}\n\n错误详情: \n{ex}";
        }
    }
}
