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
	}
}
