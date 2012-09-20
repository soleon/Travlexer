using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codify.Collections;
using Codify.Commands;
using Codify.Entities;
using Travlexer.Data;
using Travlexer.WindowsPhone.Infrastructure;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class ManageViewModel : NotifyableEntity
    {
        #region Private Members

        private readonly IDataContext _data;
        private readonly Dictionary<ManagementSections, IEnumerable<AppBarButtonViewModel>> _managementSectionAppBarButtons;
        private readonly Dictionary<ManagementSections, IEnumerable<AppBarMenuItemViewModel>> _managementSectionMenuItems;

        #endregion


        #region Constructors

        public ManageViewModel(IDataContext data)
        {
            _data = data;
            var places = data.Places;

            Routes = new AdaptedObservableCollection<Route, CheckableViewModel<Route>>(route => new CheckableViewModel<Route> {Data = route}, _data.Routes);
            Trips = new AdaptedObservableCollection<Trip, CheckableViewModel<Trip>>(trip => new CheckableViewModel<Trip> {Data = trip}, _data.Trips);
            PersonalPlaces = new AdaptedObservableCollection<Place, CheckableViewModel<Place>>(place => new CheckableViewModel<Place> {Data = place}, place => !place.IsSearchResult, places);
            SearchResults = new AdaptedObservableCollection<Place, CheckableViewModel<Place>>(place => new CheckableViewModel<Place> {Data = place}, place => place.IsSearchResult, places);

            CommandDeleteSelectedItems = new DelegateCommand(OnDeleteSelectedItems);
            CommandSelectAllItems = new DelegateCommand(OnSelectAllItems);
            CommandClearSelection = new DelegateCommand(OnClearSelection);
        }


        #endregion


        #region Event Handling

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
                    if (Trips.All(t => !t.IsChecked)) return;
                    if (MessageBox.Show("This will remove all selected trips including all routes and places in these trips. Do you want to continue?", "Remove Trips", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        for (var i = Trips.Count - 1; i >= 0; i--)
                        {
                            var item = Trips[i];
                            if (item.IsChecked) _data.RemoveTrip(item.Data);
                        }
                    break;
                case ManagementSections.Routes:
                    if (Routes.All(r => !r.IsChecked)) return;
                    if (MessageBox.Show("This will remove all selected routes including all places in these routes. Do you want to continue?", "Clear Routes", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        for (var i = Routes.Count - 1; i >= 0; i--)
                        {
                            var item = Routes[i];
                            if (item.IsChecked) _data.RemoveRoute(item.Data);
                        }
                    break;
                case ManagementSections.PersonalPlaces:
                    if (PersonalPlaces.All(p => !p.IsChecked)) return;
                    if (MessageBox.Show("This will remove all selected places. Do you want to continue?", "Clear Places", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        for (var i = PersonalPlaces.Count - 1; i >= 0; i--)
                        {
                            var item = PersonalPlaces[i];
                            if (item.IsChecked) _data.RemovePlace(item.Data);
                        }
                    break;
                case ManagementSections.SearchResults:
                    if (SearchResults.All(p => !p.IsChecked)) return;
                    if (MessageBox.Show("This will remove all selected search results. Do you want to continue?", "Clear Search Results", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        for (var i = SearchResults.Count - 1; i >= 0; i--)
                        {
                            var item = SearchResults[i];
                            if (item.IsChecked) _data.RemovePlace(item.Data);
                        }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion


        #region Public Properties

        public AdaptedObservableCollection<Route, CheckableViewModel<Route>> Routes { get; private set; }

        public AdaptedObservableCollection<Trip, CheckableViewModel<Trip>> Trips { get; private set; }

        public AdaptedObservableCollection<Place, CheckableViewModel<Place>> PersonalPlaces { get; private set; }

        public AdaptedObservableCollection<Place, CheckableViewModel<Place>> SearchResults { get; private set; }

        public ManagementSections SelectedManagementSection
        {
            get { return _selectedManagementSection; }
            set { SetValue(ref _selectedManagementSection, value, SelectedManagementSectionProperty); }
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

        #endregion


        #region Commands

        public DelegateCommand CommandDeleteSelectedItems { get; private set; }

        public DelegateCommand CommandSelectAllItems { get; private set; }

        public DelegateCommand CommandClearSelection { get; private set; }

        #endregion
    }
}