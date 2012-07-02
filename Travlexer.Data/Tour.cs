using System.Collections.ObjectModel;
using Codify.Entities;

namespace Travlexer.Data
{
    /// <summary>
    /// A tour is a collection of trips.
    /// </summary>
    public class Tour : NotifyableEntity
    {
        /// <summary>
        /// Gets or sets the name of this tour.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value, NameProperty); }
        }

        private string _name;
        private const string NameProperty = "Name";

        /// <summary>
        /// Gets or sets the trips in this tour.
        /// </summary>
        public ObservableCollection<Trip> Trips { get; set; } 
    }
}