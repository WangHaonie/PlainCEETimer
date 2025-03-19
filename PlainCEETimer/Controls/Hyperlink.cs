using System.Diagnostics;
using System.Windows.Forms;

namespace PlainCEETimer.Controls
{
    public sealed class Hyperlink : LinkLabel
    {
        public string HyperLink { get; set; }

        protected sealed override void OnLinkClicked(LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(HyperLink);
            base.OnLinkClicked(e);
        }
    }
}
