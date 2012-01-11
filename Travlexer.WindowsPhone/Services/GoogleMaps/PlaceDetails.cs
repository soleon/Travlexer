using System.Collections.Generic;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Services.GoogleMaps
{
	public class PlaceDetails : Place
	{
		#region Public Properties

		[JsonProperty(PropertyName = "formatted_phone_number")]
		public string FormattedPhoneNumber { get; set; }

		[JsonProperty(PropertyName = "formatted_address")]
		public string FormattedAddress { get; set; }

		[JsonProperty(PropertyName = "address_components")]
		public IList<AddressComponent> AddressComponents { get; set; }

		#endregion


		#region Operators

		public static implicit operator Models.Place(PlaceDetails details)
		{
			return new Models.Place(details.Geometry.Location, name: details.Name)
			{
				Details = new Models.PlaceDetails { ContactNumber = details.FormattedPhoneNumber },
				FormattedAddress = details.FormattedAddress,
				ViewPort = details.Geometry.ViewPort,
				Reference = details.Reference
			};
		}

		#endregion
	}
}
