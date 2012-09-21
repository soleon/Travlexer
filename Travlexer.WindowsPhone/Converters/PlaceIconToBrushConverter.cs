using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Travlexer.Data;

namespace Travlexer.WindowsPhone.Converters
{
    public class PlaceIconToBrushConverter : IValueConverter
    {
        private static ResourceDictionary _resources;
        private static readonly Dictionary<PlaceIcon, Brush> IconBrushMap = new Dictionary<PlaceIcon, Brush>();

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
                    return IconBrushMap.ContainsKey(icon) ? IconBrushMap[icon] : (IconBrushMap[icon] = (ImageBrush)_resources["BrushIconGolf"]);
                case PlaceIcon.Vehicle:
                    return IconBrushMap.ContainsKey(icon) ? IconBrushMap[icon] : (IconBrushMap[icon] = (ImageBrush)_resources["BrushIconCar"]);
                case PlaceIcon.Drink:
                    return IconBrushMap.ContainsKey(icon) ? IconBrushMap[icon] : (IconBrushMap[icon] = (ImageBrush)_resources["BrushIconDrink"]);
                case PlaceIcon.Fuel:
                    return IconBrushMap.ContainsKey(icon) ? IconBrushMap[icon] : (IconBrushMap[icon] = (ImageBrush)_resources["BrushIconFuel"]);
                case PlaceIcon.Property:
                    return IconBrushMap.ContainsKey(icon) ? IconBrushMap[icon] : (IconBrushMap[icon] = (ImageBrush)_resources["BrushIconHome"]);
                case PlaceIcon.Restaurant:
                    return IconBrushMap.ContainsKey(icon) ? IconBrushMap[icon] : (IconBrushMap[icon] = (ImageBrush)_resources["BrushIconRestaurant"]);
                case PlaceIcon.Shop:
                    return IconBrushMap.ContainsKey(icon) ? IconBrushMap[icon] : (IconBrushMap[icon] = (ImageBrush)_resources["BrushIconShoppingCart"]);
                case PlaceIcon.Airport:
                    return IconBrushMap.ContainsKey(icon) ? IconBrushMap[icon] : (IconBrushMap[icon] = (ImageBrush)_resources["BrushIconAirplane"]);
                case PlaceIcon.PublicTransport:
                    return IconBrushMap.ContainsKey(icon) ? IconBrushMap[icon] : (IconBrushMap[icon] = (ImageBrush)_resources["BrushIconTrain"]);
                case PlaceIcon.Information:
                    return IconBrushMap.ContainsKey(icon) ? IconBrushMap[icon] : (IconBrushMap[icon] = (ImageBrush)_resources["BrushIconInformation"]);
                case PlaceIcon.MoneyExchange:
                    return IconBrushMap.ContainsKey(icon) ? IconBrushMap[icon] : (IconBrushMap[icon] = (ImageBrush)_resources["BrushIconMoney"]);
                case PlaceIcon.Internet:
                    return IconBrushMap.ContainsKey(icon) ? IconBrushMap[icon] : (IconBrushMap[icon] = (ImageBrush)_resources["BrushIconWifi"]);
                case PlaceIcon.Ferry:
                    return IconBrushMap.ContainsKey(icon) ? IconBrushMap[icon] : (IconBrushMap[icon] = (ImageBrush)_resources["BrushIconAnchor"]);
                case PlaceIcon.Casino:
                    return IconBrushMap.ContainsKey(icon) ? IconBrushMap[icon] : (IconBrushMap[icon] = (ImageBrush)_resources["BrushIconDice"]);
                default:
                    return IconBrushMap.ContainsKey(icon) ? IconBrushMap[icon] : (IconBrushMap[icon] = (ImageBrush)_resources["BrushIconPlace"]);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
