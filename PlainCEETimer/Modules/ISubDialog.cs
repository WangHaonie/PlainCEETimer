using PlainCEETimer.Controls;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public interface ISubDialog<TData, TSubDialog>
        where TData : IListViewObject<TData>
        where TSubDialog : AppDialog, ISubDialog<TData, TSubDialog>
    {
        TData Data { get; set; }
        DialogResult ShowDialog();
    }
}
