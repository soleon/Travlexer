using System;
using System.Collections.Generic;
using Travlexer.Data;

namespace Travlexer.WindowsPhone.Infrastructure
{
    public static class Utilities
    {
        /// <summary>
        ///     Decodes the encoded polyline points string.
        /// </summary>
        /// <remarks>
        ///     Translated from the Original Java code located at http://www.geekyblogger.com/2010/12/decoding-polylines-from-google-maps.html.
        /// </remarks>
        public static List<Location> DecodePolylinePoints(String encodedPoints)
        {
            var poly = new List<Location>();
            var chars = encodedPoints.ToCharArray();
            int index = 0, len = encodedPoints.Length;
            int lat = 0, lng = 0;
            while (index < len)
            {
                int b, shift = 0, result = 0;
                do
                {
                    b = chars[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                var dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lat += dlat;
                shift = 0;
                result = 0;
                do
                {
                    b = chars[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);
                var dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lng += dlng;
                var p = new Location
                {
                    Latitude = lat/1E5,
                    Longitude = lng/1E5
                };
                poly.Add(p);
            }
            return poly;
        }

        /// <summary>
        ///     Decodes the encoded polyline levels string.
        /// </summary>
        /// <remarks>
        ///     Translated from the Original Java code located at http://www.geekyblogger.com/2010/12/decoding-polylines-from-google-maps.html.
        /// </remarks>
        public static List<int> DecodePolylineLevels(string encodedLevels)
        {
            var chars = encodedLevels.ToCharArray();
            int index = 0, len = encodedLevels.Length;
            var levels = new List<int>();
            while (index < len)
            {
                int b;
                do
                {
                    b = chars[index++] - 63;
                    levels.Add(b);
                } while (b >= 0x20);
            }
            return levels;
        }
    }
}