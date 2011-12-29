using System.ComponentModel;
using Travlexer.WindowsPhone.Models;
using Travlexer.WindowsPhone.Services.GoogleMaps;
using Travlexer.WindowsPhone.ViewModels;
using Location = Travlexer.WindowsPhone.Models.Location;

namespace Travlexer.WindowsPhone
{
	public class DesignTime
	{
#if DEBUG
		public UserPin UserPin
		{
			get
			{
				if (!DesignerProperties.IsInDesignTool)
				{
					return null;
				}
				return _userPin ?? (_userPin = new UserPin(
					new Location(9.1540930, -1.39166990), name: "Jason's New Place")
					{
						FormattedAddress = "235 South Dowling St, Darlington NSW 2017, Australia"
					});
			}
		}
		private UserPin _userPin;

		public MapViewModel MapViewModel
		{
			get
			{
				if (!DesignerProperties.IsInDesignTool)
				{
					return null;
				}
				var data = Globals.DataContext = new DataContext(new GoogleMapsClientMock());
				data.AddNewUserPin(new Location(9.1540930, -1.39166990), PlaceIcon.Fuel);
				data.AddNewUserPin(new Location(-76.016093664209961, -120.9375), PlaceIcon.Drink);
				return _mapViewModel ?? (_mapViewModel = new MapViewModel());
			}
		}
		private MapViewModel _mapViewModel;
#endif
	}
}
