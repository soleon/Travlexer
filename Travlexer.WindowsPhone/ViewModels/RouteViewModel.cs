using Codify.ViewModels;
using Travlexer.Data;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class RouteViewModel : DataViewModel<Route, MapViewModel>
    {
        public RouteViewModel(Route data, MapViewModel parent) : base(data, parent) {}
    }
}