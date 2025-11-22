using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public abstract class ListViewDialog<TData, TSubDialog> : AppDialog
    where TData : IListViewData<TData>
    where TSubDialog : AppDialog, IListViewSubDialog<TData>
{
    private class ListViewItemComparer<T> : IComparer
        where T : IListViewData<T>
    {
        int IComparer.Compare(object x, object y)
        {
            return ((T)((ListViewItem)x).Tag).CompareTo((T)((ListViewItem)y).Tag);
        }
    }

    public TData[] Data { get; set; }

    protected virtual TData[] DefaultData { get; }

    protected sealed override AppFormParam Params => AppFormParam.AllControl;

    private bool HasDefaultData;
    private ContextMenu ContextMenuMain;
    private MenuItem ContextDuplicate;
    private MenuItem ContextEdit;
    private MenuItem ContextDelete;
    private MenuItem ContextSelectAll;
    private PlainButton ButtonOperation;
    private readonly string MsgDelete;
    private readonly string MsgAddDup;
    private readonly string MsgEditDup;
    private readonly ListViewItemSet<TData> ItemsSet = new();
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
        MsgDelete = $"确认删除所选{desc}吗？此操作将不可撤销！";
        MsgAddDup = $"列表中已存在该{desc}，请重新添加！";
        MsgEditDup = $"列表中已存在该{desc}，请重新编辑！";
    }

    protected sealed override void OnInitializing()
    {
        this.AddControls(b =>
        [
            ListViewMain,

            ButtonOperation = b.Button("操作(&O) ▼", ContextMenuMain = ContextMenuBuilder.Build(b =>
            [
                b.Item("添加(&A)", (_, _) =>
                {
                    var dialog = GetSubDialog();

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
                var count = ListViewMain.SelectedItemsCount;
                var enable = count == 1;
                ContextDelete.Enabled = count != 0;
                ContextDuplicate.Enabled = enable;
                ContextEdit.Enabled = enable;
                ContextSelectAll.Enabled = Items.Count != 0;
            }))
        ]);

        ListViewMain.ContextMenu = ContextMenuMain;
        base.OnInitializing();
    }

    protected sealed override void RunLayout(bool isHighDpi)
    {
        ArrangeCommonButtonsR(ButtonA, ButtonB, ListViewMain, 1, 3);
        ArrangeControlYL(ButtonOperation, ListViewMain, -1, 3);
    }

    protected sealed override void OnLoad()
    {
        ListViewMain.Suspend(() =>
        {
            var hasData = !Data.IsNullOrEmpty();
            HasDefaultData = !DefaultData.IsNullOrEmpty();

            if (HasDefaultData)
            {
                foreach (var d in DefaultData)
                {
                    AddItem(d);
                }
            }

            if (hasData)
            {
                foreach (var d in Data)
                {
                    AddItem(d);
                }
            }

            if (HasDefaultData || hasData)
            {
                Items[0].EnsureVisible();
            }

            ListViewMain.AutoAdjustColumnWidth();
        });

        ListViewMain.MouseDoubleClick += ContextEdit_Click;
        ListViewMain.ListViewItemSorter = new ListViewItemComparer<TData>();
    }

    protected sealed override void OnKeyDown(KeyEventArgs e)
    {
        var handled = false;

        switch (e.Control, e.KeyCode)
        {
            case (true, Keys.A):
                ContextSelectAll_Click(null, null);
                handled = true;
                break;
            case (true, Keys.C):
                ContextDuplicate_Click(null, null);
                handled = true;
                break;
            case (false, Keys.Delete):
                ContextDelete_Click(null, null);
                handled = true;
                break;
        }

        e.Handled = handled;
        base.OnKeyDown(e);
    }

    protected sealed override bool OnClickButtonA()
    {
        var length = Items.Count;

        if (length == 0)
        {
            Data = [];
        }
        else
        {
            var tmp = new List<TData>(length);

            for (int i = 0; i < length; i++)
            {
                var data = (TData)Items[i].Tag;

                if (!OnCollectingData(data))
                {
                    tmp.Add(data);
                }
            }

            Data = [.. tmp];
            OnCollectingData(default);
        }

        return base.OnClickButtonA();
    }

    protected abstract int GetGroupIndex(TData data);

    protected abstract ListViewItem GetListViewItem(TData data);

    protected abstract IListViewSubDialog<TData> GetSubDialog(TData data = default);

    protected virtual bool OnCollectingData(TData data) => false;

    protected virtual bool OnRemovingData(TData data) => true;

    private void ContextDuplicate_Click(object sender, EventArgs e)
    {
        if (ListViewMain.SelectedItemsCount == 1)
        {
            var data = (TData)ListViewMain.SelectedItem.Tag;

            if (OnRemovingData(data))
            {
                var dialog = GetSubDialog(data);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    AddItemSafe(dialog.Data);
                }
            }
        }
    }

    private void ContextEdit_Click(object sender, EventArgs e)
    {
        if (ListViewMain.SelectedItemsCount != 0)
        {
            var item = ListViewMain.SelectedItem;
            var data = (TData)item.Tag;
            var dialog = GetSubDialog(data);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                EditItemSafe(item, dialog.Data, data);
            }
        }
    }

    private void ContextDelete_Click(object sender, EventArgs e)
    {
        var selected = ListViewMain.SelectedItemsCount;

        if (selected != 0 &&
            MessageX.Warn(MsgDelete, MessageButtons.YesNo) == DialogResult.Yes)
        {
            ListViewMain.Suspend(() =>
            {
                if (!HasDefaultData && selected == Items.Count)
                {
                    RemoveAllItems();
                }
                else
                {
                    var items = ListViewMain.SelectedItems;
                    ListViewItem item;

                    for (int i = selected - 1; i >= 0; i--)
                    {
                        item = items[i];
                        RemoveItem(item, (TData)item.Tag);
                    }
                }

                ListViewMain.AutoAdjustColumnWidth();
            });

            UserChanged();
        }
    }

    private void ContextSelectAll_Click(object sender, EventArgs e)
    {
        ListViewMain.Suspend(() => ListViewMain.SelectAll(true));
    }

    private void AddItemSafe(TData data)
    {
        if (ItemsSet.CanAdd(data))
        {
            AddItemCore(data);
        }
        else
        {
            MessageX.Error(MsgAddDup);

            var dialog = GetSubDialog(data);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AddItemSafe(dialog.Data);
            }
        }
    }

    private void EditItemSafe(ListViewItem item, TData newData, TData oldData)
    {
        var flag = ItemsSet.CanEdit(newData, item);

        if (flag != null)
        {
            if ((bool)flag)
            {
                RemoveItem(item, oldData, true);
                AddItemCore(newData);
            }
            else
            {
                MessageX.Error(MsgEditDup);

                var dialog = GetSubDialog(newData);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    EditItemSafe(item, dialog.Data, oldData);
                }
            }
        }
    }

    private void AddItemCore(TData data)
    {
        ListViewMain.Suspend(() =>
        {
            ListViewMain.SelectAll(false);
            AddItem(data, true);
            ListViewMain.AutoAdjustColumnWidth();
        });

        UserChanged();
    }

    private void AddItem(TData data, bool isSelected = false)
    {
        var item = GetListViewItem(data);
        item.Group = Groups[GetGroupIndex(data)];
        item.Tag = data;
        item.Selected = isSelected;
        item.Focused = isSelected;
        Items.Add(item);
        ItemsSet.Add(data, item);

        if (isSelected)
        {
            item.EnsureVisible();
        }
    }

    private void RemoveItem(ListViewItem item, TData data, bool isEdit = false)
    {
        if (isEdit || OnRemovingData(data))
        {
            Items.Remove(item);
            ItemsSet.Remove(data);
        }
    }

    private void RemoveAllItems()
    {
        Items.Clear();
        ItemsSet.Clear();
    }
}
