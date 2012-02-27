using System;
using System.Collections.Generic;

namespace Codify.GoogleMaps.Controls
{
	public class TileSource : Microsoft.Phone.Controls.Maps.TileSource
	{
		#region Private Members

		private const int XModulus = 2;
		private const int YModulus = 2;
		private static readonly string[,] _subDomains = new[,] { { "0", "2" }, { "1", "3" } };

		private static readonly Dictionary<Layer, string> _layersMapping = new Dictionary<Layer, string>
		{
			{ Layer.Satellite, "s" },
			{ Layer.Street, "m" },
			{ Layer.StreetOverlay, "h" },
			{ Layer.SatelliteHybrid, "y" },
			{ Layer.TerrainHybrid, "p" },
			{ Layer.Terrain, "t" },
			{ Layer.TrafficOverlay, "traffic" },
			{ Layer.TransitOverlay, "transit" },
			{ Layer.WaterOverlay, "r" }
		};

		#endregion


		#region Public Properties

		/// <summary>
		/// Gets or sets the <see cref="Controls.Layer"/> to display for this tile.
		/// </summary>
		public Layer Layer { get; set; }

		#endregion


		#region Public Methods

		public override Uri GetUri(int x, int y, int zoomLevel)
		{
			return GetUri(x, y, zoomLevel, Layer);
		}

		public static Uri GetUri(QuadKey key)
		{
			return GetUri(key.X, key.Y, key.ZoomLevel, key.Layer);
		}

		public static Uri GetUri(int x, int y, int z, Layer layer)
		{
			var subDomain = _subDomains[x % XModulus, y % YModulus];
			var lyr = _layersMapping[layer];
			var lng = Utilities.CurrentLanguageCode;
			var uri = new Uri(Uri.UriSchemeHttp + Uri.SchemeDelimiter + "mt" + subDomain + ".google.com/vt/lyrs=" + lyr + "&x=" + x + "&y=" + y + "&z=" + z + "&hl=" + lng);
			return uri;
		}

		#endregion
	}
}
