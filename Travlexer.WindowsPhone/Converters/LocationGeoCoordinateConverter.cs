using System;
using System.Device.Location;
using System.Globalization;
using System.Windows.Data;
using Travlexer.WindowsPhone.Models;

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
			var location = (Location)value;
			return new GeoCoordinate(location.Latitude, location.Longitude);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is GeoCoordinate))
			{
				return null;
			}
			var coordinate = (GeoCoordinate)value;
			return new Location(coordinate.Latitude, coordinate.Longitude);
		}
	}
}