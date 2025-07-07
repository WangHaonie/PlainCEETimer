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
        /// <summary>
        /// 获取不指定父窗体的消息框实例
        /// </summary>
        public static MessageBoxHelper Instance { get; } = new();

        private static readonly Bitmap InfoIcon;
        private static readonly Bitmap WarningIcon;
        private static readonly Bitmap ErrorIcon;

        static MessageBoxHelper()
        {
            InfoIcon = GetIcon(76);
            WarningIcon = GetIcon(79);
            ErrorIcon = GetIcon(93);
        }

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
            var (title, icon, sound) = GetStuff(level);

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
                return new AppMessageBox(message, title, autoClose, buttons, sound, icon).ShowCore(parent);
            }
        }

        private (string, Bitmap, SystemSound) GetStuff(MessageLevel level) => level switch
        {
            /*

            获取 imageres.dll 里的 Info、Warning、Error 图标的索引参考:

            Icons in imageres.dll
            https://renenyffenegger.ch/development/Windows/PowerShell/examples/WinAPI/ExtractIconEx/imageres.html


            播放与 MessageBox 同款音效参考:

            c# - Selecting sounds from Windows and playing them - Stack Overflow
            https://stackoverflow.com/a/5194223/21094697

            */

            MessageLevel.Warning => ("警告 - 高考倒计时", WarningIcon, SystemSounds.Exclamation),
            MessageLevel.Error => ("错误 - 高考倒计时", ErrorIcon, SystemSounds.Hand),
            _ => ("提示 - 高考倒计时", InfoIcon, SystemSounds.Asterisk)
        };

        private string GetExMessage(string msg, Exception ex)
        {
            return ex == null ? msg : $"{msg}\n\n错误信息: \n{ex.Message}\n\n错误详情: \n{ex}";
        }

        private static Bitmap GetIcon(int index)
        {
            return IconHelper.GetIcon("imageres.dll", index).ToBitmap();
        }
    }
}
