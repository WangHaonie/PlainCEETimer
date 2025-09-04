using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.UI.Controls;

namespace PlainCEETimer.UI;

public class ControlBuilder
{
    public PlainLabel Label(string text)
    {
        return new PlainLabel(text);
    }

    public ColorBlock Block(string text)
    {
        return Block(true, false, text, null, null);
    }

    public ColorBlock Block(bool isFore, ColorBlock preview, EventHandler onColorChanged)
    {
        return Block(false, isFore, null, preview, onColorChanged);
    }

    public PlainLinkLabel Link(string text, LinkLabelLinkClickedEventHandler onClick)
    {
        var ctrl = new PlainLinkLabel() { Text = text };
        ctrl.LinkClicked += onClick;
        return ctrl;
    }

    public PlainLinkLabel Hyperlink(string text, string link)
    {
        return new() { Hyperlink = link, Text = text };
    }

    public PlainButton Button(string text, EventHandler onClick)
    {
        return Button(text, false, onClick, null);
    }

    public PlainButton Button(string text, ContextMenu menu)
    {
        return Button(text, false, null, menu);
    }

    public PlainButton Button(string text, int maxW, int maxH, EventHandler onClick)
    {
        var ctrl = Button(text, false, onClick, null);
        ctrl.MinimumSize = new();
        ctrl.MaximumSize = new(maxW, maxH);
        return ctrl;
    }

    public PlainButton Button(string text, bool autoSize, EventHandler onClick)
    {
        return Button(text, autoSize, onClick, null);
    }

    public PlainCheckBox CheckBox(string text, EventHandler onCheckedChanged)
    {
        var ctrl = new PlainCheckBox { Text = text, AutoSize = true };
        ctrl.CheckedChanged += onCheckedChanged;
        return ctrl;
    }

    public PlainRadioButton RadioButton(string text, EventHandler onClick)
    {
        var ctrl = new PlainRadioButton { Text = text, AutoSize = true };
        ctrl.Click += onClick;
        return ctrl;
    }

    public PlainComboBox ComboBox(int w, EventHandler onSelectedIndexChanged, params string[] items)
    {
        var ctrl = new PlainComboBox();
        ctrl.SetBounds(0, 0, w, 23);

        var data = new ComboData[items.Length];

        for (int i = 0; i < items.Length; i++)
        {
            data[i] = new(items[i], i);
        }

        ctrl.DataSource = data;
        ctrl.DisplayMember = nameof(ComboData.Display);
        ctrl.ValueMember = nameof(ComboData.Value);
        ctrl.SelectedIndexChanged += onSelectedIndexChanged;
        return ctrl;
    }

    public PlainNumericUpDown NumericUpDown(int w, decimal maxValue, EventHandler onValueChanged)
    {
        var ctrl = new PlainNumericUpDown() { Maximum = maxValue };
        ctrl.SetBounds(0, 0, w, 23);
        ctrl.ValueChanged += onValueChanged;
        return ctrl;
    }

    public PlainTextBox TextBox(int w, bool expandable, EventHandler onTextChanged)
    {
        var ctrl = new PlainTextBox(expandable);
        ctrl.SetBounds(0, 0, w, 23);
        ctrl.MaxLength = Validator.MaxCustomTextLength;
        ctrl.TextChanged += onTextChanged;
        return ctrl;
    }

    public PlainTextBox TextArea(int w, int h, EventHandler onTextChanged)
    {
        var ctrl = TextBox(w, false, onTextChanged);
        ctrl.Multiline = true;
        ctrl.ScrollBars = ScrollBars.Vertical;
        ctrl.WordWrap = true;
        ctrl.HideSelection = false;
        ctrl.SetBounds(0, 0, w, h);
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

    private ColorBlock Block(bool isPreview, bool isFore, string text, ColorBlock preview, EventHandler onColorChanged)
    {
        var ctrl = new ColorBlock(isPreview, isFore, preview);

        if (text != null)
        {
            ctrl.Text = text;
        }

        ctrl.ColorChanged += onColorChanged;
        return ctrl;
    }

    private PlainButton Button(string text, bool autoSize, EventHandler onClick, ContextMenu menu)
    {
        var ctrl = new PlainButton(menu) { Text = text, MinimumSize = new(0, 23) };

        if (autoSize)
        {
            ctrl.AutoSize = true;
            ctrl.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        ctrl.Click += onClick;
        return ctrl;
    }
}
