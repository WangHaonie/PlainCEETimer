using PlainCEETimer.Controls;
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

        public DialogResult Info(string Message, TabPage ParentTabPage = null, AppMessageBoxButtons Buttons = AppMessageBoxButtons.OK, bool AutoClose = false)
            => Popup(Message, MessageLevel.Info, ParentTabPage, Buttons, AutoClose);

        public DialogResult Warn(string Message, TabPage ParentTabPage = null, AppMessageBoxButtons Buttons = AppMessageBoxButtons.OK, bool AutoClose = false)
            => Popup(Message, MessageLevel.Warning, ParentTabPage, Buttons, AutoClose);

        public DialogResult Error(string Message, TabPage ParentTabPage = null, AppMessageBoxButtons Buttons = AppMessageBoxButtons.OK, bool AutoClose = false)
            => Popup(Message, MessageLevel.Error, ParentTabPage, Buttons, AutoClose);

        private DialogResult Popup(string Message, MessageLevel Level, TabPage ParentTabPage, AppMessageBoxButtons Buttons, bool AutoClose)
        {
            var (Title, AppMessageBoxIcon, Sound) = GetStuff(Level);

            if (Parent != null)
            {
                if (Parent.InvokeRequired)
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
                else
                {
                    return ShowPopup();
                }
            }
            else
            {
                return ShowPopup();
            }

            DialogResult ShowPopup()
            {
                var Mbox = new AppMessageBox(Sound, Buttons, AutoClose);

                if (Parent != null)
                {
                    Parent.ReActivate();
                    Parent.KeepOnScreen();
                }

                if (ParentTabPage != null)
                {
                    ((TabControl)ParentTabPage.Parent).SelectedTab = ParentTabPage;
                }

                return Mbox.ShowCore(Parent, Message, Title, AppMessageBoxIcon);
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

        private static Bitmap GetIcon(int Index)
        {
            NativeInterop.ExtractIconEx("imageres.dll", Index, out IntPtr hIcon, out _, 1);
            return Icon.FromHandle(hIcon).ToBitmap();
        }
    }
}
