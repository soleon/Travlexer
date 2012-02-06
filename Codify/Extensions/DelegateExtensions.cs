using System;
using System.ComponentModel;
using System.Windows.Input;
using Codify.Commands;

namespace Codify.Extensions
{
	public static class DelegateExtensions
	{
		public static void ExecuteIfNotNull(this PropertyChangedEventHandler handler, object sender, PropertyChangedEventArgs e)
		{
			if (handler == null)
			{
				return;
			}
			handler(sender, e);
		}

		public static void ExecuteIfNotNull(this ICommand command, object parameter = null)
		{
			if (command == null)
			{
				return;
			}
			command.Execute(parameter);
		}

		public static void ExecuteIfNotNull<T>(this DelegateCommand<T> command, T parameter)
		{
			if (command == null)
			{
				return;
			}
			command.Execute(parameter);
		}

		public static void ExecuteIfNotNull(this Action action)
		{
			if (action == null)
			{
				return;
			}
			action();
		}

		public static void ExecuteIfNotNull(this EventHandler handler, object sender, EventArgs e)
		{
			if (handler == null)
			{
				return;
			}
			handler(sender, e);
		}

		public static void ExecuteIfNotNull<T>(this Action<T> action, T parameter)
		{
			if (action == null)
			{
				return;
			}
			action(parameter);
		}

		public static void ExecuteIfNotNull<T1, T2>(this Action<T1, T2> action, T1 param1, T2 param2)
		{
			if (action == null)
			{
				return;
			}
			action(param1, param2);
		}

		public static TResult ExecuteIfNotNull<TResult>(this Func<TResult> function)
		{
			return function == null ? default(TResult) : function();
		}

		public static TResult ExecuteIfNotNull<T, TResult>(this Func<T, TResult> function, T param, TResult defaultResult = default(TResult))
		{
			return function == null ? defaultResult : function(param);
		}

		public static TResult ExecuteIfNotNull<T1, T2, TResult>(this Func<T1, T2, TResult> function, T1 param1, T2 param2, TResult defaultResult = default(TResult))
		{
			if (function == null)
			{
				return defaultResult;
			}
			return function(param1, param2);
		}

		public static void ExecuteIfNotNull<T>(this T obj, Action<T> action)where T:class
		{
			if (obj == null || action ==null)
			{
				return;
			}
			action(obj);
		}
	}
}
