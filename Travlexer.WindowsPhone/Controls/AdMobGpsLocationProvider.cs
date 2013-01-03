using Google.AdMob.Ads.WindowsPhone7;

namespace Travlexer.WindowsPhone.Controls
{
    public class AdMobGpsLocationProvider
    {
        public GpsLocation? GpsLocation
        {
            get
            {
                var geoCoordinate = ApplicationContext.GeoCoordinateWatcher.Position.Location;
                if (geoCoordinate == null || geoCoordinate.IsUnknown) return null;
                return new GpsLocation
                {
                    Accuracy = geoCoordinate.HorizontalAccuracy,
                    Latitude = geoCoordinate.Latitude,
                    Longitude = geoCoordinate.Longitude
                };
            }
        }
    }
}