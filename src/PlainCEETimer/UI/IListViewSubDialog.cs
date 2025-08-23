using System.Windows.Forms;

namespace PlainCEETimer.UI
{
    public interface IListViewSubDialog<T>
        where T : IListViewData<T>
    {
        T Data { get; set; }
        DialogResult ShowDialog();
    }
}
