﻿using System.Windows;
using System.Windows.Forms;

namespace PlainCEETimer.WPF.Controls
{
    public partial class AppMessageBox
    {
        private DialogResult Result;

        public AppMessageBox()
        {
            InitializeComponent();
        }

        public DialogResult ShowMessageDialog(string Text, string Title, Window Parent)
        {
            return Result;
        }
    }
}
