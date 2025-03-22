using PlainCEETimer.Forms;
using PlainCEETimer.Modules;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public abstract class ListViewDialog<TData> : AppDialog where TData : IListViewObject<TData>
    {
        public TData[] Data { get; set; }

        protected string ContentDescription { get; set; }

        protected string DialogTitle
        {
            get => Text;
            set => Text = value;
        }

        protected string[] ListViewHeaders
        {
            get => ListViewMain.Headers;
            set => ListViewMain.Headers = value;
        }

        protected int ListViewWidth
        {
            get => ListViewMain.Size.Width;
            set => ListViewMain.Size = new(value, 140);
        }

        protected ListViewItem SelectedItem
        {
            get => ListViewMain.SelectedItem;
        }

        private readonly ListViewEx ListViewMain = new()
        {
            Location = new(3, 3),
            UseCompatibleStateImageBehavior = false
        };

        private Button ButtonOperation;
        private readonly bool UseClassicContextMenu = MainForm.UseClassicContextMenu;
        private readonly HashSet<TData> ListViewItemsIndex = [];
        private ContextMenu ContextMenuMain;
        private MenuItem ContextEdit;
        private MenuItem ContextDelete;
        private MenuItem ContextSelectAll;
        private ContextMenuStrip ContextMenuStripMain;
        private ToolStripMenuItem StripEdit;
        private ToolStripMenuItem StripDelete;
        private ToolStripMenuItem StripSelectAll;

        protected ListViewDialog() : base(AppDialogProp.BindButtons | AppDialogProp.KeyPreview)
        {
            CompositedStyle = true;
            InitializeComponent();
        }

        protected override void OnLoad()
        {
            CompactControlsY(ButtonA, PanelMain);
            CompactControlsY(ButtonB, PanelMain);
            CompactControlsY(ButtonOperation, PanelMain);
            AlignControlsREx(ButtonA, ButtonB, PanelMain);
            AlignControlsL(ButtonOperation, ButtonA, PanelMain);

            if (Data != null && Data.Count() != 0)
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

        protected override void OnKeyDown(KeyEventArgs e)
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

        protected override void ButtonA_Click()
        {
            Data = ListViewMain.GetData<TData>();
            base.ButtonA_Click();
        }

        protected abstract void ContextAdd_Click(object sender, EventArgs e);

        protected abstract void ContextEdit_Click(object sender, EventArgs e);

        protected abstract void OpenRetryDialog(TData Data);

        protected abstract void AddItem(TData Info, bool IsSelected = false);

        protected void AlignExtraButtonL(Button Btn, int cxTweak = 0)
        {
            AlignControlsX(Btn, ButtonOperation);
            CompactControlsX(Btn, ButtonOperation, cxTweak);
        }

        protected void AddItem(ListViewItem Item, TData TagData)
        {
            ListViewMain.Items.Add(Item);
            ListViewItemsIndex.Add(TagData);
        }

        protected void AddItemSafe(TData Info, ListViewItem Item = null)
        {
            var EditMode = Item != null;

            if (ListViewItemsIndex.Add(Info) || EditMode)
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

        private void ButtonOperation_Click(object sender, EventArgs e)
        {
            ShowContextMenu(ButtonOperation, ContextMenuMain, ContextMenuStripMain, UseClassicContextMenu);
        }

        private void ContextDelete_Click(object sender, EventArgs e)
        {
            if (ListViewMain.SelectedItemsCount != 0 && MessageX.Warn($"确认删除所选{ContentDescription}吗？此操作将不可撤销！", Buttons: AppMessageBoxButtons.YesNo) == DialogResult.Yes)
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

        private void RemoveItem(ListViewItem Item, TData TagData)
        {
            ListViewMain.Items.Remove(Item);
            ListViewItemsIndex.Remove(TagData);
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
            Controls.Add(ButtonOperation);
            Controls.Add(ButtonB);
            Controls.Add(ButtonA);
            Controls.Add(PanelMain);
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
                    AddItem(ContextMenuConstants.Add, ContextAdd_Click),
                    AddSeparator(),
                    ContextEdit = AddItem(ContextMenuConstants.Edit, ContextEdit_Click),
                    ContextDelete = AddItem(ContextMenuConstants.Delete, ContextDelete_Click),
                    AddSeparator(),
                    ContextSelectAll = AddItem(ContextMenuConstants.SelectAll, ContextSelectAll_Click)
                ]);

                ListViewMain.ContextMenu = ContextMenuMain;
                ContextMenuMain.Popup += (_, _) => HandleMenuItemEnabling();
            }
            else
            {
                ContextMenuStripMain = CreateNewStrip
                ([
                    AddStripItem(ContextMenuConstants.Add, ContextAdd_Click),
                    AddStripSeparator(),
                    StripEdit = AddStripItem(ContextMenuConstants.Edit, ContextEdit_Click),
                    StripDelete = AddStripItem(ContextMenuConstants.Delete, ContextDelete_Click),
                    AddStripSeparator(),
                    StripSelectAll = AddStripItem(ContextMenuConstants.SelectAll, ContextSelectAll_Click)
                ]);

                ListViewMain.ContextMenuStrip = ContextMenuStripMain;
                ContextMenuStripMain.Opening += (_, _) => HandleMenuItemEnabling();
            }

            ListViewMain.ListViewItemSorter = new PlainComparer<TData>();
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
