using System;
using System.Windows.Input;

namespace Panini.Commands
{
    class ChartCommandHandler<T> : ICommand where T : class
    {
        public Predicate<T> CanExecuteDelegate { get; set; }
        public Action<T> ExecuteDelegate { get; set; }

        public bool CanExecute(object parameter)
        {
            return CanExecuteDelegate == null || CanExecuteDelegate((T)parameter);
        }

        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null) ExecuteDelegate((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
