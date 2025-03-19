using PlainCEETimer.Controls;
using PlainCEETimer.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PlainCEETimer.Dialogs
{
    public partial class ExamInfoManager : AppDialog
    {
        public ExamInfoObject[] ExamInfo { get; set; }

        private readonly HashSet<ExamInfoObject> ItemDataSet = [];
        private readonly bool UseClassicContextMenu = MainForm.UseClassicContextMenu;
        private ContextMenu ContextMenuMain;
        private MenuItem ContextEdit;
        private MenuItem ContextDelete;
        private ContextMenuStrip ContextMenuStripMain;
        private ToolStripMenuItem StripEdit;
        private ToolStripMenuItem StripDelete;

        public ExamInfoManager() : base(AppDialogProp.BindButtons | AppDialogProp.KeyPreview)
        {
            CompositedStyle = true;
            InitializeComponent();
            InitializeExtra();
        }

        protected override void OnLoad()
        {
            if (ExamInfo != null)
            {
                if (ExamInfo.Count() == 0)
                {
                    AddItem(new ExamInfoObject() { Name = "文本测试" });
                    ListViewMain.AutoAdjustColumnWidth();
                    ListViewMain.ClearAll();
                }
                else
                {
                    ListViewMain.Suspend(() =>
                    {
                        foreach (var Info in ExamInfo)
                        {
                            AddItem(Info);
                            ItemDataSet.Add(Info);
                        }

                        ListViewMain.AutoAdjustColumnWidth();
                    });
                }
            }

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
            ExamInfo = ListViewMain.GetData<ExamInfoObject>();
            base.ButtonA_Click();
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

        private void ButtonOperation_Click(object sender, EventArgs e)
        {
            ShowContextMenu(ButtonOperation, ContextMenuMain, ContextMenuStripMain, UseClassicContextMenu);
        }

        private void ContextAdd_Click(object sender, EventArgs e)
        {
            ExamInfoDialog Dialog = new();

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                AddItemSafe(Dialog.ExamInfo);
            }
        }

        private void ContextEdit_Click(object sender, EventArgs e)
        {
            var TargetItem = ListViewMain.SelectedItem;
            ExamInfoDialog Dialog = new((ExamInfoObject)TargetItem.Tag);

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                AddItemSafe(Dialog.ExamInfo, TargetItem);
            }
        }

        private void ContextDelete_Click(object sender, EventArgs e)
        {
            if (ListViewMain.SelectedItemsCount != 0 && MessageX.Warn("确认删除所选考试信息吗？此操作将不可撤销！", Buttons: AppMessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ListViewMain.RemoveSelectedItems();
                UserChanged();
            }
        }

        private void ContextSelectAll_Click(object sender, EventArgs e)
        {
            if (ListViewMain.ItemsCount != 0)
            {
                foreach (ListViewItem Item in ListViewMain.Items)
                {
                    Item.Selected = true;
                }
            }
        }

        private void InitializeExtra()
        {
            if (UseClassicContextMenu)
            {
                ContextMenuMain = CreateNew
                ([
                    AddItem("添加(&A)", ContextAdd_Click),
                    AddSeparator(),
                    ContextEdit = AddItem("编辑(&E)", ContextEdit_Click),
                    ContextDelete = AddItem("删除(&D)", ContextDelete_Click),
                    AddSeparator(),
                    AddItem("全选(&Q)", ContextSelectAll_Click)
                ]);

                ListViewMain.ContextMenu = ContextMenuMain;
                ContextMenuMain.Popup += (_, _) => HandleMenuItemEnabling();
            }
            else
            {
                ContextMenuStripMain = CreateNewStrip
                ([
                    AddStripItem("添加(&A)", ContextAdd_Click),
                    AddStripSeparator(),
                    StripEdit = AddStripItem("编辑(&E)", ContextEdit_Click),
                    StripDelete = AddStripItem("删除(&D)", ContextDelete_Click),
                    AddStripSeparator(),
                    AddStripItem("全选(&Q)", ContextSelectAll_Click)
                ]);

                ListViewMain.ContextMenuStrip = ContextMenuStripMain;
                ContextMenuStripMain.Opening += (_, _) => HandleMenuItemEnabling();
            }

            ListViewMain.ListViewItemSorter = new PlainComparer<ExamInfoObject>();
        }

        private void HandleMenuItemEnabling()
        {
            var SelectedCount = ListViewMain.SelectedItemsCount;
            var EnableDelete = SelectedCount != 0;
            var EnableEdit = SelectedCount == 1;

            if (UseClassicContextMenu)
            {
                ContextDelete.Enabled = EnableDelete;
                ContextEdit.Enabled = EnableEdit;
            }
            else
            {
                StripDelete.Enabled = EnableDelete;
                StripEdit.Enabled = EnableEdit;
            }
        }

        private void AddItemSafe(ExamInfoObject Info, ListViewItem Item = null)
        {
            var EditMode = Item != null;

            if (ItemDataSet.Add(Info))
            {
                if (EditMode)
                {
                    ListViewMain.Items.Remove(Item);
                }

                AddItem(Info);
                ListViewMain.Sort();
                ListViewMain.AutoAdjustColumnWidth();
                UserChanged();
            }
            else
            {
                MessageX.Error(EditMode ? "检测到此考试信息在编辑后与现有的重复。\n\n请重新编辑！" : "检测待添加的考试信息与现有的重复。\n\n请重新添加！");
                ExamInfoDialog Dialog = new(Info);

                if (Dialog.ShowDialog() == DialogResult.OK)
                {
                    AddItemSafe(Dialog.ExamInfo);
                }
            }
        }

        private void AddItem(ExamInfoObject Info)
        {
            ListViewMain.Items.Add(new ListViewItem([Info.Name, Info.Start.ToString(App.DateTimeFormat), Info.End.ToString(App.DateTimeFormat)])
            {
                Tag = Info
            });
        }
    }
}
