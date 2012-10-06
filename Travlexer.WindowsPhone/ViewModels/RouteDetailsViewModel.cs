using System.Collections.Generic;
using System.Linq;
using Codify.Commands;
using Codify.Extensions;
using Travlexer.Data;
using Codify.ViewModels;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class RouteDetailsViewModel : DataViewModel<Route>
    {
        public RouteDetailsViewModel()
        {
            var route = Data = ApplicationContext.Data.SelectedRoute.Value;

            ApplicationContext.Data.Places.ForEach(
                p =>
                {
                    if (p.Id == route.DeparturePlaceId) DeparturePlace = p;
                    else if (p.Id == route.ArrivalPlaceId) ArrivalPlace = p;
                });

            uint index = 1;
            Steps = route.Steps.Select(step => new RouteStepSummaryViewModel(index++, step));

            CommandGoToStep = new DelegateCommand<RouteStep>(step => SelectedStep = step);
        }

        public IEnumerable<RouteStepSummaryViewModel> Steps { get; private set; }

        public RouteStep SelectedStep
        {
            get { return _selectedStep; }
            set { SetValue(ref _selectedStep, value, SelectedStepProperty); }
        }

        private RouteStep _selectedStep;
        private const string SelectedStepProperty = "SelectedStep";

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

        public DelegateCommand<RouteStep> CommandGoToStep { get; private set; }
    }
    public class RouteStepSummaryViewModel : DataViewModel<RouteStep>
    {
        public RouteStepSummaryViewModel(uint index, RouteStep step)
        {
            Index = index;
            Data = step;
            Distance = step.Distance.ToDistanceText();
        }

        public uint Index { get; private set; }

        public string Distance { get; private set; }
    }
}