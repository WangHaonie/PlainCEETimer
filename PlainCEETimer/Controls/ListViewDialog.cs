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
        private readonly HashSet<TData> ListViewItemsSet = [];
        private readonly ListViewGroupCollection Groups;
        private readonly ListViewEx ListViewMain = new()
        {
            Left = 3,
            Top = 3,
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

                for (int i = 0; i < groups.Length; i++)
                {
                    Groups.Add(new(groups[i]));
                }
            }
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

                    ListViewMain.Items[0].EnsureVisible();
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
            var length = ListViewMain.ItemsCount;
            var tmp = new TData[length];
            var src = ListViewMain.Items;

            for (int i = 0; i < length; i++)
            {
                tmp[i] = (TData)src[i].Tag;
            }

            Data = tmp;
            return base.OnClickButtonA();
        }

        /// <summary>
        /// 在 <see cref="ButtonOperation"/> 的右侧添加一个新的按钮
        /// </summary>
        /// <param name="Btn">新按钮的实例</param>
        /// <param name="cxTweak">与 ButtonOperation 水平方向上的间距</param>
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
                AddItemSafe(SubDialog.Data, TargetItem, TargetItemData);
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

        private void AddItemSafe(TData data, ListViewItem item = null, TData old = default)
        {
            var EditMode = item != null && old != null;
            var CanAdd = ListViewItemsSet.Add(data);

            if ((EditMode && CanAdd) || CanAdd)
            {
                if (EditMode)
                {
                    RemoveItem(item, old);
                }

                ListViewMain.Suspend(() =>
                {
                    ListViewMain.SelectAll(false);
                    AddItem(data, true);
                    ListViewMain.Sort();
                    ListViewMain.AutoAdjustColumnWidth();
                });

                UserChanged();
            }
            else
            {
                MessageX.Error(EditMode
                    ? $"检测到此{ItemDescription}在编辑后与现有的重复。\n\n请重新编辑！"
                    : $"检测待添加的{ItemDescription}与现有的重复。\n\n请重新添加！");
                OpenRetryDialog(data);
            }
        }

        private void AddItem(TData data, bool IsSelected = false)
        {
            var item = GetListViewItem(data);
            item.Group = Groups[GetGroupIndex(data)];
            item.Tag = data;
            item.Selected = IsSelected;
            item.Focused = IsSelected;
            ListViewMain.Items.Add(item);
            ListViewItemsSet.Add(data);

            if (IsSelected)
            {
                item.EnsureVisible();
            }
        }

        private void RemoveItem(ListViewItem item, TData data)
        {
            ListViewMain.Items.Remove(item);
            ListViewItemsSet.Remove(data);
        }

        private void OpenRetryDialog(TData data)
        {
            var SubDialog = GetSubDialog(data);

            if (SubDialog.ShowDialog() == DialogResult.OK)
            {
                AddItemSafe(SubDialog.Data);
            }
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
            ContextSelectAll.Enabled = ListViewMain.ItemsCount != 0;
        }
    }
}
