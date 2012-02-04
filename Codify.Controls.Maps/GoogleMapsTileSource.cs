using System;
using System.Collections.Generic;
using Microsoft.Phone.Controls.Maps;

namespace Codify.Controls.Maps
{
	public class GoogleMapsTileSource : TileSource
	{
		#region Private Members

		const int XModulus = 2;
		const int YModulus = 2;
		private static readonly string[][] _subDomains = new[] { new[] { "0", "2" }, new[] { "1", "3" } };

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
			var subDomain = _subDomains[x % XModulus][y % YModulus];
			var layer = _layersMapping[Layer];
			var uri = new Uri("http://mt" + subDomain + ".google.com/vt/lyrs=" + layer + "&x=" + x + "&y=" + y + "&z=" + zoomLevel);
			return uri;
		}

		#endregion
	}
}
