using System.Windows.Forms;

namespace PlainCEETimer.UI;

public interface IListViewSubDialog<out T>
    where T : IListViewData<T>
{
    T Data { get; }

    DialogResult ShowDialog();
}
