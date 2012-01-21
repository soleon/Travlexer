using System.Collections.Generic;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Infrastructure.Services.GoogleMaps
{
	public class Leg
	{
		[JsonProperty(PropertyName = "distance")]
		public Distance Distance { get; set; }

		[JsonProperty(PropertyName = "duration")]
		public Duration Duration { get; set; }

		[JsonProperty(PropertyName = "end_address")]
		public string EndAddress { get; set; }

		[JsonProperty(PropertyName = "end_location")]
		public LatLng EndLocation { get; set; }

		[JsonProperty(PropertyName = "start_address")]
		public string StartAddress { get; set; }

		[JsonProperty(PropertyName = "start_location")]
		public LatLng StartLocation { get; set; }

		[JsonProperty(PropertyName = "steps")]
		public IList<Step> Steps { get; set; }
	}
}