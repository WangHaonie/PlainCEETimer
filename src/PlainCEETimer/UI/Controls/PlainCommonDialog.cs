using System;
using System.Windows.Forms;
using PlainCEETimer.Interop;

namespace PlainCEETimer.UI.Controls;

public abstract class PlainCommonDialog : CommonDialog
{
    public string Text { get; set; }
    public AppForm Parent { get; set; }

    protected WNDPROC DialogHook => hook;

    private CommonDialogHelper Helper;
    private WNDPROC hook;

    public virtual DialogResult Show()
    {
        return ShowDialog(Parent);
    }

    protected abstract BOOL RunDialog(HWND hWndOwner);

    protected sealed override bool RunDialog(IntPtr hwndOwner)
    {
        try
        {
            hook = HookProc;
            Helper = new(this, Parent, Text, base.HookProc);
            return RunDialog((HWND)hwndOwner);
        }
        catch
        {
            return false;
        }
    }

    protected sealed override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
    {
        return Helper.HookProc(hWnd, msg, wparam, lparam);
    }

    public sealed override void Reset()
    {

    }
}
