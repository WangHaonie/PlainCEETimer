using System;
using System.Windows.Forms;

namespace PlainCEETimer.Modules
{
    public interface IListViewSubDialog<T> : IDisposable
        where T : IListViewData<T>
    {
        T Data { get; set; }
        DialogResult ShowDialog();
    }
}
