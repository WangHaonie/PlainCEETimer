using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PlainCEETimer.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.Controls
{
    public abstract class ListViewDialog<TData, TSubDialog> : AppDialog
        where TData : IListViewObject<TData>
        where TSubDialog : AppDialog, ISubDialog<TData>
    {
        public TData[] Data { get; set; }
        private ContextMenu ContextMenuMain;
        private ContextMenuStrip ContextMenuStripMain;
        private PlainButton ButtonOperation;
        private MenuItem ContextEdit;
        private MenuItem ContextDelete;
        private MenuItem ContextSelectAll;
        private ToolStripMenuItem StripEdit;
        private ToolStripMenuItem StripDelete;
        private ToolStripMenuItem StripSelectAll;
        private readonly bool UseClassicContextMenu = MainForm.UseClassicContextMenu;
        private readonly string ContentDescription;
        private readonly HashSet<TData> ListViewItemsSet = [];
        private readonly ListViewEx ListViewMain = new()
        {
            Location = new(3, 3),
            UseCompatibleStateImageBehavior = false
        };

        private ListViewDialog() : base(AppFormParam.AllControl | AppFormParam.CompositedStyle)
        {
            InitializeComponent();
        }

        protected ListViewDialog(int listViewWidth, string title, string content, string[] headers) : this()
        {
            Text = title;
            ListViewMain.Headers = headers;
            ListViewMain.Size = new Size(ScaleToDpi(listViewWidth), ScaleToDpi(218));
            ContentDescription = content;
        }

        /// <summary>
        /// 实现向 ListView 添加新项的逻辑。请务必在此调用基类 <see cref="AddItem(ListViewItem, TData)"/> 方法以完成添加。
        /// </summary>
        /// <param name="Data">该项包含的数据</param>
        /// <param name="IsSelected">是否选中该项</param>
        protected abstract void AddItem(TData Data, bool IsSelected = false);

        /// <summary>
        /// 获取用于向用户显示 添加、更改、重试 的对话框实例。
        /// </summary>
        /// <param name="Existing">现有数据</param>
        /// <returns><see cref="ISubDialog{TData}"/> 实例</returns>
        protected abstract ISubDialog<TData> GetSubDialogInstance(TData Existing = default);

        protected override void AdjustUI()
        {
            CompactControlsY(ButtonA, PanelMain);
            CompactControlsY(ButtonB, PanelMain);
            CompactControlsY(ButtonOperation, PanelMain);
            AlignControlsREx(ButtonA, ButtonB, PanelMain);
            AlignControlsL(ButtonOperation, ButtonA, PanelMain);
        }

        protected override void OnLoad()
        {
            if (Data?.Count() != 0)
            {
                ListViewMain.Suspend(() =>
                {
                    foreach (var Info in Data)
                    {
                        AddItem(Info);
                    }

                    ListViewMain.AutoAdjustColumnWidth();
                });
            }

            ListViewMain.AutoAdjustColumnWidth();
            ListViewMain.MouseDoubleClick += ListViewMain_MouseDoubleClick;
        }

        protected override void OnShown()
        {
            ListViewMain.Focus();
        }

        protected sealed override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                ContextSelectAll_Click(null, null);
            }
            else if (e.KeyCode == Keys.Delete)
            {
                ContextDelete_Click(null, null);
            }

            e.Handled = true;
            base.OnKeyDown(e);
        }

        protected sealed override bool ButtonA_Click()
        {
            var length = ListViewMain.ItemsCount;
            var tmp = new TData[length];
            var src = ListViewMain.Items;

            for (int i = 0; i < length; i++)
            {
                tmp[i] = (TData)src[i].Tag;
            }

            Data = tmp;
            return base.ButtonA_Click();
        }

        /// <summary>
        /// 在 <see cref="ButtonOperation"/> 的右侧添加一个新的按钮
        /// </summary>
        /// <param name="Btn">新按钮的实例</param>
        /// <param name="cxTweak">与 ButtonOperation 水平方向上的间距</param>
        protected void AddNewButton(Button Btn, int cxTweak = 0)
        {
            AlignControlsX(Btn, ButtonOperation);
            CompactControlsX(Btn, ButtonOperation, cxTweak);
        }

        /// <summary>
        /// 向 ListView 添加项。该方法应当在 <see cref="AddItem(TData, bool)"/> 中被调用
        /// </summary>
        /// <param name="Item">ListView 项</param>
        /// <param name="Data">包含的数据</param>
        protected void AddItem(ListViewItem Item, TData Data)
        {
            ListViewMain.Items.Add(Item);
            ListViewItemsSet.Add(Data);
        }

        private void ButtonOperation_Click(object sender, EventArgs e)
        {
            ShowContextMenu(ButtonOperation, ContextMenuMain, ContextMenuStripMain, UseClassicContextMenu);
        }

        private void ContextAdd_Click(object sender, EventArgs e)
        {
            var SubDialog = GetSubDialogInstance();

            if (SubDialog.ShowDialog() == DialogResult.OK)
            {
                AddItemSafe(SubDialog.Data);
            }
        }

        private void ContextEdit_Click(object sender, EventArgs e)
        {
            var TargetItem = ListViewMain.SelectedItems[0];
            var SubDialog = GetSubDialogInstance((TData)TargetItem.Tag);

            if (SubDialog.ShowDialog() == DialogResult.OK)
            {
                AddItemSafe(SubDialog.Data, TargetItem);
            }
        }

        private void ContextDelete_Click(object sender, EventArgs e)
        {
            if (ListViewMain.SelectedItemsCount != 0 && MessageX.Warn($"确认删除所选{ContentDescription}吗？此操作将不可撤销！", Buttons: MessageButtons.YesNo) == DialogResult.Yes)
            {
                ListViewMain.Suspend(() =>
                {
                    foreach (ListViewItem Item in ListViewMain.SelectedItems)
                    {
                        RemoveItem(Item, (TData)Item.Tag);
                    }
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
            if (e.Button == MouseButtons.Left)
            {
                var MouseLocation = e.Location;

                if (ListViewMain.GetItemAt(MouseLocation.X, MouseLocation.Y) != null)
                {
                    ContextEdit_Click(null, null);
                }
            }
        }

        private void AddItemSafe(TData Info, ListViewItem Item = null)
        {
            var EditMode = Item != null;

            if (ListViewItemsSet.Add(Info) || EditMode)
            {
                if (EditMode)
                {
                    RemoveItem(Item, Info);
                }

                ListViewMain.Suspend(() =>
                {
                    ListViewMain.SelectAll(false);
                    AddItem(Info, true);
                    ListViewMain.Sort();
                    ListViewMain.AutoAdjustColumnWidth();
                });

                UserChanged();
            }
            else
            {
                MessageX.Error(EditMode
                    ? $"检测到此{ContentDescription}在编辑后与现有的重复。\n\n请重新编辑！"
                    : $"检测待添加的{ContentDescription}与现有的重复。\n\n请重新添加！");
                OpenRetryDialog(Info);
            }
        }

        private void OpenRetryDialog(TData Data)
        {
            var SubDialog = GetSubDialogInstance(Data);

            if (SubDialog.ShowDialog() == DialogResult.OK)
            {
                AddItemSafe(SubDialog.Data);
            }
        }

        private void RemoveItem(ListViewItem Item, TData TagData)
        {
            ListViewMain.Items.Remove(Item);
            ListViewItemsSet.Remove(TagData);
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

            if (UseClassicContextMenu)
            {
                ContextMenuMain = CreateNew
                ([
                    AddItem(Constants.Add, ContextAdd_Click),
                    AddSeparator(),
                    ContextEdit = AddItem(Constants.Edit, ContextEdit_Click),
                    ContextDelete = AddItem(Constants.Delete, ContextDelete_Click),
                    AddSeparator(),
                    ContextSelectAll = AddItem(Constants.SelectAll, ContextSelectAll_Click)
                ]);

                ListViewMain.ContextMenu = ContextMenuMain;
                ContextMenuMain.Popup += (_, _) => HandleMenuItemEnabling();
            }
            else
            {
                ContextMenuStripMain = CreateNewStrip
                ([
                    AddStripItem(Constants.Add, ContextAdd_Click),
                    AddStripSeparator(),
                    StripEdit = AddStripItem(Constants.Edit, ContextEdit_Click),
                    StripDelete = AddStripItem(Constants.Delete, ContextDelete_Click),
                    AddStripSeparator(),
                    StripSelectAll = AddStripItem(Constants.SelectAll, ContextSelectAll_Click)
                ]);

                ListViewMain.ContextMenuStrip = ContextMenuStripMain;
                ContextMenuStripMain.Opening += (_, _) => HandleMenuItemEnabling();
            }

            ListViewMain.ListViewItemSorter = new PlainListViewComparer<TData>();
        }

        private void HandleMenuItemEnabling()
        {
            var SelectedCount = ListViewMain.SelectedItemsCount;
            var EnableDelete = SelectedCount != 0;
            var EnableEdit = SelectedCount == 1;
            var EnableSelectAll = ListViewMain.ItemsCount != 0;

            if (UseClassicContextMenu)
            {
                ContextDelete.Enabled = EnableDelete;
                ContextEdit.Enabled = EnableEdit;
                ContextSelectAll.Enabled = EnableSelectAll;
            }
            else
            {
                StripDelete.Enabled = EnableDelete;
                StripEdit.Enabled = EnableEdit;
                StripSelectAll.Enabled = EnableSelectAll;
            }
        }
    }
}
