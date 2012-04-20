using System;
using System.ComponentModel;

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
			return function == null ? defaultResult : function(param1, param2);
		}

		public static TResult ExecuteIfNotNull<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> function, T1 param1, T2 param2, T3 param3, T4 param4, TResult defaultResult = default(TResult))
		{
			return function == null ? defaultResult : function(param1, param2, param3, param4);
		}
	}
}
