using System;
using System.Device.Location;
using System.Globalization;
using System.Windows.Data;
using Travlexer.WindowsPhone.Infrastructure.Models;

namespace Travlexer.WindowsPhone.Converters
{
	public class LocationGeoCoordinateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Location))
			{
				return null;
			}
			return (GeoCoordinate)(Location)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is GeoCoordinate))
			{
				return null;
			}
			return (Location)(GeoCoordinate)value;
		}
	}
}