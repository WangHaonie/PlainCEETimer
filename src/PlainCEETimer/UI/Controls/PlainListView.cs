using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Linq;

namespace PlainCEETimer.UI.Controls;

/*

ListView 深色主题 参考：

win32-darkmode/win32-darkmode/ListViewUtil.h at master · ysc3839/win32-darkmode
https://github.com/ysc3839/win32-darkmode/blob/master/win32-darkmode/ListViewUtil.h

*/

public sealed class PlainListView : ListView, IThemeAware
{
    private sealed class SysHeader32NativeWindow : NativeWindow
    {
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM.SETCURSOR:
                case WM.CONTEXTMENU:
                    m.Result = new(1);
                    return;
                case NativeConstants.HDM_LAYOUT:
                    base.WndProc(ref m);
                    var lparam = m.LParam;
                    var prc = Marshal.ReadIntPtr(lparam, HDLAYOUT.prc);
                    var pwpos = Marshal.ReadIntPtr(lparam, HDLAYOUT.pwpos);
                    var height = Marshal.ReadInt32(pwpos, WINDOWPOS.cy);
                    height += (int)(height * 0.2F);
                    Marshal.WriteInt32(pwpos, WINDOWPOS.cy, height);
                    Marshal.WriteInt32(prc, RECT.top, height);
                    return;
            }

            base.WndProc(ref m);
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
    private bool UseDark;
    private bool invalidate;
    private ThemeHelper themeHelper;
    private SysHeader32NativeWindow hnw;
    private readonly ColumnHeader BlankColumn = new() { Text = "", Width = 0 };

    public PlainListView()
    {
        View = View.Details;
        FullRowSelect = true;
        HeaderStyle = ColumnHeaderStyle.Nonclickable;
        HideSelection = false;
        ShowItemToolTips = true;

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
        themeHelper ??= new(this);
        UpdateRowHeight();
        base.OnHandleCreated(e);
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        UpdateRowHeight();
        AutoAdjustWidth();
    }

    protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
    {
        e.NewWidth = Columns[e.ColumnIndex].Width;
        e.Cancel = true;
        base.OnColumnWidthChanging(e);
    }

    protected override void Dispose(bool disposing)
    {
        themeHelper.Destroy();
        base.Dispose(disposing);
    }

    protected override void WndProc(ref Message m)
    {
        switch (m.Msg)
        {
            case WM.NOTIFY:

                switch (Marshal.ReadInt32(m.LParam, NMHDR.code))
                {
                    case NativeConstants.NM_CUSTOMDRAW:

                        switch (Marshal.ReadInt32(m.LParam, NMCUSTOMDRAW.dwDrawStage))
                        {
                            case NativeConstants.CDDS_PREPAINT:
                                m.Result = new(NativeConstants.CDRF_NOTIFYITEMDRAW);
                                return;
                            case NativeConstants.CDDS_ITEMPREPAINT:
                                Win32UI.SetTextColor(Marshal.ReadIntPtr(m.LParam, NMCUSTOMDRAW.hdc),
                                    UseDark ? Colors.DarkForeListViewHeader : Colors.LightForeListViewHeader);
                                m.Result = new(NativeConstants.CDRF_DODEFAULT);
                                return;
                        }

                        break;

                    case NativeConstants.TTN_GETDISPINFOW:
                        /*

                        ToolTip 自动换行 参考：

                        How to Implement Multiline Tooltips - Win32 apps | Microsoft Learn
                        https://learn.microsoft.com/en-us/windows/win32/controls/implement-multiline-tooltips

                         */

                        base.WndProc(ref m);
                        Win32UI.SendMessage(Marshal.ReadIntPtr(m.LParam), NativeConstants.TTM_SETMAXTIPWIDTH, 0, Width);
                        return;
                }

                break;
        }

        base.WndProc(ref m);
    }

    private void UpdateRowHeight()
    {
        var fh = FontHeight;
        fh += (int)(fh * 0.38F);
        var old = SmallImageList;

        SmallImageList = new()
        {
            ColorDepth = ColorDepth.Depth32Bit,
            ImageSize = new(1, fh)
        };

        old.Destroy();
    }

    private void AutoAdjustWidth()
    {
        BeginUpdate();
        AutoAdjustColumnWidth();
        EndUpdate();
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        UseDark = useDark;
        ForeColor = useDark ? Colors.DarkForeText : SystemColors.WindowText;
        BackColor = useDark ? Colors.DarkBackText : SystemColors.Window;

        IntPtr hListView = Handle;
        IntPtr hHeader = Win32UI.SendMessage(hListView, NativeConstants.LVM_GETHEADER, 0, 0);
        IntPtr hToolTips = Win32UI.SendMessage(hListView, NativeConstants.LVM_GETTOOLTIPS, 0, 0);

        ThemeManager.EnableDarkModeForControl(hListView, useDark ? SystemStyle.ItemsViewDark : SystemStyle.ItemsView);
        ThemeManager.EnableDarkModeForControl(hHeader, useDark ? SystemStyle.ItemsViewDark : SystemStyle.ItemsView);
        ThemeManager.EnableDarkModeForControl(hToolTips, useDark ? SystemStyle.ExplorerDark : SystemStyle.Explorer);

        if (!invalidate)
        {
            Win32UI.SetTopMostWindow(hToolTips);
            hnw?.ReleaseHandle();
            hnw = new SysHeader32NativeWindow();
            hnw.AssignHandle(hHeader);

            if (SystemVersion.IsWindows11)
            {
                Win32UI.SetRoundCornerEx(hToolTips, true);
            }

            invalidate = true;
        }
    }
}
