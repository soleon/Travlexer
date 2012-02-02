using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Infrastructure.Services.GoogleMaps
{
	public class Geometry
	{
		public enum LocationTypes
		{
			APPROXIMATE,
			GEOMETRIC_CENTER,
			RANGE_INTERPOLATED,
			ROOFTOP
		}

		[JsonProperty(PropertyName = "location")]
		public LatLng Location { get; set; }

		[JsonProperty(PropertyName = "viewport")]
		public ViewPort ViewPort { get; set; }

		[JsonProperty(PropertyName = "location_type")]
		public LocationTypes LocationType { get; set; }
	}
}