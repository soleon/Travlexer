using Codify.Extensions;
using Travlexer.Data;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class RouteSummaryViewModel : CheckableViewModel<Route>
    {
        public RouteSummaryViewModel(Route data)
        {
            Data = data;
            ApplicationContext.Data.Places.ForEach(
                p =>
                {
                    if (p.Id == data.DeparturePlaceId) DeparturePlace = p;
                    else if (p.Id == data.ArrivalPlaceId) ArrivalPlace = p;
                });
        }

        public Place DeparturePlace { get; private set; }

        public Place ArrivalPlace { get; private set; }

        public string Distance
        {
            get { return Data.Distance.ToDistanceText(); }
        }

        public string Duration
        {
            get { return Data.Duration.ToDurationText(); }
        }
    }
}