using System;
using System.Diagnostics;

namespace Travlexer.WindowsPhone.Models
{
	/// <summary>
	/// Represents a geographical location.
	/// </summary>
	[DebuggerDisplay("{Latitude,nq}, {Longitude,nq}")]
	public class Location
	{
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


		#region Operators

		public static implicit operator System.Device.Location.GeoCoordinate(Location location)
		{
			return new System.Device.Location.GeoCoordinate(location.Latitude, location.Longitude);
		}

		public static implicit operator Location(System.Device.Location.GeoCoordinate coordinate)
		{
			return new Location(coordinate.Latitude, coordinate.Longitude);
		}

		#endregion
	}
}
