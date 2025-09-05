using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
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
    protected string ItemDescription { get; set; } = "项";

    protected override AppFormParam Params => AppFormParam.AllControl;

    private ContextMenu ContextMenuMain;
    private MenuItem ContextDuplicate;
    private MenuItem ContextEdit;
    private MenuItem ContextDelete;
    private MenuItem ContextSelectAll;
    private PlainButton ButtonOperation;
    private PlainButton NewButton;
    private readonly ListViewItemSet<TData> ItemsSet = new();
    private readonly ListView.ListViewItemCollection Items;
    private readonly ListViewGroupCollection Groups;

    private readonly PlainListView ListViewMain = new()
    {
        Location = new(3, 3),
        UseCompatibleStateImageBehavior = false
    };

    protected ListViewDialog(int listViewWidth, string[] headers, string[] groups)
    {
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
    }

    /// <summary>
    /// 获取展示该 <see cref="TData"/> 的 <see cref="ListViewItem"/> 关联的分组的索引
    /// </summary>
    /// <param name="data">给定的数据</param>
    /// <returns>表示索引的 <see cref="int"/></returns>
    protected abstract int GetGroupIndex(TData data);

    /// <summary>
    /// 获取用于展示 <see cref="TData"/> 的 <see cref="ListViewItem"/> 实例 
    /// </summary>
    /// <param name="data">给定的数据</param>
    /// <returns><see cref="ListViewItem"/></returns>
    protected abstract ListViewItem GetListViewItem(TData data);

    /// <summary>
    /// 获取用于向用户显示 添加、更改、重试 的 <see cref="IListViewSubDialog{T}"/> 对话框实例。
    /// </summary>
    /// <param name="data">现有数据</param>
    /// <returns><see cref="IListViewSubDialog{TData}"/></returns>
    protected abstract IListViewSubDialog<TData> GetSubDialog(TData data = default);

    /// <summary>
    /// 在 操作 按钮的右侧添加一个新按钮。
    /// </summary>
    protected virtual PlainButton AddButton(ControlBuilder b) => null;

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
                }),

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
            })),

            NewButton = AddButton(b)
        ]);

        ListViewMain.ContextMenu = ContextMenuMain;
        base.OnInitializing();
    }

    protected sealed override void StartLayout(bool isHighDpi)
    {
        ArrangeCommonButtonsR(ButtonA, ButtonB, ListViewMain, 1, 3);
        ArrangeControlYL(ButtonOperation, ListViewMain, -1, 3);

        if (NewButton != null)
        {
            ArrangeControlXT(NewButton, ButtonOperation, 3);
        }
    }

    protected sealed override void OnLoad()
    {
        ListViewMain.Suspend(() =>
        {
            var data = Data;

            if (data != null && data.Length != 0)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    AddItem(data[i]);
                }

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
        var tmp = new TData[length];

        for (int i = 0; i < length; i++)
        {
            tmp[i] = (TData)Items[i].Tag;
        }

        Data = tmp;
        return base.OnClickButtonA();
    }

    private void ContextDuplicate_Click(object sender, EventArgs e)
    {
        if (ListViewMain.SelectedItemsCount == 1)
        {
            var item = ListViewMain.SelectedItem;
            var dialog = GetSubDialog((TData)item.Tag);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AddItemSafe(dialog.Data);
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
            MessageX.Warn($"确认删除所选{ItemDescription}吗？此操作将不可撤销！", MessageButtons.YesNo) == DialogResult.Yes)
        {
            ListViewMain.Suspend(() =>
            {
                if (selected == Items.Count)
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
        ListViewMain.Suspend(() => ListViewMain.SelectAll(BOOL.TRUE));
    }

    private void AddItemSafe(TData data)
    {
        if (ItemsSet.CanAdd(data))
        {
            AddItemCore(data);
        }
        else
        {
            MessageX.Error($"检测待添加的{ItemDescription}与现有的重复。\n\n请重新添加！");
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
                RemoveItem(item, oldData);
                AddItemCore(newData);
            }
            else
            {
                MessageX.Error($"检测到此{ItemDescription}在编辑后与现有的重复。\n\n请重新编辑！");
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
            ListViewMain.SelectAll(BOOL.FALSE);
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

    private void RemoveItem(ListViewItem item, TData data)
    {
        Items.Remove(item);
        ItemsSet.Remove(data);
    }

    private void RemoveAllItems()
    {
        Items.Clear();
        ItemsSet.Clear();
    }
}
