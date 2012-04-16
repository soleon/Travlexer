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
        private static ImageBrush
            _brushIconLocation,
            _brushIconRecreation,
            _brushIconVehicle,
            _brushIconDring,
            _brushIconFuel,
            _brushIconProperty,
            _brushIconRestaurant,
            _brushIconShop;

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
            switch ((PlaceIcon)value)
            {
                case PlaceIcon.Recreation:
                    return _brushIconRecreation ?? (_brushIconRecreation = (ImageBrush)_resources["BrushIconLocation"]);
                case PlaceIcon.Vehicle:
                    return _brushIconVehicle ?? (_brushIconVehicle = (ImageBrush)_resources["BrushIconLocation"]);
                case PlaceIcon.Drink:
                    return _brushIconDring ?? (_brushIconDring = (ImageBrush)_resources["BrushIconLocation"]);
                case PlaceIcon.Fuel:
                    return _brushIconFuel ?? (_brushIconFuel = (ImageBrush)_resources["BrushIconLocation"]);
                case PlaceIcon.Property:
                    return _brushIconProperty ?? (_brushIconProperty = (ImageBrush)_resources["BrushIconHome"]);
                case PlaceIcon.Restaurant:
                    return _brushIconRestaurant ?? (_brushIconRestaurant = (ImageBrush)_resources["BrushIconLocation"]);
                case PlaceIcon.Shop:
                    return _brushIconShop ?? (_brushIconShop = (ImageBrush)_resources["BrushIconLocation"]);
                default:
                    return _brushIconLocation ?? (_brushIconLocation = (ImageBrush)_resources["BrushIconLocation"]);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
