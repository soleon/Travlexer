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
    public class RouteMethodKeyValueIconConverter : IValueConverter
    {
        public static List<KeyValueIcon<RouteMethod, string, ImageBrush>> RouteMethods
        {
            get
            {
                if (_resources == null)
                {
                    _resources = Application.Current.Resources;
                }
                return _routeMethods ?? (_routeMethods = new List<KeyValueIcon<RouteMethod, string, ImageBrush>>
                {
                    new KeyValueIcon<RouteMethod, string, ImageBrush>
                    {
                        Key = RouteMethod.Default,
                        Value = "Fastest Route",
                        Icon = (ImageBrush) _resources["BrushIconLightning"]
                    },
                    new KeyValueIcon<RouteMethod, string, ImageBrush>
                    {
                        Key = RouteMethod.AvoidTolls,
                        Value = "Avoid Tolls",
                        Icon = (ImageBrush) _resources["BrushIconDollar"]
                    },
                    new KeyValueIcon<RouteMethod, string, ImageBrush>
                    {
                        Key = RouteMethod.AvoidHighways,
                        Value = "Avoid Highways",
                        Icon = (ImageBrush) _resources["BrushIconHighway"]
                    }
                });
            }
        }

        private static List<KeyValueIcon<RouteMethod, string, ImageBrush>> _routeMethods;

        private static ResourceDictionary _resources;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is RouteMethod))
            {
                return null;
            }
            var method = (RouteMethod) value;
            return RouteMethods.First(kvi => kvi.Key == method);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is KeyValueIcon<RouteMethod, string, ImageBrush>))
            {
                return null;
            }
            var target = (KeyValueIcon<RouteMethod, string, ImageBrush>) value;
            return RouteMethods.First(kvi => kvi.Key == target.Key).Key;
        }
    }
}