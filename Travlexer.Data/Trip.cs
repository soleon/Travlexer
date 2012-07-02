using System.Collections.ObjectModel;
using Codify.Entities;

namespace Travlexer.Data
{
    /// <summary>
    /// A trip is a collection of routes.
    /// </summary>
    public class Trip : NotifyableEntity
    {
        /// <summary>
        /// Gets or sets the name of this trip.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value, NameProperty); }
        }

        private string _name;
        private const string NameProperty = "Name";

        /// <summary>
        /// Gets or sets the routes in this trip.
        /// </summary>
        public ObservableCollection<Route> Routes { get; set; }
    }
}