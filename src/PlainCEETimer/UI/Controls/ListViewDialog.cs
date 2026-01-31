using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.Modules.Linq;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public abstract class ListViewDialog<TData, TChildDialog> : AppDialog
    where TData : IListViewData<TData>
    where TChildDialog : AppDialog, IListViewChildDialog<TData>
{
    private class ListViewItemComparer<T> : IComparer
        where T : IListViewData<T>
    {
        int IComparer.Compare(object x, object y)
        {
            return ((T)((ListViewItem)x).Tag).CompareTo((T)((ListViewItem)y).Tag);
        }
    }

    /// <summary>
    /// 获取或设置要展示在列表中的数据。
    /// </summary>
    public TData[] Data { get; set; }

    /// <summary>
    /// 获取或设置要固定在列表中的数据。
    /// </summary>
    public TData[] FixedData
    {
        get
        {
            if (IsDisposed)
            {
                return fixedData;
            }

            if (FixedDataItemSet != null)
            {
                var count = FixedDataItemSet.Count;
                var data = new TData[count];
                using var e = FixedDataItemSet.GetEnumerator();

                for (int i = 0; e.MoveNext(); i++)
                {
                    data[i] = (TData)e.Current.Tag;
                }

                return data;
            }

            return null;
        }

        set
        {
            if (FixedDataItemSet == null && (HasFixedData = !value.IsNullOrEmpty()))
            {
                FixedDataItemSet = new(value.Length);
                fixedData = value;
            }
        }
    }

    protected virtual bool AllowExcludeItems { get; }

    protected sealed override AppFormParam Params => AppFormParam.AllControl;

    private bool HasFixedData;
    private TData[] fixedData;
    private ContextMenu ContextMenuMain;
    private MenuItem ContextDuplicate;
    private MenuItem ContextEdit;
    private MenuItem ContextDelete;
    private MenuItem ContextExclude;
    private MenuItem ContextInclude;
    private MenuItem ContextSelectAll;
    private PlainButton ButtonOperation;
    private HashSet<ListViewItem> FixedDataItemSet;
    private readonly string DefaultItemDesc;
    private readonly string MsgDelete;
    private readonly string MsgAddDup;
    private readonly string MsgEditDup;
    private readonly bool UseDark = ThemeManager.ShouldUseDarkMode;
    private readonly ListViewItemSet<TData> ItemSet = new();
    private readonly ListView.ListViewItemCollection Items;
    private readonly ListViewGroupCollection Groups;

    private readonly PlainListView ListViewMain = new()
    {
        Location = new(3, 3),
        UseCompatibleStateImageBehavior = false
    };

    protected ListViewDialog(int listViewWidth, string[] headers, string[] groups, string itemDesc = null)
    {
        var desc = itemDesc ?? "项";
        ListViewMain.Headers = headers;
        ListViewMain.Size = new Size(ScaleToDpi(listViewWidth), ScaleToDpi(218));

        if (groups != null && groups.Length != 0)
        {
            ListViewMain.ShowGroups = true;
            Groups = ListViewMain.Groups;

            foreach (var group in groups)
            {
                Groups.Add(new(group));
            }
        }

        Items = ListViewMain.Items;
        DefaultItemDesc = "默认" + desc;
        MsgDelete = $"确认删除所选{desc}吗？此操作将不可撤销！";
        MsgAddDup = $"列表中已存在该{desc}，请重新添加！";
        MsgEditDup = $"列表中已存在该{desc}，请重新编辑！";
    }

    protected sealed override void OnInitializing()
    {
        this.AddControls(b =>
        [
            ListViewMain,

            ButtonOperation = b.Button("操作(&O) ▼").AttachContextMenu(b =>
            [
                b.Item("添加(&A)", (_, _) =>
                {
                    var dialog = GetChildDialog();

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        AddItemSafe(dialog.Data);
                    }
                }).Default(),

                b.Separator(),
                ContextDuplicate = b.Item("重复(&C)", ContextDuplicate_Click),
                ContextEdit = b.Item("编辑(&E)", ContextEdit_Click),
                ContextDelete = b.Item("删除(&D)", ContextDelete_Click),
                b.Separator(),
                ContextSelectAll = b.Item("全选(&Q)", ContextSelectAll_Click)
            ], (_, _) =>
            {
                ListViewMain.GetSelection(out _, out var item, out var total);

                var atMost1 = total == 1;
                var selected = total != 0;

                ContextDelete.Enabled = selected;
                ContextDuplicate.Enabled = atMost1;
                ContextEdit.Enabled = atMost1;
                ContextSelectAll.Enabled = Items.Count != 0;

                if (AllowExcludeItems)
                {
                    var excluded = selected && ((TData)item.Tag).Excluded;
                    ContextExclude.Enabled = selected && (!atMost1 || !excluded);
                    ContextInclude.Enabled = selected && (!atMost1 || excluded);
                }

            }, out ContextMenuMain)
        ]);

        if (AllowExcludeItems)
        {
            ContextMenuMain.AddItems(b =>
            [
                b.Separator(),
                ContextExclude = b.Item("排除(&X)", ContextExclude_Click),
                ContextInclude = b.Item("包括(&I)", ContextInclude_Click)
            ], ContextSelectAll.Index - 1);
        }

        ListViewMain.ContextMenu = ContextMenuMain;
        base.OnInitializing();
    }

    protected sealed override void RunLayout(bool isHighDpi)
    {
        ArrangeCommonButtonsR(ButtonA, ButtonB, ListViewMain, 1, 3);
        ArrangeControlYL(ButtonOperation, ListViewMain, -1, 3);
        InitWindowSize(ButtonB, 3, 3);
        ListViewMain.ColumnMaxWidth = ScaleToDpi(500);
    }

    protected sealed override void OnLoad()
    {
        ListViewMain.BeginUpdate();

        var hasData = !Data.IsNullOrEmpty();

        if (HasFixedData)
        {
            for (int i = 0; i < fixedData.Length; i++)
            {
                AddItemCore(fixedData[i]);
            }
        }

        if (hasData)
        {
            foreach (var d in Data)
            {
                AddItemCore(d);
            }
        }

        if (HasFixedData || hasData)
        {
            Items[0].EnsureVisible();
        }

        ListViewMain.AutoAdjustColumnWidth();

        ListViewMain.EndUpdate();
        ListViewMain.MouseDoubleClick += ContextEdit_Click;
        ListViewMain.ListViewItemSorter = new ListViewItemComparer<TData>();
    }

    protected sealed override void OnKeyDown(KeyEventArgs e)
    {
        var handled = true;

        switch (e.Control, e.KeyCode)
        {
            case (true, Keys.A):
                ContextSelectAll_Click(null, null);
                break;
            case (true, Keys.C):
                ContextDuplicate_Click(null, null);
                break;
            case (true, Keys.X):
                ContextExclude_Click(null, null);
                break;
            case (true, Keys.I):
                ContextInclude_Click(null, null);
                break;
            case (false, Keys.Delete):
                ContextDelete_Click(null, null);
                break;
            default:
                handled = false;
                break;
        }

        e.Handled = handled;
        base.OnKeyDown(e);
    }

    protected sealed override bool OnClickButtonA()
    {
        TData[] data = [];
        var total = Items.Count;
        var length = total - (HasFixedData ? fixedData.Length : 0);

        if (length != 0)
        {
            data = new TData[length];

            for (int i = 0, j = 0; i < total; i++)
            {
                var item = Items[i];

                if (!IsDefault(item) && j < length)
                {
                    data[j] = (TData)item.Tag;
                    j++;
                }
            }
        }

        Data = data;
        fixedData = FixedData;
        return base.OnClickButtonA();
    }

    protected abstract int GetGroupIndex(TData data);

    protected abstract ListViewItem GetListViewItem(TData data);

    protected abstract IListViewChildDialog<TData> GetChildDialog(TData data = default);

    private void ContextDuplicate_Click(object sender, EventArgs e)
    {
        ListViewMain.GetSelection(out _, out var first, out var total);

        if (total == 1)
        {
            if (!IsDefault(first))
            {
                var dialog = GetChildDialog((TData)first.Tag);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    AddItemSafe(dialog.Data);
                }
            }
        }
    }

    private void ContextEdit_Click(object sender, EventArgs e)
    {
        ListViewMain.GetSelection(out _, out var first, out var total);

        if (total != 0)
        {
            var data = (TData)first.Tag;
            var dialog = GetChildDialog(data);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                EditItemSafe(first, dialog.Data, data);
            }
        }
    }

    private void ContextDelete_Click(object sender, EventArgs e)
    {
        ListViewMain.GetSelection(out var items, out _, out var total);

        if (total != 0 &&
            MessageX.Warn(MsgDelete, MessageButtons.YesNo) == DialogResult.Yes)
        {
            ListViewMain.BeginUpdate();

            if (!HasFixedData && total == Items.Count)
            {
                RemoveAllItems();
            }
            else
            {
                ListViewItem item;

                for (int i = total - 1; i >= 0; i--)
                {
                    item = items[i];
                    RemoveItem(item, (TData)item.Tag);
                }
            }

            ListViewMain.AutoAdjustColumnWidth();
            ListViewMain.EndUpdate();
            UserChanged();
        }
    }

    private void ContextSelectAll_Click(object sender, EventArgs e)
    {
        ListViewMain.BeginUpdate();
        ListViewMain.SelectAll(true);
        ListViewMain.EndUpdate();
    }

    private void ContextExclude_Click(object sender, EventArgs e)
    {
        if (AllowExcludeItems)
        {
            ExcludeSelectedItems();
        }
    }

    private void ContextInclude_Click(object sender, EventArgs e)
    {
        if (AllowExcludeItems)
        {
            ExcludeSelectedItems(false);
        }
    }

    private void AddItemSafe(TData data)
    {
        if (ItemSet.CanAdd(data))
        {
            AddItem(data);
        }
        else
        {
            MessageX.Error(MsgAddDup);

            var dialog = GetChildDialog(data);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AddItemSafe(dialog.Data);
            }
        }
    }

    private void EditItemSafe(ListViewItem item, TData newData, TData oldData)
    {
        var flag = ItemSet.CanEdit(newData, item);

        if (flag != null)
        {
            if ((bool)flag)
            {
                EditItem(item, newData, oldData, HasFixedData && FixedDataItemSet.Remove(item), false);
            }
            else
            {
                MessageX.Error(MsgEditDup);

                var dialog = GetChildDialog(newData);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    EditItemSafe(item, dialog.Data, oldData);
                }
            }
        }
    }

    private void EditItem(ListViewItem item, TData newData, TData oldData, bool isDefault, bool reverseEx)
    {
        newData.Excluded = reverseEx ^ oldData.Excluded;
        RemoveItem(item, oldData, true);
        AddItem(newData);
    }

    private void AddItem(TData data)
    {
        ListViewMain.BeginUpdate();
        ListViewMain.SelectAll(false);
        AddItemCore(data, true);
        ListViewMain.AutoAdjustColumnWidth();
        ListViewMain.EndUpdate();
        UserChanged();
    }

    private void AddItemCore(TData data, bool isSelected = false)
    {
        var item = GetListViewItem(data);
        var isDefault = data.Default;
        item.Group = Groups[GetGroupIndex(data)];
        item.Tag = data;
        item.Selected = isSelected;
        item.Focused = isSelected;
        Items.Add(item);
        ItemSet.Add(data, item);

        if (isSelected)
        {
            item.EnsureVisible();
        }

        if (isDefault)
        {
            item.Text = DefaultItemDesc;
            item.ForeColor = UseDark ? Colors.DarkForeListViewDefaultItem : Colors.LightForeListViewDefaultItem;
            FixedDataItemSet.Add(item);
        }

        if (AllowExcludeItems && data.Excluded)
        {
            item.ForeColor = UseDark ? Colors.DarkForeTextDisabled : Colors.LightForeTextDisabled;
        }
    }

    private void RemoveItem(ListViewItem item, TData data, bool isEdit = false)
    {
        if (isEdit || !IsDefault(item))
        {
            Items.Remove(item);
            ItemSet.Remove(data);
        }
    }

    private void ExcludeSelectedItems(bool exclude = true)
    {
        ListViewMain.GetSelection(out var items, out _, out var total);

        if (total != 0)
        {
            var changed = false;
            ListViewMain.BeginUpdate();

            for (int i = 0; i < total; i++)
            {
                var item = items[i];
                var data = (TData)item.Tag;

                if (data.Excluded != exclude)
                {
                    EditItem(item, data.Copy(), data, false, true);
                    changed = true;
                }
            }

            ListViewMain.EndUpdate();

            if (changed)
            {
                UserChanged();
            }
        }
    }

    private void RemoveAllItems()
    {
        Items.Clear();
        ItemSet.Clear();
    }

    private bool IsDefault(ListViewItem item)
    {
        return HasFixedData && FixedDataItemSet.Contains(item);
    }
}
