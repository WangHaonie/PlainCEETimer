using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Controls
{
    public abstract class ListViewDialog<TData, TSubDialog> : AppDialog
        where TData : IListViewData<TData>
        where TSubDialog : AppDialog, IListViewSubDialog<TData>
    {
        private class ListViewItemComparer<T> : IComparer
            where T : IListViewData<T>
        {
            int IComparer.Compare(object x, object y) => ((T)((ListViewItem)x).Tag).CompareTo((T)((ListViewItem)y).Tag);
        }

        public TData[] Data { get; set; }
        protected string ItemDescription { get; set; } = "项";

        private ContextMenu ContextMenuMain;
        private PlainButton ButtonOperation;
        private MenuItem ContextEdit;
        private MenuItem ContextDelete;
        private MenuItem ContextSelectAll;
        private readonly HashSet<TData> ItemsSet = [];
        private readonly ListView.ListViewItemCollection Items;
        private readonly ListViewGroupCollection Groups;
        private readonly ListViewEx ListViewMain = new()
        {
            Location = new(3, 3),
            UseCompatibleStateImageBehavior = false
        };

        protected ListViewDialog(int listViewWidth, string[] headers, string[] groups) : base(AppFormParam.AllControl)
        {
            InitializeComponent();
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
        /// 获取用于展示 <see cref="TData"/> 的 <see cref="ListViewItem"/> 实例 
        /// </summary>
        /// <param name="data">给定的数据</param>
        /// <returns><see cref="ListViewItem"/></returns>
        protected abstract ListViewItem GetListViewItem(TData data);

        /// <summary>
        /// 获取展示该 <see cref="TData"/> 的 <see cref="ListViewItem"/> 关联的分组的索引
        /// </summary>
        /// <param name="data">给定的数据</param>
        /// <returns>表示索引的 <see cref="int"/></returns>
        protected abstract int GetGroupIndex(TData data);

        /// <summary>
        /// 获取用于向用户显示 添加、更改、重试 的 <see cref="IListViewSubDialog{T}"/> 对话框实例。
        /// </summary>
        /// <param name="data">现有数据</param>
        /// <returns><see cref="IListViewSubDialog{TData}"/></returns>
        protected abstract IListViewSubDialog<TData> GetSubDialog(TData data = default);

        protected override void AdjustUI()
        {
            CompactControlsY(ButtonA, PanelMain);
            CompactControlsY(ButtonB, PanelMain);
            CompactControlsY(ButtonOperation, PanelMain);
            AlignControlsREx(ButtonA, ButtonB, PanelMain);
            AlignControlsL(ButtonOperation, ButtonA, PanelMain);
        }

        protected sealed override void OnLoad()
        {
            if (Data != null && Data.Length != 0)
            {
                ListViewMain.Suspend(() =>
                {
                    foreach (var data in Data)
                    {
                        AddItem(data);
                    }

                    Items[0].EnsureVisible();
                });
            }

            ListViewMain.AutoAdjustColumnWidth();
            ListViewMain.MouseDoubleClick += ListViewMain_MouseDoubleClick;
        }

        protected sealed override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                ContextSelectAll_Click(null, null);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                ContextDelete_Click(null, null);
                e.Handled = true;
            }

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

        protected void AddNewButton(Button Btn)
        {
            AlignControlsX(Btn, ButtonOperation);
            CompactControlsX(Btn, ButtonOperation, 6);
        }

        private void ButtonOperation_Click(object sender, EventArgs e)
        {
            ContextMenuMain.Show(ButtonOperation, new(0, ButtonOperation.Height));
        }

        private void ContextEdit_Click(object sender, EventArgs e)
        {
            var TargetItem = ListViewMain.SelectedItems[0];
            var TargetItemData = (TData)TargetItem.Tag;
            var SubDialog = GetSubDialog(TargetItemData);

            if (SubDialog.ShowDialog() == DialogResult.OK)
            {
                EditItemSafe(TargetItem, SubDialog.Data, TargetItemData);
            }
        }

        private void ContextDelete_Click(object sender, EventArgs e)
        {
            if (ListViewMain.SelectedItemsCount != 0 && MessageX.Warn($"确认删除所选{ItemDescription}吗？此操作将不可撤销！", Buttons: MessageButtons.YesNo) == DialogResult.Yes)
            {
                ListViewMain.Suspend(() =>
                {
                    foreach (ListViewItem Item in ListViewMain.SelectedItems)
                    {
                        RemoveItem(Item, (TData)Item.Tag);
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

        private void ListViewMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && ListViewMain.GetItemAt(e.X, e.Y) != null)
            {
                ContextEdit_Click(null, null);
            }
        }

        private void AddItemSafe(TData data)
        {
            if (ItemsSet.Add(data))
            {
                AddItemCore(data);
            }
            else
            {
                MessageX.Error($"检测待添加的{ItemDescription}与现有的重复。\n\n请重新添加！");
                var SubDialog = GetSubDialog(data);

                if (SubDialog.ShowDialog() == DialogResult.OK)
                {
                    AddItemSafe(SubDialog.Data);
                }
            }
        }

        private void EditItemSafe(ListViewItem item, TData newData, TData oldData)
        {
            if (ItemsSet.Add(newData))
            {
                RemoveItem(item, oldData);
                AddItemCore(newData);
            }
            else if (!newData.Equals(oldData))
            {
                MessageX.Error($"检测到此{ItemDescription}在编辑后与现有的重复。\n\n请重新编辑！");
                var SubDialog = GetSubDialog(newData);

                if (SubDialog.ShowDialog() == DialogResult.OK)
                {
                    EditItemSafe(item, SubDialog.Data, oldData);
                }
            }
        }

        private void AddItemCore(TData data)
        {
            ListViewMain.Suspend(() =>
            {
                ListViewMain.SelectAll(false);
                AddItem(data, true);
                ListViewMain.Sort();
                ListViewMain.AutoAdjustColumnWidth();
            });

            UserChanged();
        }

        private void AddItem(TData data, bool IsSelected = false)
        {
            var item = GetListViewItem(data);
            item.Group = Groups[GetGroupIndex(data)];
            item.Tag = data;
            item.Selected = IsSelected;
            item.Focused = IsSelected;
            Items.Add(item);
            ItemsSet.Add(data);

            if (IsSelected)
            {
                item.EnsureVisible();
            }
        }

        private void RemoveItem(ListViewItem item, TData data)
        {
            Items.Remove(item);
            ItemsSet.Remove(data);
        }

        private void InitializeComponent()
        {
            PanelMain.SuspendLayout();
            SuspendLayout();

            PanelMain.AutoSize = true;
            PanelMain.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            PanelMain.Controls.Add(ListViewMain);
            PanelMain.Location = new(6, 3);
            PanelMain.Size = new(453, 145);

            ButtonA.Enabled = false;
            ButtonA.Location = new(296, 149);
            ButtonA.Size = new(75, 23);
            ButtonA.Text = "保存(&S)";
            ButtonA.UseVisualStyleBackColor = true;

            ButtonB.Location = new(377, 149);
            ButtonB.Size = new(75, 23);
            ButtonB.Text = "取消(&C)";
            ButtonB.UseVisualStyleBackColor = true;

            ButtonOperation = new()
            {
                Location = new(9, 149),
                Size = new(75, 23),
                Text = "操作(&O) ▼",
                UseVisualStyleBackColor = true
            };

            ButtonOperation.Click += ButtonOperation_Click;

            AutoScaleDimensions = new(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new(460, 176);
            Controls.AddRange([ButtonOperation, ButtonB, ButtonA, PanelMain]);
            Font = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;

            PanelMain.ResumeLayout(false);
            ResumeLayout(false);

            ContextMenuMain = ContextMenuBuilder.Build(b =>
            [
                b.Item("添加(&A)", (_, _) =>
                {
                    var SubDialog = GetSubDialog();

                    if (SubDialog.ShowDialog() == DialogResult.OK)
                    {
                        AddItemSafe(SubDialog.Data);
                    }
                }),

                b.Separator(),
                ContextEdit = b.Item("编辑(&E)", ContextEdit_Click),
                ContextDelete = b.Item("删除(&D)", ContextDelete_Click),
                b.Separator(),
                ContextSelectAll = b.Item("全选(&Q)", ContextSelectAll_Click)
            ]);

            ListViewMain.ContextMenu = ContextMenuMain;
            ContextMenuMain.Popup += (_, _) => HandleMenuItemEnabling();
            ListViewMain.ListViewItemSorter = new ListViewItemComparer<TData>();
        }

        private void HandleMenuItemEnabling()
        {
            var SelectedCount = ListViewMain.SelectedItemsCount;
            ContextDelete.Enabled = SelectedCount != 0;
            ContextEdit.Enabled = SelectedCount == 1;
            ContextSelectAll.Enabled = Items.Count != 0;
        }
    }
}
