using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Travlexer.WindowsPhone.Infrastructure.Models;

namespace Travlexer.WindowsPhone.Converters
{
	public class PlaceIconToBrushConverter : IValueConverter
	{
		private static ResourceDictionary _resources;
		private static ImageBrush _brushIconLocation;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is PlaceIcon))
			{
				return null;
			}
			if (_resources == null)
			{
				_resources = Application.Current.Resources;
			}
			switch ((PlaceIcon) value)
			{
					//case PlaceIcon.Recreation:
					//    break;
					//case PlaceIcon.Vehicle:
					//    break;
					//case PlaceIcon.Drink:
					//    break;
					//case PlaceIcon.Fuel:
					//    break;
					//case PlaceIcon.Property:
					//    break;
					//case PlaceIcon.Restaurant:
					//    break;
					//case PlaceIcon.Shop:
					//    break;
				default:
					return _brushIconLocation ?? (_brushIconLocation = (ImageBrush) _resources["BrushIconLocation"]);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
