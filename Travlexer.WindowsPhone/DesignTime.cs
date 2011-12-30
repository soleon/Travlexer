using System.ComponentModel;
using Travlexer.WindowsPhone.Models;
using Travlexer.WindowsPhone.Services.GoogleMaps;
using Travlexer.WindowsPhone.ViewModels;
using Location = Travlexer.WindowsPhone.Models.Location;
using Place = Travlexer.WindowsPhone.Models.Place;

namespace Travlexer.WindowsPhone
{
	public class DesignTime
	{
#if DEBUG
		public Place UserPin
		{
			get
			{
				if (!DesignerProperties.IsInDesignTool)
				{
					return null;
				}
				var data = Globals.DataContext = new DataContext(new GoogleMapsClientMock());
				return _userPin ?? (_userPin = data.AddNewPlace(new Location(9.1540930, -1.39166990), PlaceIcon.Fuel));
			}
		}
		private Place _userPin;

		public MapViewModel MapViewModel
		{
			get
			{
				if (!DesignerProperties.IsInDesignTool)
				{
					return null;
				}
				var data = Globals.DataContext = new DataContext(new GoogleMapsClientMock());
				data.AddNewPlace(new Location(9.1540930, -1.39166990));
				return _mapViewModel ?? (_mapViewModel = new MapViewModel());
			}
		}
		private MapViewModel _mapViewModel;
#endif
	}
}
