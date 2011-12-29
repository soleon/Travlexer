using System;
using System.ComponentModel;
using Travelexer.WindowsPhone.Core.Commands;

namespace Travelexer.WindowsPhone.Core.Extensions
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

		public static void ExecuteIfNotNull(this DelegateCommand command, object parameter = null)
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
	}
}
