using System.Windows.Forms;

namespace PlainCEETimer.UI;

public interface IListViewChildDialog<out T>
    where T : IListViewData<T>
{
    T Data { get; }

    DialogResult ShowDialog();
}
