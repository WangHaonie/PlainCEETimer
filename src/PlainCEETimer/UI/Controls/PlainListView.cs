using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI.Controls;

/*

ListView 深色主题 参考：

win32-darkmode/win32-darkmode/ListViewUtil.h at master · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/blob/master/win32-darkmode/ListViewUtil.h

*/

public sealed class PlainListView : ListView
{
    private sealed class SysHeader32NativeWindow : NativeWindow
    {
        public SysHeader32NativeWindow(IntPtr hHeader)
        {
            AssignHandle(hHeader);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SETCURSOR = 0x0020;

            if (m.Msg == WM_SETCURSOR)
            {
                m.Result = new(1);
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }

    public int SelectedItemsCount => SelectedItems.Count;

    public ListViewItem SelectedItem => SelectedItems[0];

    public string[] Headers
    {
        get;
        set
        {
            if (value != null && value.Length != 0)
            {
                Columns.Clear();

                foreach (var title in value)
                {
                    Columns.Add(new ColumnHeader() { Text = title });
                }
            }

            field = value;
        }
    }

    private readonly ColumnHeader BlankColumn = new() { Text = "", Width = 0 };
    private static readonly bool UseDark = ThemeManager.ShouldUseDarkMode;

    public PlainListView()
    {
        View = View.Details;
        FullRowSelect = true;
        HeaderStyle = ColumnHeaderStyle.Nonclickable;
        HideSelection = false;
        ShowItemToolTips = true;

        if (UseDark)
        {
            ForeColor = Colors.DarkForeText;
            BackColor = Colors.DarkBackText;
        }

        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
    }

    public void SelectAll(bool selected)
    {
        if (Items.Count != 0)
        {
            Win32UI.ListViewSelectAllItems(Handle, selected);
        }
    }

    public void Suspend(Action action)
    {
        BeginUpdate();
        action();
        EndUpdate();
        Focus();
    }

    public void AutoAdjustColumnWidth()
    {
        Columns.Add(BlankColumn);

        foreach (ColumnHeader column in Columns)
        {
            column.Width = -2;
        }

        Columns.Remove(BlankColumn);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        const int LVM_FIRST = 0x1000;
        const int LVM_GETTOOLTIPS = LVM_FIRST + 78;
        const int LVM_GETHEADER = LVM_FIRST + 31;

        IntPtr hListView = Handle;
        IntPtr hHeader = Win32UI.SendMessage(hListView, LVM_GETHEADER, 0, 0);
        IntPtr hToolTips = Win32UI.SendMessage(hListView, LVM_GETTOOLTIPS, 0, 0);
        new SysHeader32NativeWindow(hHeader);

        var LVstyle = NativeStyle.ItemsView;
        var TTstyle = NativeStyle.Explorer;

        if (UseDark)
        {
            LVstyle = NativeStyle.ItemsViewDark;
            TTstyle = NativeStyle.ExplorerDark;
        }

        ThemeManager.EnableDarkModeForControl(hListView, LVstyle);
        ThemeManager.EnableDarkModeForControl(hHeader, LVstyle);
        ThemeManager.EnableDarkModeForControl(hToolTips, TTstyle);
        Win32UI.SetTopMostWindow(hToolTips);

        if (SystemVersion.IsWindows11)
        {
            Win32UI.SetRoundCornerEx(hToolTips, true);
        }

        base.OnHandleCreated(e);
    }

    protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
    {
        e.NewWidth = Columns[e.ColumnIndex].Width;
        e.Cancel = true;
        base.OnColumnWidthChanging(e);
    }

    protected override void WndProc(ref Message m)
    {
        const int WM_NOTIFY = 0x004E;
        const int NM_CUSTOMDRAW = -12;
        const int CDDS_PREPAINT = 0x00000001;
        const int CDRF_NOTIFYITEMDRAW = 0x00000020;
        const int CDDS_ITEMPREPAINT = 0x00010000 | CDDS_PREPAINT;
        const int CDRF_DODEFAULT = 0x00000000;

        if (m.Msg == WM_NOTIFY && Marshal.ReadInt32(m.LParam, 16 /* NMHDR.code */) == NM_CUSTOMDRAW)
        {
            switch (Marshal.ReadInt32(m.LParam, 24 /* NMCUSTOMDRAW.dwDrawStage */))
            {
                case CDDS_PREPAINT:
                    m.Result = new(CDRF_NOTIFYITEMDRAW);
                    return;
                case CDDS_ITEMPREPAINT:
                    Win32UI.SetTextColor(Marshal.ReadIntPtr(m.LParam, 32 /* NMCUSTOMDRAW.hdc */),
                        UseDark ? Colors.DarkForeListViewHeader : Colors.LightForeListViewHeader);
                    m.Result = new(CDRF_DODEFAULT);
                    return;
            }
        }

        base.WndProc(ref m);
    }
}
