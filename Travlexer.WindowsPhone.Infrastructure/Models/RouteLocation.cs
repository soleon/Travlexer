using System;
using Codify.Models;

namespace Travlexer.WindowsPhone.Infrastructure.Models
{
	/// <summary>
	/// Represents either the departure or arrival location for a route.
	/// </summary>
	public class RouteLocation : ModelBase
	{
		#region Constructor

		public RouteLocation(Guid placeId, string address)
		{
			PlaceId = placeId;
			_address = address;
		}

		public RouteLocation() {}

		#endregion


		#region Public Properties

		public Guid PlaceId { get; set; }

		public string Address
		{
			get { return _address; }
			set
			{
				if (SetProperty(ref _address, value, AddressProperty))
				{
					PlaceId = Guid.Empty;
				}
			}
		}

		private string _address;
		private const string AddressProperty = "Address";

		#endregion
	}
}
