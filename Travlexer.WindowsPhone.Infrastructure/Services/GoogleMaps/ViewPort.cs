using Microsoft.Phone.Controls.Maps;
using Newtonsoft.Json;

namespace Travlexer.WindowsPhone.Infrastructure.Services.GoogleMaps
{
	public class ViewPort
	{
		private const string Delimiter = "|";


		#region Public Properties

		[JsonProperty(PropertyName = "northeast")]
		public LatLng Northeast { get; set; }

		[JsonProperty(PropertyName = "southwest")]
		public LatLng Southwest { get; set; }

		#endregion


		#region Operators

		public static implicit operator LocationRect(ViewPort viewPort)
		{
			if (viewPort == null)
			{
				return null;
			}
			return new LocationRect
			{
				Northeast = viewPort.Northeast,
				Southwest = viewPort.Southwest
			};
		}

		public static implicit operator ViewPort(LocationRect rect)
		{
			if (rect == null)
			{
				return null;
			}
			return new ViewPort
			{
				Northeast = rect.Northeast,
				Southwest = rect.Southwest
			};
		}

		public static implicit operator Models.ViewPort(ViewPort viewPort)
		{
			if (viewPort == null)
			{
				return null;
			}
			return new Models.ViewPort
			{
				Northeast = viewPort.Northeast,
				Southwest = viewPort.Southwest
			};
		}

		public static implicit operator ViewPort(Models.ViewPort model)
		{
			if (model == null)
			{
				return null;
			}
			return new ViewPort
			{
				Northeast = model.Northeast,
				Southwest = model.Southwest
			};
		}

		#endregion


		#region Public Methods

		public override string ToString()
		{
			return Southwest + Delimiter + Northeast;
		}

		#endregion
	}
}