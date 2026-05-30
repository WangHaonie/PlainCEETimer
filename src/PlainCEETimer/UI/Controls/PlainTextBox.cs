using System;
using System.Drawing;
using System.Windows.Forms;
using PlainCEETimer.Interop;
using PlainCEETimer.Interop.Extensions;
using PlainCEETimer.Modules;
using PlainCEETimer.Modules.Configuration;
using PlainCEETimer.Modules.Extensions;
using PlainCEETimer.UI.Extensions;

namespace PlainCEETimer.UI.Controls;

public sealed class PlainTextBox : TextBox, IThemeAware
{
    private sealed class TextBoxFlyout(PlainTextBox parent) : AppForm
    {
        public string Content
        {
            get => ContentBox.Text;
            set => ContentBox.Text = value;
        }

        protected override AppWindowStyle Params => AppWindowStyle.RoundCornerSmall | AppWindowStyle.OnEscClosing | AppWindowStyle.ModelessDialog;

        protected override bool SuppressAutoPosition => true;

        private PlainTextBox ContentBox;
        private PlainButton ButtonClose;
        private PlainButton ButtonApply;
        private PlainTextCounter LabelCounter;

        public void Input(string text)
        {
            ContentBox.Input(ContentBox.Text.Clean().Length + text.Length, text);
        }

        protected override void OnInitializing()
        {
            base.OnInitializing();
            Location = parent.LocationToScreen(-4, -4);

            this.AddControls(b =>
            [
                ContentBox = b.TextArea(0, 0, null),
                ButtonClose = b.Button("×", 18, 20, (_, _) => Close()),
                ButtonApply = b.Button("√", 18, 20, ButtonApply_Click),
                LabelCounter = b.Counter(ContentBox, ConfigValidator.IsValidCustomLength)
            ]);
        }

        protected override void RunLayout(bool isHighDpi)
        {
            ContentBox.Text = parent.Text;
            ContentBox.SetBounds(0, 0, parent.Width, ScaleToDpi(110), BoundsSpecified.Size);
            ArrangeFirstControl(ContentBox, 4, 4);
            ArrangeCommonButtonsR(ButtonApply, ButtonClose, ContentBox, 0, 3);
            ArrangeControlYL(LabelCounter, ContentBox);
            CenterControlY(LabelCounter, ButtonApply);
            parent.OnExpandableVisibleChanged(true);
            InitWindowSize(ButtonClose, 3, 3);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ButtonApply_Click(null, null);
            }

            base.OnKeyDown(e);
        }

        protected override void OnClosed()
        {
            parent.OnExpandableVisibleChanged(false);
            parent.Focus();
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            DialogEndResult = true;
            Close();
        }
    }

    public new string Text
    {
        get => base.Text;
        set
        {
            base.Text = value;

            if (expandable && Child != null && !Child.IsDisposed)
            {
                Child.Content = value;
            }
        }
    }

    public PreferredColors PreferredColors { get; set; }

    public event EventHandler<bool> ExpandableVisibleChanged;

    private AppForm ParentForm;
    private PlainButton ButtonExpand;
    private TextBoxFlyout Child;
    private ThemeHelper themeHelper;
    private readonly bool expandable;
    private readonly Debouncer debouncer;
    private readonly ControlDebounceHelper debounceHelper;

    public PlainTextBox(bool isExpandable)
    {
        MaxLength = ConfigValidator.MaxCustomTextLength;
        debounceHelper = new(this);

        if (expandable = isExpandable)
        {
            this.AddControls(b =>
            [
                ButtonExpand = b.Button("..", 18, 20, (_, _) =>
                {
                    Child = new(this);
                    ParentForm.LocationChanged += OnParentLocationChanged;

                    Child.WhenEnd(e =>
                    {
                        if (e.Result == true)
                        {
                            Text = Child.Content;
                        }

                        ParentForm.LocationChanged -= OnParentLocationChanged;
                    });

                    Child.Show(ParentForm);
                }).With(x =>
                {
                    x.Cursor = Cursors.Arrow;
                    x.Dock = DockStyle.Right;
                })
            ]);
        }

        debouncer = new(base.OnTextChanged);
    }

    public void Input(int totalLength, string text)
    {
        if (totalLength <= MaxLength)
        {
            SelectedText = text;
        }
    }

    public void InputFlyout(string text)
    {
        Child?.Input(text);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (expandable)
        {
            UpdateExpandButtonMargin();
        }

        themeHelper ??= new(this);
        ParentForm = this.FindParentForm();
    }

    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        UpdateExpandButtonMargin();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        e.SuppressKeyPress = e.KeyCode is Keys.Enter or Keys.Space;
        base.OnKeyDown(e);
    }

    protected override void OnTextChanged(EventArgs e)
    {
        if (!debounceHelper.ShouldDebounce)
        {
            base.OnTextChanged(e);
            return;
        }

        debouncer.Debounce(e);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM.PASTE)
        {
            if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText().Clean();
                Input(text.Length + Text.Length, text);
            }

            return;
        }

        base.WndProc(ref m);
    }

    protected override void Dispose(bool disposing)
    {
        themeHelper.Destroy();
        debouncer.Destroy();
        base.Dispose(disposing);
    }

    private void OnParentLocationChanged(object sender, EventArgs e)
    {
        Child.Location = this.LocationToScreen(-4, -4);
    }

    private void UpdateExpandButtonMargin()
    {
        if (expandable)
        {
            /*

            TextBox 留白参考：

            c# - TextBox String/Text's Padding For Custom Control - Stack Overflow
            https://stackoverflow.com/a/38450341

            */

            Win32UI.SendMessage(Handle, NativeConstants.EM_SETMARGINS, NativeConstants.EC_RIGHTMARGIN, int.MakeLong(0, ButtonExpand.Width));
        }
    }

    private void OnExpandableVisibleChanged(bool visible)
    {
        ExpandableVisibleChanged?.Invoke(this, visible);
    }

    void IThemeAware.UpdateTheme(bool useDark, bool init)
    {
        var pcs = PreferredColors;

        if (pcs != null)
        {
            var cp = useDark ? pcs.Dark : pcs.Light;
            ForeColor = cp.Fore;
            BackColor = cp.Back;
        }
        else
        {
            ForeColor = useDark ? Colors.DarkForeText : SystemColors.WindowText;
            BackColor = useDark ? Colors.DarkBackText : SystemColors.Window;
        }

        ThemeManager.EnableDarkModeForControl(this, useDark ? SystemStyle.CfdDark : SystemStyle.Cfd, true);
    }
}
