using System;
using System.Collections.Generic;
using System.Reflection;
using Codify.ViewModels;
using Travlexer.WindowsPhone.Infrastructure;
using Travlexer.WindowsPhone.Infrastructure.Models;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class PlaceDetailsViewModel : DataViewModel<Place>
    {
        #region Constructor

        public PlaceDetailsViewModel()
        {
            Data = ApplicationContext.Data.SelectedPlace;
        }

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
