using PlainCEETimer.WPF.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainCEETimer.WPF.ViewModels
{
    public class AppMessageBoxViewModel : ViewModelBase
    {
        public string WindowTitle
        {
            get => field;
            set => SetField(ref field, value);
        }
    }
}
