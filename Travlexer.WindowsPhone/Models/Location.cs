using System;
using System.Device.Location;
using System.Globalization;

namespace Travlexer.WindowsPhone.Models
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


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Location"/> class.
		/// </summary>
		public Location(double latitude, double longitude)
		{
			if (latitude < -90D || latitude > 90D)
			{
				throw new ArgumentOutOfRangeException("latitude", "Latitude is out of range. Valid latitude is between -90 and 90.");
			}
			if (longitude < -180D || longitude > 180D)
			{
				throw new ArgumentOutOfRangeException("longitude", "Longitude is out of range. Valid latitude is between -180 and 180.");
			}
			Latitude = latitude;
			Longitude = longitude;
		}

		#endregion


		#region Public Properties

		public double Latitude { get; private set; }
		public double Longitude { get; private set; }

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
			return new Location(coordinate.Latitude, coordinate.Longitude);
		}

		#endregion
	}
}
