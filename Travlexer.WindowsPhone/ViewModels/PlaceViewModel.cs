using Codify.ViewModels;
using Travlexer.Data;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class PlaceViewModel : DataViewModel<Place, MapViewModel>
    {
        public PlaceViewModel(Place data, MapViewModel parent) : base(data, parent) {}
    }
}