using System;
using Microsoft.Phone.Controls.Maps;

namespace Codify.Controls.Maps
{
	public class GoogleMapsTileSource : TileSource
	{
		private static readonly string[][] _subDomains = new[] { new[] { "0", "2" }, new[] { "1", "3" } };

		public override Uri GetUri(int x, int y, int zoomLevel)
		{
			var subDomain = _subDomains[x % 2][y % 2];
			var uri = new Uri("http://mt" + subDomain + ".google.com/vt/lyrs=m&x=" + x + "&y=" + y + "&z=" + zoomLevel);
			return uri;
		}
	}
}
