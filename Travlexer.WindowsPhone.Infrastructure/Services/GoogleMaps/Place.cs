using System.Collections.Generic;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Infrastructure.Services.GoogleMaps
{
	public class Place
	{
		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "icon")]
		public string Icon { get; set; }

		[JsonProperty(PropertyName = "vicinity")]
		public string Vicinity { get; set; }

		[JsonProperty(PropertyName = "types")]
		public IList<string> Types { get; set; }

		[JsonProperty(PropertyName = "geometry")]
		public Geometry Geometry { get; set; }

		[JsonProperty(PropertyName = "reference")]
		public string Reference { get; set; }

		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }

		public static implicit operator Models.Place(Place place)
		{
			if (place == null)
			{
				return null;
			}
			return new Models.Place(place.Geometry.Location, name: place.Name)
			{
				ViewPort = place.Geometry.ViewPort,
				Reference = place.Reference
			};
		}
	}
}