using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using Codify.Collections;
using Codify.Commands;
using Codify.Entities;
using Codify.Extensions;
using Codify.WindowsPhone;
using Travlexer.Data;
using Travlexer.WindowsPhone.Infrastructure;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class ManageViewModel : NotifyableEntity
    {
        #region Private Members

        private readonly IDataContext _data;
        private readonly INavigationService _navigationService;

        #endregion


        #region Constructors

        public ManageViewModel(IDataContext data, INavigationService navigationService)
        {
            _data = data;
            _navigationService = navigationService;

            var places = data.Places;

            Routes = new AdaptedObservableCollection<Route, RouteSummaryViewModel>(route => new RouteSummaryViewModel(route), source: _data.Routes);
            Trips = new AdaptedObservableCollection<Trip, CheckableViewModel<Trip>>(trip => new CheckableViewModel<Trip> { Data = trip }, source: _data.Trips);
            PersonalPlaces = new AdaptedObservableCollection<Place, CheckableViewModel<Place>>(place => new CheckableViewModel<Place> { Data = place }, place => !place.IsSearchResult, (p1, p2) => String.CompareOrdinal(p1.Name, p2.Name), places);
            SearchResults = new AdaptedObservableCollection<Place, CheckableViewModel<Place>>(place => new CheckableViewModel<Place> { Data = place }, place => place.IsSearchResult, (p1, p2) => String.CompareOrdinal(p1.Name, p2.Name), places);

            CommandDeleteSelectedItems = new DelegateCommand(OnDeleteSelectedItems);
            CommandSelectAllItems = new DelegateCommand(OnSelectAllItems);
            CommandClearSelection = new DelegateCommand(OnClearSelection);
            CommandPinSelectedSearchResult = new DelegateCommand(OnPinSelectedSearchResult);
            CommandGoToPlace = new DelegateCommand<Place>(OnGoToPlace);
            CommandShowPlaceDetails = new DelegateCommand<Place>(OnShowPlaceDetails);
            CommandShowRouteDetails = new DelegateCommand<Route>(OnShowRouteDetails);

            _navigationService.Navigating += OnNavigating;
        }

        private void OnShowRouteDetails(Route route)
        {
            _data.SelectedRoute.Value = route;
            _navigationService.Navigate<RouteDetailsViewModel>();
        }

        #endregion


        #region Event Handling

        /// <summary>
        /// Captures navigate back event from this view model to perform some disposal actions.
        /// </summary>
        private void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            // Catches the "Back" navigating event except if it is navigating back from outside of this application.
            if (!e.IsNavigationInitiator || e.NavigationMode != NavigationMode.Back) return;

            if (sender == this)
            {
                // When the sender is this view model, then we are navigating back to the previous view model.
                // Dispose all necessary data here.
                _navigationService.Navigating -= OnNavigating;
                Routes.Dispose();
                Trips.Dispose();
                PersonalPlaces.Dispose();
                SearchResults.Dispose();
            }
            else
            {
                // When the sender is not this view model, then we are navigating back from somewhere else to this view model.
                // Refresh all necessary data here.
                Routes.Refresh();
                Trips.Refresh();
                PersonalPlaces.Refresh();
                SearchResults.Refresh();
            }
        }

        private void OnShowPlaceDetails(Place place)
        {
            _data.SelectedPlace.Value = place;
            _navigationService.Navigate<PlaceDetailsViewModel>();
        }

        private void OnGoToPlace(Place place)
        {
            _data.SelectedPlace.Value = place;
            _navigationService.GoBack();
        }

        private void OnPinSelectedSearchResult()
        {
            var selectedSearchResults = SearchResults.Where(p => p.IsChecked).ToArray();
            if (selectedSearchResults.Length == 0) return;
            selectedSearchResults.ForEach(p => p.Data.IsSearchResult = false);
            PersonalPlaces.Refresh();
            SearchResults.Refresh();
        }

        private void OnClearSelection()
        {
            switch (_selectedManagementSection)
            {
                case ManagementSection.Trips:
                    foreach (var trip in Trips)
                        trip.IsChecked = false;
                    break;
                case ManagementSection.Routes:
                    foreach (var route in Routes)
                        route.IsChecked = false;
                    break;
                case ManagementSection.PersonalPlaces:
                    foreach (var place in PersonalPlaces)
                        place.IsChecked = false;
                    break;
                case ManagementSection.SearchResults:
                    foreach (var place in SearchResults)
                        place.IsChecked = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnSelectAllItems()
        {
            switch (_selectedManagementSection)
            {
                case ManagementSection.Trips:
                    foreach (var trip in Trips)
                        trip.IsChecked = true;
                    break;
                case ManagementSection.Routes:
                    foreach (var route in Routes)
                        route.IsChecked = true;
                    break;
                case ManagementSection.PersonalPlaces:
                    foreach (var place in PersonalPlaces)
                        place.IsChecked = true;
                    break;
                case ManagementSection.SearchResults:
                    foreach (var place in SearchResults)
                        place.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDeleteSelectedItems()
        {
            switch (_selectedManagementSection)
            {
                case ManagementSection.Trips:
                    if (Trips.All(t => !t.IsChecked) ||
                        MessageBox.Show("This will remove all selected trips including all routes and places in these trips. Do you want to continue?", "Remove Trips", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                        return;

                    var selectedTrips = Trips.Where(tripVm => tripVm.IsChecked).Select(tripVm => tripVm.Data).ToArray();
                    for (var i = selectedTrips.Length - 1; i >= 0; i--)
                    {
                        var trip = selectedTrips[i];
                        var routes = trip.Routes;
                        for (var j = routes.Count - 1; j >= 0; j--)
                        {
                            var route = routes[j];
                            var id = route.DeparturePlaceId;
                            if (id != Guid.Empty) _data.RemovePlace(route.DeparturePlaceId);
                            id = route.ArrivalPlaceId;
                            if (id != Guid.Empty) _data.RemovePlace(route.ArrivalPlaceId);
                            _data.RemoveRoute(route);
                        }
                        _data.RemoveTrip(trip);
                    }
                    break;
                case ManagementSection.Routes:
                    if (Routes.All(r => !r.IsChecked) ||
                        MessageBox.Show("This will remove all selected routes. Do you want to continue?", "Remove Routes", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                        return;
                    var removePlaces = MessageBox.Show("Do you also want to remove all connected pins and search results?", "Remove Connected Locations", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
                    var selectedRoutes = Routes.Where(routeVm => routeVm.IsChecked).ToArray();
                    for (var i = selectedRoutes.Length - 1; i >= 0; i--)
                    {
                        var route = selectedRoutes[i];
                        if (removePlaces)
                        {
                            _data.RemovePlace(route.DeparturePlace);
                            _data.RemovePlace(route.ArrivalPlace);
                        }
                        _data.RemoveRoute(route.Data);
                    }
                    break;
                case ManagementSection.PersonalPlaces:
                    if (PersonalPlaces.All(p => !p.IsChecked) ||
                        MessageBox.Show("This will remove all selected places and all routes that are connected to them. Do you want to continue?", "Remove Places", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                        return;

                    var selectedPlaces = PersonalPlaces.Where(placeVm => placeVm.IsChecked).Select(placeVm => placeVm.Data).ToArray();
                    for (var i = selectedPlaces.Length - 1; i >= 0; i--)
                        _data.RemovePlace(selectedPlaces[i]);
                    break;
                case ManagementSection.SearchResults:
                    if (SearchResults.All(p => !p.IsChecked) ||
                        MessageBox.Show("This will remove all selected search results and all routes that are connected to them. Do you want to continue?", "Clear Search Results", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                        return;

                    var selectedResults = SearchResults.Where(placeVm => placeVm.IsChecked).Select(placeVm => placeVm.Data).ToArray();
                    for (var i = selectedResults.Length - 1; i >= 0; i--)
                        _data.RemovePlace(selectedResults[i]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion


        #region Public Properties

        public AdaptedObservableCollection<Route, RouteSummaryViewModel> Routes { get; private set; }

        public AdaptedObservableCollection<Trip, CheckableViewModel<Trip>> Trips { get; private set; }

        public AdaptedObservableCollection<Place, CheckableViewModel<Place>> PersonalPlaces { get; private set; }

        public AdaptedObservableCollection<Place, CheckableViewModel<Place>> SearchResults { get; private set; }

        public ManagementSection SelectedManagementSection
        {
            get { return _selectedManagementSection; }
            set { SetValue(ref _selectedManagementSection, value, SelectedManagementSectionProperty, IsSearchResultsSectionSelectedProperty); }
        }

        private ManagementSection _selectedManagementSection;
        private const string SelectedManagementSectionProperty = "SelectedManagementSection";

        public IEnumerable<AppBarButtonViewModel> SelectedManagementSectionAppBarButtons
        {
            get { return _selectedManagementSectionAppBarButtons; }
            set { SetValue(ref _selectedManagementSectionAppBarButtons, value, SelectedManagementSectionAppBarButtonsProperty); }
        }

        private IEnumerable<AppBarButtonViewModel> _selectedManagementSectionAppBarButtons;
        private const string SelectedManagementSectionAppBarButtonsProperty = "SelectedManagementSectionAppBarButtons";

        public IEnumerable<AppBarMenuItemViewModel> SelectedManagementSectionMenuItems
        {
            get { return _selectedManagementSectionMenuItems; }
            set { SetValue(ref _selectedManagementSectionMenuItems, value, SelectedManagementSectionMenuItemsProperty); }
        }

        private IEnumerable<AppBarMenuItemViewModel> _selectedManagementSectionMenuItems;
        private const string SelectedManagementSectionMenuItemsProperty = "SelectedManagementSectionMenuItems";

        public bool IsSearchResultsSectionSelected
        {
            get { return _selectedManagementSection == ManagementSection.SearchResults; }
        }

        private const string IsSearchResultsSectionSelectedProperty = "IsSearchResultsSectionSelected";

        #endregion


        #region Commands

        public DelegateCommand CommandDeleteSelectedItems { get; private set; }

        public DelegateCommand CommandSelectAllItems { get; private set; }

        public DelegateCommand CommandClearSelection { get; private set; }

        public DelegateCommand CommandPinSelectedSearchResult { get; private set; }

        public DelegateCommand<Place> CommandGoToPlace { get; private set; }

        public DelegateCommand<Place> CommandShowPlaceDetails { get; private set; }

        public DelegateCommand<Route> CommandShowRouteDetails { get; private set; }

        #endregion
    }
}