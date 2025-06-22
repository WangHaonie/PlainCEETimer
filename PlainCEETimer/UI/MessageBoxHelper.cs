using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
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
        private readonly AppForm Parent = parent;

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

            if (Parent != null && Parent.InvokeRequired)
            {
                #region 来自网络
                /*

                在 Invoke 方法内部获取到 DialogResult 返回值 参考:

                c# - Return Ivoke message DialogResult - Stack Overflow
                https://stackoverflow.com/a/29256646/21094697

                */
                return (DialogResult)Parent.Invoke(ShowPopup); // 等效于 Func<DialogResult>
                #endregion
            }

            return ShowPopup();


            DialogResult ShowPopup()
            {
                Parent?.ReActivate();
                return new AppMessageBox(message, autoClose, buttons, sound, icon).ShowCore(Parent, title);
            }
        }

        private (string, Bitmap, SystemSound) GetStuff(MessageLevel Level) => Level switch
        {
            #region 来自网络
            /*

            获取 imageres.dll 里的 Info、Warning、Error 图标的索引参考:

            Icons in imageres.dll
            https://renenyffenegger.ch/development/Windows/PowerShell/examples/WinAPI/ExtractIconEx/imageres.html


            播放与 MessageBox 同款音效参考:

            c# - Selecting sounds from Windows and playing them - Stack Overflow
            https://stackoverflow.com/a/5194223/21094697

            */

            MessageLevel.Warning => (App.WarnMsg, WarningIcon, SystemSounds.Exclamation),
            MessageLevel.Error => (App.ErrMsg, ErrorIcon, SystemSounds.Hand),
            _ => (App.InfoMsg, InfoIcon, SystemSounds.Asterisk)

            #endregion
        };

        private string GetExMessage(string msg, Exception ex)
        {
            return ex == null ? msg : $"{msg}\n\n错误信息: \n{ex.Message}\n\n错误详情: \n{ex}";
        }

        private static Bitmap GetIcon(int Index)
        {
            return IconHelper.GetIcon("imageres.dll", Index).ToBitmap();
        }
    }
}
