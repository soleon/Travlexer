using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Codify.Extensions;

namespace Codify.Threading
{
    public class Task
    {
        #region Private Members

        private int _count;
        private Action _callback;
        private readonly List<Action> _actions = new List<Action>();

        #endregion


        #region Public Methods

        /// <summary>
        ///     Registers the specified async action in this task.
        /// </summary>
        /// <typeparam name="T">The type of the callback parameter.</typeparam>
        /// <param name="asyncAction">The async action to perform in this task.</param>
        /// <param name="callback">The callback to be executed when the async action is finished.</param>
        /// <returns>
        ///     The <see cref="Task" /> that owns the registered async action.
        /// </returns>
        public Task Register<T>(Action<Action<T>> asyncAction, Action<T> callback = null)
        {
            Interlocked.Increment(ref _count);
            Action a = () =>
            {
                Action<T> action = t =>
                {
                    callback.ExecuteIfNotNull(t);
                    Interlocked.Decrement(ref _count);
                    if (_count == 0)
                    {
                        _callback.ExecuteIfNotNull();
                    }
                };

                asyncAction(action);
            };
            _actions.Add(a);
            return this;
        }

        /// <summary>
        ///     Starts the execution of all registered async actions in this task.
        /// </summary>
        /// <param name="callback">The final callback after all registered async actions have called back.</param>
        public void Start(Action callback = null)
        {
            _callback = callback;
            foreach (var action in _actions.TakeWhile(a => _count != 0))
            {
                action();
            }
        }

        /// <summary>
        ///     Cancels execution of all registered async actions and executes the final callback.
        /// </summary>
        public void Cancel()
        {
            Interlocked.Exchange(ref _count, 0);
            _callback.ExecuteIfNotNull();
        }

        #endregion
    }
}