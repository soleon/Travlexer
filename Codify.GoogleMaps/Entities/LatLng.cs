using System.Device.Location;
using System.Globalization;
using Newtonsoft.Json;

namespace Codify.GoogleMaps.Entities
{
	public class LatLng
	{
		#region Constants

		private const string Delimiter = ",";

		#endregion


		#region Public Properties

		[JsonProperty(PropertyName = "lat")]
		public double Lat { get; set; }

		[JsonProperty(PropertyName = "lng")]
		public double Lng { get; set; }

		#endregion


		#region Public Methods

		public override string ToString()
		{
			return Lat.ToString(NumberFormatInfo.InvariantInfo) + Delimiter + Lng.ToString(NumberFormatInfo.InvariantInfo);
		}

		#endregion


		#region Operators

		public static implicit operator GeoCoordinate(LatLng latLng)
		{
			return latLng == null ? null : new GeoCoordinate(latLng.Lat, latLng.Lng);
		}

		public static implicit operator LatLng(GeoCoordinate coordinate)
		{
			return coordinate == null ? null : new LatLng { Lat = coordinate.Latitude, Lng = coordinate.Longitude };
		}

		#endregion
	}
}