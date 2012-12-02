using System;
using System.Windows.Input;
using Codify.Extensions;

namespace Codify.Commands
{
    /// <summary>
    ///     The DelegateCommand provides an implementation of <see cref="System.Windows.Input.ICommand" />
    ///     that can delegate <see cref="System.Windows.Input.ICommand.CanExecute(object)">CanExecute</see> and
    ///     <see cref="System.Windows.Input.ICommand.Execute(object)">Execute</see> call to listeners with a
    ///     generic parameter type.
    /// </summary>
    /// <typeparam name="TParam">The parameter type</typeparam>
    public class DelegateCommand<TParam> : ICommand
    {
        #region Public Properties

        /// <summary>
        ///     This action will be executed in response to <see cref="System.Windows.Input.ICommand.Execute(object)" /> being invoked.
        /// </summary>
        public Action<TParam> ExecutedAction { get; set; }

        /// <summary>
        ///     This function will be called in response to <see cref="System.Windows.Input.ICommand.CanExecute(object)" /> being invoked.
        /// </summary>
        public Func<TParam, bool> CanExecuteFunction { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        ///     Creates a new instance of DelegateCommand
        /// </summary>
        public DelegateCommand() {}

        /// <summary>
        ///     Creates a new instance of DelegatingCommand
        /// </summary>
        /// <param name="executedAction">The action that will execute</param>
        public DelegateCommand(Action<TParam> executedAction)
        {
            ExecutedAction = executedAction;
        }

        /// <summary>
        ///     Creates a new instance of DelegatingCommand
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
        ///     Raised when the status of the CanExecute method changes
        /// </summary>
        public event EventHandler CanExecuteChanged;

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
        ///     Determines if the command can execute in its current state.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(TParam parameter)
        {
            return CanExecuteFunction == null || CanExecuteFunction(parameter);
        }

        /// <summary>
        ///     Determines if the command can execute in its current state.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return CanExecute(ConvertType(parameter));
        }

        /// <summary>
        ///     Executes the command with the given parameter
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(TParam parameter)
        {
            ExecutedAction.ExecuteIfNotNull(parameter);
        }

        /// <summary>
        ///     Executes the command with the given parameter
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            Execute(ConvertType(parameter));
        }

        /// <summary>
        ///     Raises a notification that the CanExecute function has changed
        /// </summary>
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged.ExecuteIfNotNull(this, EventArgs.Empty);
        }

        #endregion
    }
}