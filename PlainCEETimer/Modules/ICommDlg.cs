using System;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public interface ICommDlg
    {
        /// <summary>
        /// 表示自定义 <see cref="CommonDialog"/> 的回调方法。应当在该方法中返回基类 <see cref="CommonDialog.HookProc(IntPtr, int, IntPtr, IntPtr)"/> 方法。
        /// </summary>
        IntPtr HookProcCallBack(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 获取或设置该 <see cref="CommonDialog"/> 的窗口标题。
        /// </summary>
        string DialogTitle { get; }

        /// <summary>
        /// 定义该 <see cref="CommonDialog"/> 的衍生类型。
        /// </summary>
        /// <returns></returns>
        CommDlg DlgType { get; }
    }
}
