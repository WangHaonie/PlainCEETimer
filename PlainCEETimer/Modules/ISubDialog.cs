using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public interface ISubDialog<T>
        where T : IListViewObject<T>
    {
        T Data { get; set; }
        DialogResult ShowDialog();
    }
}
