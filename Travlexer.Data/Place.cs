using System;
using Codify;
using Codify.Entities;

namespace Travlexer.Data
{
    public class Place : NotifyableEntity
	{
		#region Constants

		private const string DefaultName = "My Place";

		#endregion


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Place"/> class.
		/// </summary>
		/// <param name="location">The geographical location of this pin.</param>
		/// <param name="id">The ID of this place.</param>
		/// <param name="name">The name of this place.</param>
		public Place(Location location, Guid id = default(Guid), string name = DefaultName)
		{
			if (location == null)
			{
				throw new ArgumentNullException("location", "Location is required in order to create a place.");
			}
			Location = location;
			Id = id == default(Guid) ? Guid.NewGuid() : id;
			Name = name;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="Place"/> class.
		/// </summary>
		/// <remarks>
		/// This parmeterless constructor is only intended for serialization purpose.
		/// Do not use this constructor in code.
		/// </remarks>
		public Place() { }

		#endregion


		#region Public Properties

		public Guid Id { get; set; }

		public string Name
		{
			get { return _name; }
			set { SetProperty(ref _name, value, NameProperty); }
		}

		private string _name;
		private const string NameProperty = "Name";

		public Location Location
		{
			get { return _location; }
			set { SetProperty(ref _location, value, LocationProperty); }
		}

		private Location _location;
		private const string LocationProperty = "Location";

		public string Address
		{
			get { return _address; }
			set { SetProperty(ref _address, value, AddressProperty); }
		}

		private string _address;
		private const string AddressProperty = "Address";

		public string ContactNumber
		{
			get { return _contactNumber; }
			set { SetProperty(ref _contactNumber, value, ContactNumberProperty); }
		}

		private string _contactNumber;
		private const string ContactNumberProperty = "ContactNumber";

		public string WebSite
		{
			get { return _webSite; }
			set { SetProperty(ref _webSite, value, WebSiteProperty); }
		}

		private string _webSite;
		private const string WebSiteProperty = "WebSite";

		public string Rating
		{
			get { return _rating; }
			set { SetProperty(ref _rating, value, RatingProperty); }
		}

		private string _rating;
		private const string RatingProperty = "Rating";

		public ElementColor Color
		{
			get { return _color; }
			set { SetProperty(ref _color, value, ColorProperty); }
		}

		private ElementColor _color;
		private const string ColorProperty = "Color";

		public PlaceIcon Icon
		{
			get { return _icon; }
			set { SetProperty(ref _icon, value, IconProperty); }
		}

		private PlaceIcon _icon;
		private const string IconProperty = "Icon";

		public bool IsSearchResult
		{
			get { return _isSearchResult; }
			set { SetProperty(ref _isSearchResult, value, IsSearchResultProperty); }
		}

		private bool _isSearchResult;
		private const string IsSearchResultProperty = "IsSearchResult";

		public string Notes
		{
			get { return _notes; }
			set { SetProperty(ref _notes, value, NotesProperty); }
		}

		private string _notes;
		private const string NotesProperty = "Notes";

		public ViewPort ViewPort
		{
			get { return _viewPort; }
			set { SetProperty(ref _viewPort, value, ViewPortProperty); }
		}

		private ViewPort _viewPort;
		private const string ViewPortProperty = "ViewPort";

		public string Reference
		{
			get { return _reference; }
			set { SetProperty(ref _reference, value, ReferenceProperty); }
		}

		private string _reference;
		private const string ReferenceProperty = "Reference";

		public DataStates DataState
		{
			get { return _dataState; }
			set { SetProperty(ref _dataState, value, DataStateProperty); }
		}

		private DataStates _dataState;
		public const string DataStateProperty = "DataState";

		#endregion
	}
}
