using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Phone.Controls.Maps;
using Travlexer.WindowsPhone.Infrastructure.Models;

namespace Travlexer.WindowsPhone.Converters
{
	public class LocationListToLocationCollectionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is List<Location>))
			{
				return null;
			}
			var list = (List<Location>)value;
			var collection = new LocationCollection();
			foreach (var l in list)
			{
				collection.Add(l);
			}
			return collection;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return new NotSupportedException();
		}
	}
}