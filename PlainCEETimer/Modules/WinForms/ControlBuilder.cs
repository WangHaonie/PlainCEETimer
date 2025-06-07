using System;
using System.Windows.Forms;
using PlainCEETimer.Controls;

namespace PlainCEETimer.Modules.WinForms
{
    public class ControlBuilder
    {
        public PlainLabel Label(string text)
        {
            return Label(0, 0, text, null);
        }

        public PlainLabel Label(int x, int y, string text)
        {
            return Label(x, y, text, null);
        }

        public PlainLabel Label(int x, int y, string text, Action<PlainLabel> additions)
        {
            var ctrl = new PlainLabel() { Text = text };
            ctrl.SetBounds(x, y, 0, 0, BoundsSpecified.Location);
            additions?.Invoke(ctrl);
            return ctrl;
        }

        public PlainLabel Block(int x, int y, EventHandler onClick, string text = "          ")
        {
            var ctrl = Label(x, y, text, null);
            ctrl.Click += onClick;
            ctrl.BorderStyle = BorderStyle.FixedSingle;
            return ctrl;
        }

        public PlainLinkLabel Link(int x, int y, string text, LinkLabelLinkClickedEventHandler OnClick)
        {
            var ctrl = new PlainLinkLabel() { Text = text };
            ctrl.SetBounds(x, y, 0, 0, BoundsSpecified.Location);
            ctrl.LinkClicked += OnClick;
            return ctrl;
        }

        public Hyperlink Hyperlink(string text, string link)
        {
            return new() { HyperLink = link, Text = text };
        }

        public PlainButton Button(int x, int y, string text)
        {
            return Button(x, y, text, true, null);
        }

        public PlainButton Button(int x, int y, string text, EventHandler onClick)
        {
            return Button(x, y, text, true, onClick);
        }

        public PlainButton Button(int x, int y, string text, bool enabled, EventHandler onClick)
        {
            var ctrl = new PlainButton() { Text = text, Enabled = enabled };
            ctrl.SetBounds(x, y, 75, 25);

            if (onClick != null)
            {
                ctrl.Click += onClick;
            }

            return ctrl;
        }

        public PlainCheckBox CheckBox(int x, int y, string text, EventHandler onCheckedChanged, bool enabled = true, bool isChecked = false)
        {
            var ctrl = new PlainCheckBox() { Text = text };
            ctrl.SetBounds(x, y, 0, 0, BoundsSpecified.Location);
            ctrl.Enabled = enabled;

            if (isChecked)
            {
                ctrl.Checked = true;
                ctrl.CheckState = CheckState.Checked;
            }

            ctrl.CheckedChanged += onCheckedChanged;
            return ctrl;
        }

        public ComboBoxEx ComboBox(int x, int y, int w, EventHandler onSelectedIndexChanged, ComboData[] data)
        {
            return ComboBox(x, y, w, true, onSelectedIndexChanged, data);
        }

        public ComboBoxEx ComboBox(int x, int y, int w, bool enabled, EventHandler onSelectedIndexChanged, ComboData[] data)
        {
            var ctrl = new ComboBoxEx() { Enabled = enabled };
            ctrl.SetBounds(x, y, w, 23);
            ctrl.DataSource = data;
            ctrl.DisplayMember = nameof(ComboData.Display);
            ctrl.ValueMember = nameof(ComboData.Value);

            if (onSelectedIndexChanged != null)
            {
                ctrl.SelectedIndexChanged += onSelectedIndexChanged;
            }

            return ctrl;
        }

        public PlainNumericUpDown NumericUpDown(int x, int y, int w, decimal maxValue, EventHandler onValueChanged)
        {
            var ctrl = new PlainNumericUpDown() { Maximum = maxValue };
            ctrl.SetBounds(x, y, w, 23);
            ctrl.ValueChanged += onValueChanged;
            return ctrl;
        }

        public PlainTextBox TextBox(int x, int y, int w, EventHandler onTextChanged)
        {
            var ctrl = new PlainTextBox();
            ctrl.SetBounds(x, y, w, 23);
            ctrl.TextChanged += onTextChanged;
            return ctrl;
        }

        public NavigationPage Page(Control[] controls)
        {
            var ctrl = new NavigationPage();
            ctrl.Controls.AddRange(controls);
            return ctrl;
        }

        public TContainer Container<TContainer>(int x, int y, int w, int h, Control[] controls)
            where TContainer : Control, new()
        {
            return Container<TContainer>(x, y, w, h, null, controls);
        }

        public TContainer Container<TContainer>(int x, int y, int w, int h, string text, Control[] controls)
            where TContainer : Control, new()
        {
            var ctrl = new TContainer() { Text = text };
            ctrl.SetBounds(x, y, w, h);
            ctrl.Controls.AddRange(controls);
            return ctrl;
        }

        //public TControl New<TControl>(string text, Action<TControl> additions = null)
        //    where TControl : Control, new()
        //{
        //    var ctrl = new TControl() { Text = text };
        //    additions?.Invoke(ctrl);
        //    return ctrl;
        //}

        //public TControl New<TControl>(int x, int y, string text, Action<TControl> additions = null)
        //    where TControl : Control, new()
        //{
        //    var ctrl = new TControl() { Text = text };
        //    ctrl.SetBounds(x, y, 0, 0, BoundsSpecified.Location);
        //    additions?.Invoke(ctrl);
        //    return ctrl;
        //}

        //public TControl New<TControl>(int x, int y, int w, int h, string text, Action<TControl> additions = null)
        //    where TControl : Control, new()
        //{
        //    var ctrl = new TControl() { Text = text };
        //    ctrl.SetBounds(x, y, w, h);
        //    additions?.Invoke(ctrl);
        //    return ctrl;
        //}

        public TControl Modify<TControl>(TControl instance, int x, int y, int w, int h, string text, Action<TControl> additions = null)
            where TControl : Control, new()
        {
            instance.SetBounds(x, y, w, h);
            instance.Text = text;
            additions?.Invoke(instance);
            return instance;
        }
    }
}
