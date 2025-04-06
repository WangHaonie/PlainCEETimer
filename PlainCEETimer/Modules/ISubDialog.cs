using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public interface ISubDialog<TData>
        where TData : IListViewObject<TData>
    {
        TData Data { get; set; }

        DialogResult ShowDialog();
    }
}
