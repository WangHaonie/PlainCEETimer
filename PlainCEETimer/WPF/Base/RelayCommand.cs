using System;
using System.Windows.Input;

namespace PlainCEETimer.WPF.Base
{
    public class RelayCommand(Action<object> ExecuteField, Func<object, bool> CanExecuteField = null) : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => CanExecuteField?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => ExecuteField(parameter);
    }
}