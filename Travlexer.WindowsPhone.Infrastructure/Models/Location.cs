using System.Device.Location;
using System.Globalization;

namespace Travlexer.WindowsPhone.Infrastructure.Models
{
	/// <summary>
	/// Represents a geographical location.
	/// </summary>
	public class Location
	{
		#region Constants

		private const string Delimiter = ", ";
		private const string FormatString = "G";

		#endregion


		#region Public Properties

		public double Latitude { get; set; }

		public double Longitude { get; set; }

		#endregion


		#region Public Methods

		/// <summary>
		/// Returns a culture invariant <see cref="System.String"/> that represents the geo-coordinate of this location.
		/// </summary>
		public override string ToString()
		{
			return (Latitude.ToString(FormatString, CultureInfo.InvariantCulture) + Delimiter + Longitude.ToString(FormatString, CultureInfo.InvariantCulture));
		}

		#endregion


		#region Operators

		public static implicit operator Location(GeoCoordinate coordinate)
		{
			return coordinate == null ? null : new Location { Latitude = coordinate.Latitude, Longitude = coordinate.Longitude };
		}

		public static implicit operator GeoCoordinate(Location location)
		{
			return location == null ? null : new GeoCoordinate(location.Latitude, location.Longitude);
		}

		#endregion
	}
}
