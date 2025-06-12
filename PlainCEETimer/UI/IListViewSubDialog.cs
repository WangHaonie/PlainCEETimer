using System;
using System.Windows.Forms;
using PlainCEETimer.Modules;

namespace PlainCEETimer.UI
{
    public interface IListViewSubDialog<T> : IDisposable
        where T : IListViewData<T>
    {
        T Data { get; set; }
        DialogResult ShowDialog();
    }
}
