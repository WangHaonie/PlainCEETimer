﻿using System.Drawing;
using System.Media;
using System.Windows.Forms;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI.Dialogs
{
    public sealed class AppMessageBox(string message, bool autoClose, MessageButtons buttons, SystemSound sound, Bitmap icon) : AppDialog(AppFormParam.KeyPreview)
    {
        private DialogResult Result;
        private Label LabelMessage;
        private PictureBox ImageIcon;

        protected override void OnInitializing()
        {
            this.AddControls(b =>
            [
                ImageIcon = b.Image(icon),
                LabelMessage = b.Label(message).With(c => SetLabelAutoWrap(c, (int)(GetCurrentScreenRect().Width * 0.75)))
            ]);

            base.OnInitializing();
            ButtonA.Enabled = true;
        }

        protected override void StartLayout(bool isHighDpi)
        {
            ArrangeControlXTop(LabelMessage, ImageIcon, 2);
            ArrangeControlYRight(ButtonB, LabelMessage, -3, 3);
            ArrangeControlXTopRtl(ButtonA, ButtonB, -3);

            if (ButtonA.Left < ImageIcon.Right)
            {
                AlignControlLeft(ButtonA, LabelMessage, 3);
                ArrangeControlXTop(ButtonB, ButtonA, 3);
            }

            if (ButtonA.Top < ImageIcon.Bottom)
            {
                CompactControlY(ButtonA, ImageIcon);
                ArrangeControlXTop(ButtonB, ButtonA, 3);
            }
        }

        protected override void OnLoad()
        {
            switch (buttons)
            {
                case MessageButtons.YesNo:
                    ButtonA.Text = "是(&Y)";
                    ButtonB.Text = "否(&N)";
                    break;
                case MessageButtons.OK:
                    ButtonA.Visible = ButtonA.Enabled = false;
                    ButtonB.Text = "确定(&O)";
                    break;
            }
        }

        protected override void OnShown()
        {
            sound.Play();

            if (autoClose)
            {
                3200.AsDelay(_ => Invoke(Close));
            }
        }

        protected override bool OnClickButtonA()
        {
            Result = buttons == MessageButtons.YesNo ? DialogResult.Yes : DialogResult.None;
            Close();
            return true;
        }

        protected override void OnClickButtonB()
        {
            Result = buttons == MessageButtons.YesNo ? DialogResult.No : DialogResult.OK;
            Close();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }

            base.OnKeyDown(e);
        }

        public DialogResult ShowCore(AppForm owner, string title)
        {
            Text = title;

            if (owner == null)
            {
                AddParam(AppFormParam.CenterScreen);
            }
            else
            {
                StartPosition = FormStartPosition.CenterParent;
            }

            ShowDialog(owner);
            return Result;
        }
    }
}
