using System;
using System.Globalization;
using System.Windows.Data;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Converters
{
	public class PushpinHighlightStateToOpacityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(!(value is PushpinHighlightStates)){
				return null;
			}
			var state = (PushpinHighlightStates)value;
			if (state == PushpinHighlightStates.None || state == PushpinHighlightStates.Highlighted)
			{
				return 1D;
			}
			return 0.3D;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}