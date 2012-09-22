using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codify.Collections;
using Codify.Commands;
using Codify.Entities;
using Codify.Extensions;
using Travlexer.Data;
using Travlexer.WindowsPhone.Infrastructure;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class ManageViewModel : NotifyableEntity
    {
        #region Private Members

        private readonly IDataContext _data;

        #endregion


        #region Constructors

        public ManageViewModel(IDataContext data)
        {
            _data = data;
            var places = data.Places;

            Routes = new AdaptedObservableCollection<Route, CheckableViewModel<Route>>(route => new CheckableViewModel<Route> {Data = route}, source: _data.Routes);
            Trips = new AdaptedObservableCollection<Trip, CheckableViewModel<Trip>>(trip => new CheckableViewModel<Trip> {Data = trip}, source: _data.Trips);
            PersonalPlaces = new AdaptedObservableCollection<Place, PlaceManagementItemViewModel>(place => new PlaceManagementItemViewModel { Data = place }, place => !place.IsSearchResult, (p1, p2) => String.CompareOrdinal(p1.Name, p2.Name), places);
            SearchResults = new AdaptedObservableCollection<Place, PlaceManagementItemViewModel>(place => new PlaceManagementItemViewModel { Data = place }, place => place.IsSearchResult, (p1, p2) => String.CompareOrdinal(p1.Name, p2.Name), places);

            CommandDeleteSelectedItems = new DelegateCommand(OnDeleteSelectedItems);
            CommandSelectAllItems = new DelegateCommand(OnSelectAllItems);
            CommandClearSelection = new DelegateCommand(OnClearSelection);
            CommandPinSelectedSearchResult = new DelegateCommand(OnPinSelectedSearchResult);
        }

        #endregion


        #region Event Handling

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
                case ManagementSections.Trips:
                    foreach (var trip in Trips)
                        trip.IsChecked = false;
                    break;
                case ManagementSections.Routes:
                    foreach (var route in Routes)
                        route.IsChecked = false;
                    break;
                case ManagementSections.PersonalPlaces:
                    foreach (var place in PersonalPlaces)
                        place.IsChecked = false;
                    break;
                case ManagementSections.SearchResults:
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
                case ManagementSections.Trips:
                    foreach (var trip in Trips)
                        trip.IsChecked = true;
                    break;
                case ManagementSections.Routes:
                    foreach (var route in Routes)
                        route.IsChecked = true;
                    break;
                case ManagementSections.PersonalPlaces:
                    foreach (var place in PersonalPlaces)
                        place.IsChecked = true;
                    break;
                case ManagementSections.SearchResults:
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
                case ManagementSections.Trips:
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
                case ManagementSections.Routes:
                    if (Routes.All(r => !r.IsChecked) ||
                        MessageBox.Show("This will remove all selected routes including all places in these routes. Do you want to continue?", "Clear Routes", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                        return;

                    var selectedRoutes = Routes.Where(routeVm => routeVm.IsChecked).Select(routeVm => routeVm.Data).ToArray();
                    for (var i = selectedRoutes.Length - 1; i >= 0; i--)
                    {
                        var route = selectedRoutes[i];
                        var id = route.DeparturePlaceId;
                        if (id != Guid.Empty) _data.RemovePlace(route.DeparturePlaceId);
                        id = route.ArrivalPlaceId;
                        if (id != Guid.Empty) _data.RemovePlace(route.ArrivalPlaceId);
                        _data.RemoveRoute(route);
                    }
                    break;
                case ManagementSections.PersonalPlaces:
                    if (PersonalPlaces.All(p => !p.IsChecked) ||
                        MessageBox.Show("This will remove all selected places. Do you want to continue?", "Clear Places", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                        return;

                    var selectedPlaces = PersonalPlaces.Where(placeVm => placeVm.IsChecked).Select(placeVm => placeVm.Data).ToArray();
                    for (var i = selectedPlaces.Length - 1; i >= 0; i--)
                        _data.RemovePlace(selectedPlaces[i]);
                    break;
                case ManagementSections.SearchResults:
                    if (SearchResults.All(p => !p.IsChecked) ||
                        MessageBox.Show("This will remove all selected search results. Do you want to continue?", "Clear Search Results", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
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

        public AdaptedObservableCollection<Route, CheckableViewModel<Route>> Routes { get; private set; }

        public AdaptedObservableCollection<Trip, CheckableViewModel<Trip>> Trips { get; private set; }

        public AdaptedObservableCollection<Place, PlaceManagementItemViewModel> PersonalPlaces { get; private set; }

        public AdaptedObservableCollection<Place, PlaceManagementItemViewModel> SearchResults { get; private set; }

        public ManagementSections SelectedManagementSection
        {
            get { return _selectedManagementSection; }
            set { SetValue(ref _selectedManagementSection, value, SelectedManagementSectionProperty, IsSearchResultsSectionSelectedProperty); }
        }

        private ManagementSections _selectedManagementSection;
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
            get { return _selectedManagementSection == ManagementSections.SearchResults; }
        }

        private const string IsSearchResultsSectionSelectedProperty = "IsSearchResultsSectionSelected";

        #endregion


        #region Commands

        public DelegateCommand CommandDeleteSelectedItems { get; private set; }

        public DelegateCommand CommandSelectAllItems { get; private set; }

        public DelegateCommand CommandClearSelection { get; private set; }

        public DelegateCommand CommandPinSelectedSearchResult { get; private set; }

        #endregion
    }

    public class PlaceManagementItemViewModel : CheckableViewModel<Place>
    {
        public PlaceManagementItemViewModel()
        {
            CommandGoToPlace = new DelegateCommand(OnGoToPlace);
        }

        private void OnGoToPlace()
        {
            ApplicationContext.Data.SelectedPlace.Value = Data;
            ApplicationContext.NavigationService.GoBack();
        }

        public DelegateCommand CommandGoToPlace { get; private set; }
    }
}