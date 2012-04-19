using System;
using System.Collections.Generic;
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
        private static Dictionary<PlaceIcon, Brush> _iconBrushMap = new Dictionary<PlaceIcon, Brush>();

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
            var icon = (PlaceIcon)value;
            switch (icon)
            {
                case PlaceIcon.Recreation:
                    return _iconBrushMap.ContainsKey(icon) ? _iconBrushMap[icon] : (_iconBrushMap[icon] = (ImageBrush)_resources["BrushIconGolf"]);
                case PlaceIcon.Vehicle:
                    return _iconBrushMap.ContainsKey(icon) ? _iconBrushMap[icon] : (_iconBrushMap[icon] = (ImageBrush)_resources["BrushIconCar"]);
                case PlaceIcon.Drink:
                    return _iconBrushMap.ContainsKey(icon) ? _iconBrushMap[icon] : (_iconBrushMap[icon] = (ImageBrush)_resources["BrushIconDrink"]);
                case PlaceIcon.Fuel:
                    return _iconBrushMap.ContainsKey(icon) ? _iconBrushMap[icon] : (_iconBrushMap[icon] = (ImageBrush)_resources["BrushIconFuel"]);
                case PlaceIcon.Property:
                    return _iconBrushMap.ContainsKey(icon) ? _iconBrushMap[icon] : (_iconBrushMap[icon] = (ImageBrush)_resources["BrushIconHome"]);
                case PlaceIcon.Restaurant:
                    return _iconBrushMap.ContainsKey(icon) ? _iconBrushMap[icon] : (_iconBrushMap[icon] = (ImageBrush)_resources["BrushIconRestaurant"]);
                case PlaceIcon.Shop:
                    return _iconBrushMap.ContainsKey(icon) ? _iconBrushMap[icon] : (_iconBrushMap[icon] = (ImageBrush)_resources["BrushIconShoppingCart"]);
                case PlaceIcon.Airport:
                    return _iconBrushMap.ContainsKey(icon) ? _iconBrushMap[icon] : (_iconBrushMap[icon] = (ImageBrush)_resources["BrushIconAirplane"]);
                case PlaceIcon.PublicTransport:
                    return _iconBrushMap.ContainsKey(icon) ? _iconBrushMap[icon] : (_iconBrushMap[icon] = (ImageBrush)_resources["BrushIconTrain"]);
                case PlaceIcon.Information:
                    return _iconBrushMap.ContainsKey(icon) ? _iconBrushMap[icon] : (_iconBrushMap[icon] = (ImageBrush)_resources["BrushIconInformation"]);
                case PlaceIcon.MoneyExchange:
                    return _iconBrushMap.ContainsKey(icon) ? _iconBrushMap[icon] : (_iconBrushMap[icon] = (ImageBrush)_resources["BrushIconMoney"]);
                case PlaceIcon.Internet:
                    return _iconBrushMap.ContainsKey(icon) ? _iconBrushMap[icon] : (_iconBrushMap[icon] = (ImageBrush)_resources["BrushIconWifi"]);
                case PlaceIcon.Ferry:
                    return _iconBrushMap.ContainsKey(icon) ? _iconBrushMap[icon] : (_iconBrushMap[icon] = (ImageBrush)_resources["BrushIconAnchor"]);
                case PlaceIcon.Casino:
                    return _iconBrushMap.ContainsKey(icon) ? _iconBrushMap[icon] : (_iconBrushMap[icon] = (ImageBrush)_resources["BrushIconDice"]);
                default:
                    return _iconBrushMap.ContainsKey(icon) ? _iconBrushMap[icon] : (_iconBrushMap[icon] = (ImageBrush)_resources["BrushIconLocation"]);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
