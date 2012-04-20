using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Travlexer.Data;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Converters
{
	public class RouteModeKeyValueIconConverter : IValueConverter
	{
		public static List<KeyValueIcon<TravelMode, string, ImageBrush>> TravelModes
		{
			get
			{
				if (_resources == null)
				{
					_resources = Application.Current.Resources;
				}
				return _routeMethods ?? (_routeMethods = new List<KeyValueIcon<TravelMode, string, ImageBrush>>
				{
					new KeyValueIcon<TravelMode, string, ImageBrush>
					{
						Key = TravelMode.Driving,
						Value = "Driving",
						Icon = (ImageBrush)_resources["BrushIconCarForeground"]
					},
					new KeyValueIcon<TravelMode, string, ImageBrush>
					{
						Key = TravelMode.Walking,
						Value = "Walking",
						Icon = (ImageBrush)_resources["BrushIconWalk"]
					},
					new KeyValueIcon<TravelMode, string, ImageBrush>
					{
						Key = TravelMode.Bicycling,
						Value = "Bicycling",
						Icon = (ImageBrush)_resources["BrushIconBicycle"]
					}
				});
			}
		}
		private static List<KeyValueIcon<TravelMode, string, ImageBrush>> _routeMethods;

		private static ResourceDictionary _resources;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is TravelMode))
			{
				return null;
			}
			var method = (TravelMode)value;
			return TravelModes.First(kvi => kvi.Key == method);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is KeyValueIcon<TravelMode, string, ImageBrush>))
			{
				return null;
			}
			var target = (KeyValueIcon<TravelMode, string, ImageBrush>)value;
			return TravelModes.First(kvi => kvi.Key == target.Key).Key;
		}
	}
}