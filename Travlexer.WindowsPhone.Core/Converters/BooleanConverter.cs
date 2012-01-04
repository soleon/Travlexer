using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Travlexer.WindowsPhone.Core.Converters
{
	public class BooleanConverter : IValueConverter
	{
		private const string ReverseParameter = "reverse";

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var result = false;
			
			if (value is string)
			{
				result = Convert((string)value);
			}
			else if (value is Visibility)
			{
				result = Convert((Visibility)value);
			}

			if (parameter is string && string.Equals((string)parameter, ReverseParameter, StringComparison.CurrentCultureIgnoreCase))
			{
				result = !result;
			}

			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		private static bool Convert(string value)
		{
			bool result;
			if (bool.TryParse(value, out result))
			{
				return result;
			}
			return !string.IsNullOrEmpty(value);
		}

		private static bool Convert(Visibility value)
		{
			return value != Visibility.Collapsed;
		}
	}
}