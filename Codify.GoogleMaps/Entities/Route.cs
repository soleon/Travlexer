using System.Collections.Generic;
using Newtonsoft.Json;

namespace Codify.GoogleMaps.Entities
{
	public class Route
	{
		[JsonProperty(PropertyName = "copyrights")]
		public string Copyrights { get; set; }

		[JsonProperty(PropertyName = "summary")]
		public string Summary { get; set; }

		[JsonProperty(PropertyName = "bounds")]
		public ViewPort Bounds { get; set; }

		[JsonProperty(PropertyName = "warnings")]
		public IList<string> Warnings { get; set; }

		[JsonProperty(PropertyName = "waypoint_order")]
		public IList<int> WaypointOrder { get; set; }

		[JsonProperty(PropertyName = "legs")]
		public IList<Leg> Legs { get; set; }

		[JsonProperty(PropertyName = "overview_polyline")]
		public Polyline OverviewPolyline { get; set; }
	}
}