using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Codify.Entities;
using Travlexer.Data;
using Travlexer.WindowsPhone.Infrastructure;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class ManageViewModel : NotifyableEntity
    {
        #region Private Members

        private readonly IDataContext _data;
        private readonly ReadOnlyObservableCollection<Place> _places;

        #endregion


        #region Constructors

        public ManageViewModel(IDataContext data)
        {
            _data = data;
            _places = data.Places;
        }

        #endregion


        #region Public Properties

        public ReadOnlyObservableCollection<Route> Routes
        {
            get { return _data.Routes; }
        }

        public ReadOnlyObservableCollection<Trip> Trips
        {
            get { return _data.Trips; }
        }

        public ReadOnlyObservableCollection<Tour> Tours
        {
            get { return _data.Tours; }
        }

        public IEnumerable<Place> PersonalPlaces { get { return _places.Where(p => !p.IsSearchResult); } }

        public IEnumerable<Place> SearchResults { get { return _places.Where(p => p.IsSearchResult); } }

        #endregion
    }
}