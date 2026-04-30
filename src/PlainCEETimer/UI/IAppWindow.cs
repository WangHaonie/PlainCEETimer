using System;
using System.Windows.Forms;
using PlainCEETimer.UI.Core;

namespace PlainCEETimer.UI;

public interface IAppWindow : IHasContextMenu, IWin32Window
{
    bool InvokeRequired { get; }

    IDialogService MessageX { get; }

    void ReActivate();

    object Invoke(Delegate method);

    IAsyncResult BeginInvoke(Delegate method);
}