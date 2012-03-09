using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace IISExpressProxy
{
	class Cmd : ICommand
	{
		private readonly Action<object> _execute;
		private readonly Func<object, bool> _canExecute;
		public Cmd(Action<object> executeAction, Func<object, bool> canExecute)
		{
			_execute = executeAction;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			_execute(parameter);
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}
}
