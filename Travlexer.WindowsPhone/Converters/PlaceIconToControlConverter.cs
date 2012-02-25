using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Travlexer.WindowsPhone.Infrastructure.Models;

namespace Travlexer.WindowsPhone.Converters
{
	public class PlaceIconToControlConverter : IValueConverter
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
				_resources = Application.Current.Resources;
			if (_brushIconLocation == null)
			{
				_brushIconLocation = (ImageBrush)_resources["BrushIconLocation"];
			}
			switch ((PlaceIcon)value)
			{
				default:
					return new Rectangle { Fill = _brushIconLocation };
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
