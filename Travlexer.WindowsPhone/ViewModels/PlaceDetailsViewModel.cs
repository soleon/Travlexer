using System;
using System.Collections.Generic;
using System.ComponentModel;
using Codify;
using Codify.Commands;
using Codify.Extensions;
using Codify.ViewModels;
using Travlexer.Data;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class PlaceDetailsViewModel : DataViewModel<Place>
    {
        #region Constructor

        public PlaceDetailsViewModel()
        {
            Data = ApplicationContext.Data.SelectedPlace;
            Data.PropertyChanged -= OnDataPropertyChanged;
            Data.PropertyChanged += OnDataPropertyChanged;

            CommandUpdatePlaceInfo = new DelegateCommand(() => ApplicationContext.Data.GetPlaceDetails(Data));

            IsBusy = Data.DataState == DataStates.Busy;
        }

        #endregion


        #region Commands

        public DelegateCommand CommandUpdatePlaceInfo { get; private set; }

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

        public bool IsBusy
        {
            get { return _isBusy; }
            private set { SetProperty(ref _isBusy, value, IsBusyProperty); }
        }

        private bool _isBusy;
        private const string IsBusyProperty = "IsBusy";

        #endregion


        #region Event Handling

        private void OnDataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != Place.DataStateProperty) return;
            var dataState = Data.DataState;
            DataStateChanged.ExecuteIfNotNull(dataState);
            IsBusy = dataState == DataStates.Busy;
        }

        #endregion


        #region Public Events

        public event Action<DataStates> DataStateChanged;

        #endregion
    }
}