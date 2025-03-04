using PlainCEETimer.WPF.Base;

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
