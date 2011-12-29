using System.Windows.Input;
using Microsoft.Phone.Controls.Maps;
using Travlexer.WindowsPhone.Extensions;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Views
{
	public partial class MapView
	{
		private MapViewModel _context;

		public MapView()
		{
			InitializeComponent();
			_context = DataContext as MapViewModel;
		}

		private void MapHold(object sender, GestureEventArgs e)
		{
			var map = sender as Map;
			var coordinate = map.ViewportPointToLocation(e.GetPosition(map));
			_context.CommandAddUserPin.ExecuteIfNotNull(coordinate);
		}
	}
}
