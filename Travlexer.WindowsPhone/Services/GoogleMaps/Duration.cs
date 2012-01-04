using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public struct Duration
	{
		[JsonProperty(PropertyName = "text")]
		public string Text { get; set; }

		[JsonProperty(PropertyName = "value")]
		public int Value { get; set; }
	}
}