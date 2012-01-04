using System.Collections.Generic;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public class PlaceDetails : Place
	{
		[JsonProperty(PropertyName = "formatted_phone_number")]
		public string FormattedPhoneNumber { get; set; }

		[JsonProperty(PropertyName = "formatted_address")]
		public string FormattedAddress { get; set; }

		[JsonProperty(PropertyName = "address_components")]
		public IList<AddressComponent> AddressComponents { get; set; }
	}
}