﻿using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Modules;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI
{
    public class ControlBuilder
    {
        public Label Label(string text)
        {
            return Label(0, 0, text);
        }

        public Label Label(int x, int y, string text)
        {
            var ctrl = new Label() { Text = text };
            ctrl.SetBounds(x, y, 0, 0, BoundsSpecified.Location);
            ctrl.AutoSize = true;
            return ctrl;
        }

        public Label Block(EventHandler onClick)
        {
            return Block("          ", onClick);
        }

        public Label Block(string text)
        {
            return Block(text, null);
        }

        public PlainLinkLabel Link(string text, LinkLabelLinkClickedEventHandler onClick)
        {
            var ctrl = new PlainLinkLabel() { Text = text };
            ctrl.LinkClicked += onClick;
            return ctrl;
        }

        public Hyperlink Hyperlink(string text, string link)
        {
            return new() { HyperLink = link, Text = text };
        }

        public PlainButton Button(string text, EventHandler onClick)
        {
            return Button(text, true, false, onClick);
        }

        public PlainButton Button(string text, bool enabled, EventHandler onClick)
        {
            return Button(text, enabled, false, onClick);
        }

        public PlainButton Button(string text, bool enabled, bool autoSize, EventHandler onClick)
        {
            var ctrl = new PlainButton() { Text = text, Enabled = enabled };
            ctrl.SetBounds(0, 0, 75, 23);

            if (autoSize)
            {
                ctrl.AutoSize = true;
                ctrl.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            }

            if (onClick != null)
            {
                ctrl.Click += onClick;
            }

            return ctrl;
        }

        public PlainCheckBox CheckBox(string text, EventHandler onCheckedChanged, bool enabled = true, bool isChecked = false)
        {
            var ctrl = new PlainCheckBox { Text = text, Enabled = enabled, AutoSize = true };

            if (isChecked)
            {
                ctrl.Checked = true;
                ctrl.CheckState = CheckState.Checked;
            }

            ctrl.CheckedChanged += onCheckedChanged;
            return ctrl;
        }

        public PlainRadioButton RadioButton(string text, EventHandler onClick)
        {
            var ctrl = new PlainRadioButton { Text = text, AutoSize = true };
            ctrl.Click += onClick;
            return ctrl;
        }

        public ComboBoxEx ComboBox(int w, EventHandler onSelectedIndexChanged, params string[] strings)
        {
            return ComboBox(w, true, onSelectedIndexChanged, strings);
        }

        public ComboBoxEx ComboBox(int w, bool enabled, EventHandler onSelectedIndexChanged, params string[] strings)
        {
            var ctrl = new ComboBoxEx() { Enabled = enabled };
            ctrl.SetBounds(0, 0, w, 23);

            var dataLength = strings.Length;
            var data = new ComboData[dataLength];

            for (int i = 0; i < dataLength; i++)
            {
                data[i] = new(strings[i], i);
            }

            ctrl.DataSource = data;
            ctrl.DisplayMember = nameof(ComboData.Display);
            ctrl.ValueMember = nameof(ComboData.Value);

            if (onSelectedIndexChanged != null)
            {
                ctrl.SelectedIndexChanged += onSelectedIndexChanged;
            }

            return ctrl;
        }

        public PlainNumericUpDown NumericUpDown(int w, decimal maxValue, EventHandler onValueChanged)
        {
            var ctrl = new PlainNumericUpDown() { Maximum = maxValue };
            ctrl.SetBounds(0, 0, w, 23);
            ctrl.ValueChanged += onValueChanged;
            return ctrl;
        }

        public PlainTextBox TextBox(int w, EventHandler onTextChanged)
        {
            var ctrl = new PlainTextBox();
            ctrl.SetBounds(0, 0, w, 23);
            ctrl.MaxLength = Validator.MaxCustomTextLength;
            ctrl.TextChanged += onTextChanged;
            return ctrl;
        }

        public PictureBox Image(Image img)
        {
            var ctrl = new PictureBox() { Image = img };
            ctrl.SetBounds(5, 3, 32, 32);
            ctrl.SizeMode = PictureBoxSizeMode.Zoom;
            return ctrl;
        }

        public DateTimePicker DateTimePicker(int w, EventHandler onValueChanged)
        {
            var ctrl = new DateTimePicker() { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd dddd HH:mm:ss" };
            ctrl.SetBounds(0, 0, w, 23);
            ctrl.ValueChanged += onValueChanged;
            return ctrl;
        }

        public NavigationPage Page(Control[] controls)
        {
            var ctrl = new NavigationPage();
            ctrl.Controls.AddRange(controls);
            return ctrl;
        }

        public PlainGroupBox GroupBox(string text, Control[] controls)
        {
            var ctrl = new PlainGroupBox() { Text = text };
            ctrl.SetBounds(6, 6, 323, 0);
            ctrl.Controls.AddRange(controls);
            return ctrl;
        }

        public Panel Panel(int x, int y, int w, int h, Control[] controls)
        {
            var ctrl = new Panel();
            ctrl.SetBounds(x, y, w, h);
            ctrl.Controls.AddRange(controls);
            return ctrl;
        }

        public TControl New<TControl>(int w, int h, string text)
            where TControl : Control, new()
        {
            var ctrl = new TControl() { Text = text };
            ctrl.SetBounds(0, 0, w, h);
            return ctrl;
        }

        private Label Block(string text, EventHandler onClick)
        {
            var ctrl = Label(text);

            if (onClick != null)
            {
                ctrl.Click += onClick;
            }

            ctrl.BorderStyle = BorderStyle.FixedSingle;
            return ctrl;
        }
    }
}
