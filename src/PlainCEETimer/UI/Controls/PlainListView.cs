using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Linq;

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
            const int WM_CONTEXTMENU = 0x007B;

            if (m.Msg is WM_SETCURSOR or WM_CONTEXTMENU)
            {
                m.Result = new(1);
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }

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

    public int ColumnMaxWidth
    {
        get => columnMaxWidth;
        set => columnMaxWidth = value;
    }

    private int columnMaxWidth;
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

    public new void EndUpdate()
    {
        base.EndUpdate();
        Focus();
    }

    public void AutoAdjustColumnWidth()
    {
        Columns.Add(BlankColumn);

        foreach (ColumnHeader column in Columns)
        {
            column.Width = -2;

            if (columnMaxWidth > 0 && column.Width > columnMaxWidth)
            {
                column.Width = columnMaxWidth;
            }
        }

        Columns.Remove(BlankColumn);
    }

    public void GetSelection(out ListViewItem[] items, out ListViewItem first, out int total)
    {
        items = null;
        first = null;
        total = -1;
        var selected = SelectedItems;

        if (selected != null && (total = selected.Count) != 0)
        {
            items = selected.ToArray<ListViewItem>();
            first = items[0];
        }
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

        const int TTN_FIRST = -520;
        const int TTN_GETDISPINFOW = TTN_FIRST - 10;
        const int WM_USER = 0x0400;
        const int TTM_SETMAXTIPWIDTH = WM_USER + 24;

        const int NMHDR_code = 16;
        const int NMCUSTOMDRAW_dwDrawStage = 24;
        const int NMCUSTOMDRAW_hdc = 32;

        if (m.Msg == WM_NOTIFY)
        {
            switch (Marshal.ReadInt32(m.LParam, NMHDR_code))
            {
                case NM_CUSTOMDRAW:
                    switch (Marshal.ReadInt32(m.LParam, NMCUSTOMDRAW_dwDrawStage))
                    {
                        case CDDS_PREPAINT:
                            m.Result = new(CDRF_NOTIFYITEMDRAW);
                            return;
                        case CDDS_ITEMPREPAINT:
                            Win32UI.SetTextColor(Marshal.ReadIntPtr(m.LParam, NMCUSTOMDRAW_hdc),
                                UseDark ? Colors.DarkForeListViewHeader : Colors.LightForeListViewHeader);
                            m.Result = new(CDRF_DODEFAULT);
                            return;
                    }
                    break;

                case TTN_GETDISPINFOW:
                    /*
                    
                    ToolTip 自动换行 参考：

                    How to Implement Multiline Tooltips - Win32 apps | Microsoft Learn
                    https://learn.microsoft.com/en-us/windows/win32/controls/implement-multiline-tooltips

                     */

                    base.WndProc(ref m);
                    Win32UI.SendMessage(Marshal.ReadIntPtr(m.LParam), TTM_SETMAXTIPWIDTH, 0, Width);
                    return;
            }
        }

        base.WndProc(ref m);
    }
}
