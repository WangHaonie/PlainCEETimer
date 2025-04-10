using System;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public interface ICommonDialog
    {
        /// <summary>
        /// 获取或设置该 <see cref="CommonDialog"/> 的窗口标题。
        /// </summary>
        string DialogTitle { get; }

        /// <summary>
        /// 定义该 <see cref="CommonDialog"/> 的衍生类型。
        /// </summary>
        /// <returns></returns>
        CommonDialogKind DialogKind { get; }

        /// <summary>
        /// 表示定义在 <see cref="CommonDialog"/> 中的默认回调方法。应当在此方法中返回基类 <see cref="CommonDialog.HookProc(IntPtr, int, IntPtr, IntPtr)"/> 方法。
        /// </summary>
        IntPtr BaseHookProc(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
    }
}
