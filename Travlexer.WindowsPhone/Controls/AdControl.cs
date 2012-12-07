using System.ComponentModel;

namespace Travlexer.WindowsPhone.Controls
{
    public class AdControl : Microsoft.Advertising.Mobile.UI.AdControl
    {
        public AdControl()
        {
            Height = 80;
            ApplicationId =
#if DEBUG
 "test_client" 
#else
 "0831f66c-364b-4a71-b1ab-dde627efbb72"
#endif
;
            AdUnitId =
#if DEBUG
 "Image480_80"
#else
 "108107"
#endif
;
            if (DesignerProperties.IsInDesignTool) return;
            var currentLocation = ApplicationContext.GeoCoordinateWatcher.Position.Location;
            Latitude = currentLocation.Latitude;
            Longitude = currentLocation.Longitude;
        }
    }
}