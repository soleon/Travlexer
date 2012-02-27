using Microsoft.Phone.Controls.Maps;

namespace Travlexer.WindowsPhone.Infrastructure.Models
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
			return new Codify.GoogleMaps.Entities.ViewPort
			{
				Northeast = rect.Northeast,
				Southwest = rect.Southwest
			};
		}

		

		public static implicit operator ViewPort(Codify.GoogleMaps.Entities.ViewPort viewPort)
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

		public static implicit operator Codify.GoogleMaps.Entities.ViewPort(ViewPort model)
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
	}
}
