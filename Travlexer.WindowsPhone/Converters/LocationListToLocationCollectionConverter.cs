using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Codify.Extensions;
using Microsoft.Phone.Controls.Maps;
using Travlexer.Data;
using Travlexer.WindowsPhone.Infrastructure;

namespace Travlexer.WindowsPhone.Converters
{
    public class LocationCollectionToLocationCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IEnumerable<Location>))
            {
                return null;
            }
            var collection = new LocationCollection();
            collection.AddRange(((IEnumerable<Location>) value).Select(l => l.ToGeoCoordinate()));
            return collection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }
}