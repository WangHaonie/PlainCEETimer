using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public interface IListViewSubDialog<T>
        where T : IListViewData<T>
    {
        T Data { get; set; }
        DialogResult ShowDialog();
    }
}
