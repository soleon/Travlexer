using System.ComponentModel;

namespace Travlexer.Core.Extensions
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
	}
}
