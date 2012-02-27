using System;
using System.Collections.Generic;
using System.Linq;
using Codify.GoogleMaps.Entities;
using Codify.Models;

namespace Travlexer.WindowsPhone.Infrastructure.Models
{
	public class Route : ModelBase
	{
		#region Public Properties

		public List<Location> Points { get; set; }

		public string Name
		{
			get { return _name; }
			set { SetProperty(ref _name, value, NameProperty); }
		}

		private string _name;
		private const string NameProperty = "Name";

		public RouteMethod Method { get; set; }

		public TravelMode Mode { get; set; }

		#endregion


		#region Public Methods

		public bool Equals(Route other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			int pointCount;
			return other.Method == Method
				   && other.Mode == Mode
				   && other.Points != null
				   && Points != null
				   && other.Points.Count == (pointCount = Points.Count)
				   && other.Points[0] == Points[0]
				   && other.Points[--pointCount] == Points[pointCount];
		}

		public override bool Equals(object obj)
		{
			if (obj is Route)
			{
				return Equals(obj as Route);
			}
			return base.Equals(obj);
		}

		#endregion


		#region Operators

		public static implicit operator Route(Codify.GoogleMaps.Entities.Route route)
		{
			if (route == null)
			{
				return null;
			}
			var newRoute = new Route();

			var legs = route.Legs;
			if (legs.Any())
			{
				var steps = legs[0].Steps;
				if (steps.Any())
				{
					var points = new List<Location>();
					foreach (var step in steps)
					{
						points.AddRange(DecodePolylinePoints(step.Polyline.Points));
					}
					newRoute.Points = points;
				}
			}

			return newRoute;
		}

		public static bool operator ==(Route left, Route right)
		{
			if (ReferenceEquals(left, null))
			{
				return ReferenceEquals(right, null);
			}
			return left.Equals(right);
		}

		public static bool operator !=(Route left, Route right)
		{
			return !(left == right);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Decodes the encoded polyline points string.
		/// </summary>
		/// <remarks>
		/// Translated from the Original Java code located at http://www.geekyblogger.com/2010/12/decoding-polylines-from-google-maps.html.
		/// </remarks>
		public static List<Location> DecodePolylinePoints(String encodedPoints)
		{
			var poly = new List<Location>();
			var chars = encodedPoints.ToCharArray();
			int index = 0, len = encodedPoints.Length;
			int lat = 0, lng = 0;
			while (index < len)
			{
				int b, shift = 0, result = 0;
				do
				{
					b = chars[index++] - 63;
					result |= (b & 0x1f) << shift;
					shift += 5;
				}
				while (b >= 0x20);
				var dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
				lat += dlat;
				shift = 0;
				result = 0;
				do
				{
					b = chars[index++] - 63;
					result |= (b & 0x1f) << shift;
					shift += 5;
				}
				while (b >= 0x20);
				var dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
				lng += dlng;
				var p = new Location
				{
					Latitude = lat / 1E5,
					Longitude = lng / 1E5
				};
				poly.Add(p);
			}
			return poly;
		}

		/// <summary>
		/// Decodes the encoded polyline levels string.
		/// </summary>
		/// <remarks>
		/// Translated from the Original Java code located at http://www.geekyblogger.com/2010/12/decoding-polylines-from-google-maps.html.
		/// </remarks>
		public static List<int> DecodePolylineLevels(string encodedLevels)
		{
			var chars = encodedLevels.ToCharArray();
			int index = 0, len = encodedLevels.Length;
			var levels = new List<int>();
			while (index < len)
			{
				int b;
				do
				{
					b = chars[index++] - 63;
					levels.Add(b);
				}
				while (b >= 0x20);
			}
			return levels;
		}

		#endregion
	}
}
