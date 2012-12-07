using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using Codify.Commands;
using Codify.ViewModels;
using Travlexer.Data;
using Travlexer.WindowsPhone.Infrastructure;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class RouteDetailsViewModel : DataViewModel<Route>
    {
        #region Private Fields

        private readonly IDataContext _data;
        private readonly GeoCoordinateWatcher _geoWatcher;

        #endregion


        #region Constructors

        public RouteDetailsViewModel(IDataContext data)
        {
            _data = data;
            var route = Data = _data.SelectedRoute.Value;

            MapViewLocations = route.Points;

            byte foundPlacesCount = 0;
            foreach (var p in data.Places)
            {
                if (p.Id == route.DeparturePlaceId)
                {
                    DeparturePlace = p;
                    foundPlacesCount++;
                }
                else if (p.Id == route.ArrivalPlaceId)
                {
                    ArrivalPlace = p;
                    foundPlacesCount++;
                }
                if (foundPlacesCount == 2) break;
            }

            uint index = 1;
            Steps = route.Steps.Select(step => new RouteStepSummaryViewModel(index++, step)).ToArray();

            CommandGoToStep = new DelegateCommand<RouteStep>(step => SelectedStep = step);

#if DEBUG
            if (DesignerProperties.IsInDesignTool) return;
#endif
            // Initialise geo-coordinate watcher.
            _geoWatcher = ApplicationContext.GeoCoordinateWatcher;
            _geoWatcher.PositionChanged += OnGeoWatcherPositionChanged;
        }

        #endregion


        #region Event Handling

        /// <summary>
        ///     Called when <see cref="GeoCoordinateWatcher.PositionChanged" /> event is raised.
        /// </summary>
        private void OnGeoWatcherPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            var location = e.Position.Location;
            if (location.IsUnknown) return;
            CurrentLocation = location;
        }

        #endregion


        #region Public Properties

        public Dictionary<ElementColor, string> RouteColors
        {
            get { return ApplicationContext.Data.ElementColorMap; }
        }

        public IEnumerable<RouteStepSummaryViewModel> Steps { get; private set; }

        public RouteStep SelectedStep
        {
            get { return _selectedStep; }
            set
            {
                if (!SetValue(ref _selectedStep, value, SelectedStepProperty) || value == null) return;
                MapViewLocations = new[] {value.StartLocation};
            }
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

        public IEnumerable<Location> MapViewLocations
        {
            get { return _mapViewLocations; }
            set { SetValue(ref _mapViewLocations, value, MapviewLocationsProperty); }
        }

        private IEnumerable<Location> _mapViewLocations;
        internal const string MapviewLocationsProperty = "MapViewLocations";

        public GeoCoordinate CurrentLocation
        {
            get { return _currentLocation; }
            set { SetValue(ref _currentLocation, value, CurrentLocationProperty); }
        }

        private GeoCoordinate _currentLocation;
        private const string CurrentLocationProperty = "CurrentLocation";

        #endregion


        #region Commands

        public DelegateCommand<RouteStep> CommandGoToStep { get; private set; }

        #endregion
    }
}