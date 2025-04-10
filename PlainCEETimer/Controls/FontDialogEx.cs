﻿using PlainCEETimer.Interop;
using PlainCEETimer.Modules;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class FontDialogEx : FontDialog, ICommDlg
    {
        public CommDlg DlgType => CommDlg.Font;
        public string DialogTitle => "选择字体 - 高考倒计时";

        private CommonDialogHelper Helper;

        public FontDialogEx(Font font)
        {
            AllowScriptChange = true;
            AllowVerticalFonts = false;
            Font = font;
            FontMustExist = true;
            MinSize = Validator.MinFontSize;
            MaxSize = Validator.MaxFontSize;
            ScriptsOnly = true;
            ShowColor = false;
            ShowEffects = false;
        }

        public DialogResult ShowDialog(AppForm owner)
        {
            Helper = new CommonDialogHelper(owner);
            return ShowDialog();
        }

        public IntPtr BaseHookProc(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam)
        {
            return base.HookProc(hWnd, Msg, wParam, lParam);
        }

        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            return Helper.HookProc(this, hWnd, msg, wparam, lparam);
        }
    }
}
