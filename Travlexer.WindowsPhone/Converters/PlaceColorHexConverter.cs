using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Travlexer.WindowsPhone.Infrustructure.Entities;
namespace Travlexer.WindowsPhone.Converters
{
	public class PlaceColorToHexConverter : IValueConverter
	{
		private static readonly Dictionary<PlaceColor, string> _colors = new Dictionary<PlaceColor, string>
		{
			{ PlaceColor.Blue, "#1BA1E2" },
			{ PlaceColor.Magenta, "#FF0097" },
			{ PlaceColor.Purple, "#A200FF" },
			{ PlaceColor.Teal, "#00ABA9" },
			{ PlaceColor.Lime, "#8CBF26" },
			{ PlaceColor.Brown, "#A05000" },
			{ PlaceColor.Pink, "#E671B8" },
			{ PlaceColor.Orange, "#F09609" },
			{ PlaceColor.Red, "#E51400" },
			{ PlaceColor.Green, "#339933" }
		};

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is PlaceColor))
			{
				return null;
			}
			return _colors[(PlaceColor) value];
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
