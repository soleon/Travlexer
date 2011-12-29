using System;
using System.Windows.Input;
using Travlexer.WindowsPhone.Extensions;

namespace Travlexer.WindowsPhone.Commands
{
	/// <summary>
	/// The DelegateCommand provides an implementation of <see cref="ICommand"/>
	/// that can delegate <see cref="ICommand.CanExecute"/> and <see cref="ICommand.Execute"/>
	/// calls to the listeners.
	/// </summary>
	public class DelegateCommand : ICommand
	{
		#region Public Properties

		/// <summary>
		/// This action will be executed in response to <see cref="ICommand.Execute"/> being invoked.
		/// </summary>
		public Action ExecutedAction { get; set; }

		/// <summary>
		/// This function will be called in response to <see cref="ICommand.CanExecute"/> being invoked.
		/// </summary>
		public Func<bool> CanExecuteFunction { get; set; }

		#endregion


		#region Constructors

		/// <summary>
		/// Creates a new instance of DelegateCommand
		/// </summary>
		/// <param name="executedAction">The action that will execute</param>
		public DelegateCommand(Action executedAction)
		{
			ExecutedAction = executedAction;
		}

		/// <summary>
		/// Creates a new instance of DelegateCommand
		/// </summary>
		/// <param name="executedAction">The action that will execute</param>
		/// <param name="canExecuteFunction">The function that determines if command can execute</param>
		public DelegateCommand(Action executedAction, Func<bool> canExecuteFunction)
		{
			ExecutedAction = executedAction;
			CanExecuteFunction = canExecuteFunction;
		}

		#endregion


		#region Events

		/// <summary>
		/// Raised when the status of the CanExecute method changes
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add { _canExecuteChanged += value; }
			remove { _canExecuteChanged -= value; }
		}

		private EventHandler _canExecuteChanged;

		#endregion


		#region Public Methods

		/// <summary>
		/// Determines if the comman can execute in its current state
		/// </summary>
		/// <returns></returns>
		public bool CanExecute()
		{
			var canExecute = CanExecuteFunction == null || CanExecuteFunction();

			return canExecute;
		}

		/// <summary>
		/// Determines if the command can execute in its current state
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public bool CanExecute(object parameter)
		{
			return CanExecute();
		}

		/// <summary>
		/// Executes the command
		/// </summary>
		public void Execute()
		{
			ExecutedAction.ExecuteIfNotNull();
		}

		/// <summary>
		/// Executes the command
		/// </summary>
		/// <param name="parameter"></param>
		public void Execute(object parameter)
		{
			Execute();
		}

		/// <summary>
		/// Raises a notification that the CanExecute function has changed
		/// </summary>
		public void NotifyCanExecuteChanged()
		{
			if (_canExecuteChanged != null)
			{
				_canExecuteChanged(this, EventArgs.Empty);
			}
		}

		#endregion
	}

	/// <summary>
	/// The DelegateCommand provides an implementation of <see cref="ICommand" />
	/// that can delegate <see cref="ICommand.CanExecute">CanExecute</see> and
	/// <see cref="ICommand.Execute">Execute</see> call to listeners with a 
	/// generic parameter type.
	/// </summary>
	/// <typeparam name="TParam">The parameter type</typeparam>
	public class DelegateCommand<TParam> : ICommand
	{
		#region Public Properties

		/// <summary>
		/// This action will be executed in response to <see cref="ICommand.Execute"/> being invoked.
		/// </summary>
		public Action<TParam> ExecutedAction { get; set; }

		/// <summary>
		/// This function will be called in response to <see cref="ICommand.CanExecute"/> being invoked.
		/// </summary>
		public Func<TParam, bool> CanExecuteFunction { get; set; }

		#endregion


		#region Constructors

		/// <summary>
		/// Creates a new instance of DelegateCommand
		/// </summary>
		public DelegateCommand() {}

		/// <summary>
		/// Creates a new instance of DelegatingCommand
		/// </summary>
		/// <param name="executedAction">The action that will execute</param>
		public DelegateCommand(Action<TParam> executedAction)
		{
			ExecutedAction = executedAction;
		}

		/// <summary>
		/// Creates a new instance of DelegatingCommand
		/// </summary>
		/// <param name="executedAction">The action that will execute</param>
		/// <param name="canExecuteFunction">The function that determines if command can execute</param>
		public DelegateCommand(Action<TParam> executedAction, Func<TParam, bool> canExecuteFunction)
		{
			ExecutedAction = executedAction;
			CanExecuteFunction = canExecuteFunction;
		}

		#endregion


		#region Events

		/// <summary>
		/// Raised when the status of the CanExecute method changes
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add { _canExecuteChanged += value; }
			remove { _canExecuteChanged -= value; }
		}

		private EventHandler _canExecuteChanged;

		#endregion


		#region Private Methods

		private static TParam ConvertType(object parameter)
		{
			if (parameter is TParam)
			{
				return (TParam) parameter;
			}

			var defaultValue = default(TParam);

			if (parameter != null && defaultValue is IConvertible)
			{
				var p = Convert.ChangeType(parameter, typeof (TParam), null);

				if (p != null)
				{
					return (TParam) p;
				}
			}

			return defaultValue;
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Determines if the command can execute in its current state.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public bool CanExecute(TParam parameter)
		{
			return CanExecuteFunction == null || CanExecuteFunction(parameter);
		}

		/// <summary>
		/// Determines if the command can execute in its current state.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public bool CanExecute(object parameter)
		{
			return CanExecute(ConvertType(parameter));
		}

		/// <summary>
		/// Executes the command with the given parameter
		/// </summary>
		/// <param name="parameter"></param>
		public void Execute(TParam parameter)
		{
			ExecutedAction.ExecuteIfNotNull(parameter);
		}

		/// <summary>
		/// Executes the command with the given parameter
		/// </summary>
		/// <param name="parameter"></param>
		public void Execute(object parameter)
		{
			Execute(ConvertType(parameter));
		}

		/// <summary>
		/// Raises a notification that the CanExecute function has changed
		/// </summary>
		public void NotifyCanExecuteChanged()
		{
			if (_canExecuteChanged != null)
			{
				_canExecuteChanged(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
