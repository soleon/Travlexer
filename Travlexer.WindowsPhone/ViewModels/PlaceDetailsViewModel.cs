using System.Collections.Generic;
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

        public PlaceDetailsViewModel(IDataContext data)
        {
            Data = data.SelectedPlace.Value;

            CommandUpdatePlaceInfo = new DelegateCommand(() => ApplicationContext.Data.GetPlaceDetails(Data));
            CommandNavigateToUrl = new DelegateCommand<string>(PhoneTasks.OpenUrl);
            CommandCallNumber = new DelegateCommand<string>(number => PhoneTasks.CallPhoneNumber(Data.Name, number));
            CommandMarkAsPin = new DelegateCommand(() => Data.IsSearchResult = false);
        }

        #endregion


        #region Commands

        public DelegateCommand CommandUpdatePlaceInfo { get; private set; }
        public DelegateCommand<string> CommandNavigateToUrl { get; private set; }
        public DelegateCommand<string> CommandCallNumber { get; private set; }
        public DelegateCommand CommandMarkAsPin { get; private set; }

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