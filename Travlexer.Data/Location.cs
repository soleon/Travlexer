using System.Globalization;

namespace Travlexer.Data
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
		/// Returns a culture invariant <see cref="string"/> that represents the geo-coordinate of this location.
		/// </summary>
		public override string ToString()
		{
			return (Latitude.ToString(FormatString, CultureInfo.InvariantCulture) + Delimiter + Longitude.ToString(FormatString, CultureInfo.InvariantCulture));
		}

		public bool Equals(Location other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return other.Latitude.Equals(Latitude) && other.Longitude.Equals(Longitude);
		}

		public override bool Equals(object obj)
		{
			if (obj is Location)
			{
				return Equals(obj as Location);
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return (Latitude.GetHashCode() ^ Longitude.GetHashCode());
		}

		#endregion


        #region Operators

        public static bool operator ==(Location left, Location right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
        }

        public static bool operator !=(Location left, Location right)
        {
            return !(left == right);
        }

        #endregion
	}
}
