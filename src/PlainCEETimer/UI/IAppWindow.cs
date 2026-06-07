using System;
using System.Windows.Forms;
using PlainCEETimer.UI.Core;

namespace PlainCEETimer.UI;

public interface IAppWindow : IHasContextMenu, IWin32Window, IThemeAware
{
    bool InvokeRequired { get; }

    IDialogService MessageX { get; }

    void ReActivate();

    object Invoke(Delegate method, params object[] args);

    IAsyncResult BeginInvoke(Delegate method, params object[] args);
}