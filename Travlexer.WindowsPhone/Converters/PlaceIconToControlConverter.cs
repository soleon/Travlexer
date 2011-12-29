using System;
using System.Globalization;
using System.Windows.Data;
using Travlexer.WindowsPhone.Models;
using Location = Travlexer.WindowsPhone.Controls.Icons.Location;

namespace Travlexer.WindowsPhone.Converters
{
	public class PlaceIconToControlConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is PlaceIcon))
			{
				return null;
			}
			switch ((PlaceIcon) value)
			{
				case PlaceIcon.General:
					return new Location();
				case PlaceIcon.Recreation:
					return new Location();
				case PlaceIcon.Vehicle:
					return new Location();
				case PlaceIcon.Drink:
					return new Location();
				case PlaceIcon.Fuel:
					return new Location();
				case PlaceIcon.Property:
					return new Location();
				case PlaceIcon.Restaurant:
					return new Location();
				case PlaceIcon.Shop:
					return new Location();
				default:
					return new Location();
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
