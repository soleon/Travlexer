using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Infrastructure.Services.GoogleMaps
{
	public class Polyline
	{
		[JsonProperty(PropertyName = "points")]
		public string Points { get; set; }

		[JsonProperty(PropertyName = "levels")]
		public string Levels { get; set; }
	}
}