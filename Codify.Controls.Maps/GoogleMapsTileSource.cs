using System;
using System.Collections.Generic;
using Codify.Extensions;
using Microsoft.Phone.Controls.Maps;

namespace Codify.Controls.Maps
{
	public class GoogleMapsTileSource : TileSource
	{
		#region Private Members

		private const int XModulus = 2;
		private const int YModulus = 2;
		private static readonly string[,] _subDomains = new[,] { { "0", "2" }, { "1", "3" } };

		private static readonly Dictionary<GoogleMapsLayer, string> _layersMapping = new Dictionary<GoogleMapsLayer, string>
		{
			{ GoogleMapsLayer.Satellite, "s" },
			{ GoogleMapsLayer.Street, "m" },
			{ GoogleMapsLayer.StreetOverlay, "h" },
			{ GoogleMapsLayer.SatelliteHybrid, "y" },
			{ GoogleMapsLayer.TerrainHybrid, "p" },
			{ GoogleMapsLayer.Terrain, "t" },
			{ GoogleMapsLayer.TrafficOverlay, "traffic" },
			{ GoogleMapsLayer.TransitOverlay, "transit" },
			{ GoogleMapsLayer.WaterOverlay, "r" }
		};

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets or sets the <see cref="GoogleMapsLayer"/> to display for this tile.
		/// </summary>
		public GoogleMapsLayer Layer { get; set; }

		#endregion


		#region Public Methods

		public override Uri GetUri(int x, int y, int zoomLevel)
		{
			var isHandled = TileRequested.ExecuteIfNotNull(new QuadKey(x, y, zoomLevel, Layer), false);
			return isHandled ? null : GetUri(x, y, zoomLevel, Layer);
		}

		public static Uri GetUri(QuadKey key)
		{
			return GetUri(key.X, key.Y, key.ZoomLevel, key.Layer);
		}

		public static Uri GetUri(int x, int y, int z, GoogleMapsLayer layer)
		{
			var subDomain = _subDomains[x % XModulus, y % YModulus];
			var lyr = _layersMapping[layer];
			var lng = GoogleMaps.CurrentLanguageCode;
			var uri = new Uri(Uri.UriSchemeHttp + Uri.SchemeDelimiter + "mt" + subDomain + ".google.com/vt/lyrs=" + lyr + "&x=" + x + "&y=" + y + "&z=" + z + "&hl=" + lng);
			return uri;
		}

		#endregion


		#region Public Events

		public static event Func<QuadKey, bool> TileRequested;

		#endregion
	}
}
