using System.Collections.Generic;
using System.Windows;
using Codify.Commands;
using Codify.ViewModels;
using Codify.WindowsPhone;
using Travlexer.Data;
using Travlexer.WindowsPhone.Infrastructure;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class PlaceDetailsViewModel : DataViewModel<Place>
    {
        #region Constructor

        public PlaceDetailsViewModel()
        {
            Data = ApplicationContext.Data.SelectedPlace.Value;

            CommandUpdatePlaceInfo = new DelegateCommand(() => ApplicationContext.Data.GetPlaceDetails(Data));
            CommandNavigateToUrl = new DelegateCommand<string>(PhoneTasks.OpenUrl);
            CommandCallNumber = new DelegateCommand<string>(number => PhoneTasks.CallPhoneNumber(Data.Name, number));
            CommandMarkAsPin = new DelegateCommand(() => Data.IsSearchResult = false);
            CommandShowInBingMaps = new DelegateCommand(() => PhoneTasks.ShowBingMaps(Data.Location.ToGeoCoordinate()));
            CommandDelete = new DelegateCommand(()=>
            {
                var connectedRouteCount = Data.ConnectedRouteIds.Count;
                if (connectedRouteCount != 0 && MessageBox.Show("Deleting this location will also delete " + connectedRouteCount + " connecting route" + (connectedRouteCount > 1 ? "s" : null) + ". Do you want to continue?", "Delete Location", MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;
                ApplicationContext.Data.RemovePlace(Data);
                if(ApplicationContext.NavigationService.CanGoBack) ApplicationContext.NavigationService.GoBack();
                else throw new ExitException();
            });
        }

        #endregion


        #region Commands

        public DelegateCommand CommandUpdatePlaceInfo { get; private set; }
        public DelegateCommand<string> CommandNavigateToUrl { get; private set; }
        public DelegateCommand<string> CommandCallNumber { get; private set; }
        public DelegateCommand CommandMarkAsPin { get; private set; }
        public DelegateCommand CommandShowInBingMaps { get; private set; }
        public DelegateCommand CommandDelete { get; private set; }

        #endregion


        #region Public Properties

        public Dictionary<PlaceIcon, string> PlaceIcons
        {
            get { return ApplicationContext.Data.PlaceIconMap; }
        }

        public Dictionary<ElementColor, string> PlaceColors
        {
            get { return ApplicationContext.Data.ElementColorMap; }
        }

        #endregion
    }
}