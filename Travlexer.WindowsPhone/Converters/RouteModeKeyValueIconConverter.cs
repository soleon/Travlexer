using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Travlexer.WindowsPhone.Infrastructure.Models;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Converters
{
	public class RouteModeKeyValueIconConverter : IValueConverter
	{
		public static List<KeyValueIcon<RouteMode, string, ImageBrush>> RouteModes
		{
			get
			{
				if (_resources == null)
				{
					_resources = Application.Current.Resources;
				}
				return _routeMethods ?? (_routeMethods = new List<KeyValueIcon<RouteMode, string, ImageBrush>>
				{
					new KeyValueIcon<RouteMode, string, ImageBrush>
					{
						Key = RouteMode.Driving,
						Value = "Driving",
						Icon = (ImageBrush)_resources["BrushIconCar"]
					},
					new KeyValueIcon<RouteMode, string, ImageBrush>
					{
						Key = RouteMode.Walking,
						Value = "Walking",
						Icon = (ImageBrush)_resources["BrushIconWalk"]
					},
					new KeyValueIcon<RouteMode, string, ImageBrush>
					{
						Key = RouteMode.Bicycling,
						Value = "Bicycling",
						Icon = (ImageBrush)_resources["BrushIconBicycle"]
					}
				});
			}
		}
		private static List<KeyValueIcon<RouteMode, string, ImageBrush>> _routeMethods;

		private static ResourceDictionary _resources;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is RouteMode))
			{
				return null;
			}
			var method = (RouteMode)value;
			return RouteModes.First(kvi => kvi.Key == method);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is KeyValueIcon<RouteMode, string, ImageBrush>))
			{
				return null;
			}
			var target = (KeyValueIcon<RouteMode, string, ImageBrush>)value;
			return RouteModes.First(kvi => kvi.Key == target.Key).Key;
		}
	}
}