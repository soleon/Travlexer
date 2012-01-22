using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Codify.WindowsPhone.Converters
{
	public class VisibilityConverter : IValueConverter
	{
		private readonly BooleanConverter _booleanConverter = new BooleanConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var boolValue = _booleanConverter.Convert(value, targetType, parameter, culture) as bool?;
			return boolValue.HasValue && boolValue.Value ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}