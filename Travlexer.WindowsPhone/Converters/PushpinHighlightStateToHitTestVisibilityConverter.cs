using System;
using System.Globalization;
using System.Windows.Data;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Converters
{
	public class PushpinHighlightStateToHitTestVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is PushpinHighlightStates))
			{
				return null;
			}
			var state = (PushpinHighlightStates)value;
			return state == PushpinHighlightStates.None || state == PushpinHighlightStates.Highlighted;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}