using System;
using System.Windows.Input;
using Codify.Extensions;

namespace Codify.Commands
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
        public event EventHandler CanExecuteChanged;

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
            CanExecuteChanged.ExecuteIfNotNull(this, EventArgs.Empty);
        }

        #endregion
    }
}