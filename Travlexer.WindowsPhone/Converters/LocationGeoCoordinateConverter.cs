using System;
using System.Device.Location;
using System.Globalization;
using System.Windows.Data;
using Codify.Extensions;
using Travlexer.Data;
using Travlexer.WindowsPhone.Infrastructure;

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
			return ((Location)value).ToGeoCoordinate();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is GeoCoordinate))
			{
				return null;
			}
			return ((GeoCoordinate)value).ToLocalLocation();
		}
	}
}