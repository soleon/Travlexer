using Microsoft.Phone.Controls.Maps;

namespace Travlexer.WindowsPhone.Models
{
	public class ViewPort
	{
		#region Public Properties

		public Location Northeast { get; set; }

		public Location Southwest { get; set; }

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
			return new Services.GoogleMaps.ViewPort
			{
				Northeast = rect.Northeast,
				Southwest = rect.Southwest
			};
		}

		#endregion
	}
}
