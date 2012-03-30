using Codify.Attributes;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Views
{
	[ViewModelType(typeof(PlaceDetailsViewModel))]
	public partial class PlaceDetailsView
	{
		public PlaceDetailsView()
		{
			InitializeComponent();
		}
	}
}
