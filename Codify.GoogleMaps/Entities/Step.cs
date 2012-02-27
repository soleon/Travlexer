using Newtonsoft.Json;

namespace Codify.GoogleMaps.Entities
{
	public class Step
	{
		[JsonProperty(PropertyName = "distance")]
		public Distance Distance { get; set; }

		[JsonProperty(PropertyName = "duration")]
		public Duration Duration { get; set; }

		[JsonProperty(PropertyName = "end_location")]
		public LatLng EndLocation { get; set; }

		[JsonProperty(PropertyName = "start_location")]
		public LatLng StartLocation { get; set; }

		[JsonProperty(PropertyName = "travel_mode")]
		public TravelMode TravelMode { get; set; }

		[JsonProperty(PropertyName = "html_instructions")]
		public string HtmlInstructions { get; set; }

		[JsonProperty(PropertyName = "polyline")]
		public Polyline Polyline { get; set; }
	}
}