using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using Travlexer.WindowsPhone.Infrustructure.Entities;

namespace Travlexer.WindowsPhone.Converters
{
	public class PlaceIconToControlConverter : IValueConverter
	{
		private static readonly Dictionary<PlaceIcon, Control> _icons = new Dictionary<PlaceIcon, Control>
		{
			{ PlaceIcon.General, new Controls.Icons.Location() }
		};

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is PlaceIcon))
			{
				return null;
			}
			return _icons[(PlaceIcon)value];
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}