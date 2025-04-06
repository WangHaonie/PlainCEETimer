using PlainCEETimer.Controls;
using PlainCEETimer.Dialogs;
using PlainCEETimer.Interop;
using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public class MessageBoxHelper(AppForm parent = null)
    {
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

        public DialogResult Info(string Message, AppMessageBoxButtons Buttons = AppMessageBoxButtons.OK, bool AutoClose = false)
            => Popup(Message, MessageLevel.Info, Buttons, AutoClose);

        public DialogResult Warn(string Message, AppMessageBoxButtons Buttons = AppMessageBoxButtons.OK, bool AutoClose = false)
            => Popup(Message, MessageLevel.Warning, Buttons, AutoClose);

        public DialogResult Error(string Message, Exception Ex = null, AppMessageBoxButtons Buttons = AppMessageBoxButtons.OK, bool AutoClose = false)
            => Popup(GetExMessage(Message, Ex), MessageLevel.Error, Buttons, AutoClose);

        private DialogResult Popup(string Message, MessageLevel Level, AppMessageBoxButtons Buttons, bool AutoClose)
        {
            var (Title, AppMessageBoxIcon, Sound) = GetStuff(Level);

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
                return new AppMessageBox(Sound, Buttons, AutoClose).ShowCore(Parent, Message, Title, AppMessageBoxIcon);
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

        private string GetExMessage(string Message, Exception ex)
        {
            if (ex == null)
            {
                return Message;
            }

            return $"{Message}\n\n错误信息: \n{ex.Message}\n\n错误详情: \n{ex}";
        }

        private static Bitmap GetIcon(int Index)
        {
            NativeInterop.ExtractIconEx("imageres.dll", Index, out IntPtr hIcon, out _, 1);
            return Icon.FromHandle(hIcon).ToBitmap();
        }
    }
}
