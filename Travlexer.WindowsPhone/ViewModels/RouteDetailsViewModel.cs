using System.Collections.Generic;
using System.Linq;
using Codify.Commands;
using Codify.Extensions;
using Codify.ViewModels;
using Travlexer.Data;
using Travlexer.WindowsPhone.Infrastructure;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class RouteDetailsViewModel : DataViewModel<Route>
    {
        #region Private Fields

        private readonly IDataContext _data;

        #endregion


        #region Constructors

        public RouteDetailsViewModel(IDataContext data)
        {
            _data = data;
            var route = Data = _data.SelectedRoute.Value;

            _data.Places.ForEach(
                p =>
                {
                    if (p.Id == route.DeparturePlaceId) DeparturePlace = p;
                    else if (p.Id == route.ArrivalPlaceId) ArrivalPlace = p;
                });

            uint index = 1;
            Steps = route.Steps.Select(step => new RouteStepSummaryViewModel(index++, step));

            CommandGoToStep = new DelegateCommand<RouteStep>(step => SelectedStep = step);
        }

        #endregion


        #region Public Properties

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

        #endregion


        #region Commands

        public DelegateCommand<RouteStep> CommandGoToStep { get; private set; }

        #endregion
    }
}