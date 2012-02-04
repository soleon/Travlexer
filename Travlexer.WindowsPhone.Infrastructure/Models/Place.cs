using System;
using Codify;
using Codify.Models;

namespace Travlexer.WindowsPhone.Infrastructure.Models
{
	public class Place : ModelBase
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
		public Place()
			: this(new Location(), name: null) {}

		#endregion


		#region Public Properties

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

		public string FormattedAddress
		{
			get { return _formattedAddress; }
			set { SetProperty(ref _formattedAddress, value, AddressProperty); }
		}

		private string _formattedAddress;
		private const string AddressProperty = "FormattedAddress";

		public PlaceDetails Details
		{
			get { return _detail; }
			set { SetProperty(ref _detail, value, DetailProperty); }
		}

		private PlaceDetails _detail;
		private const string DetailProperty = "Detail";

		public PlaceColor Color
		{
			get { return _color; }
			set { SetProperty(ref _color, value, ColorProperty); }
		}

		private PlaceColor _color;
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

		public Guid Id
		{
			get { return _id; }
			set { SetProperty(ref _id, value, IdProperty); }
		}

		private Guid _id;
		private const string IdProperty = "Id";

		public string Note
		{
			get { return _note; }
			set { SetProperty(ref _note, value, NoteProperty); }
		}

		private string _note;
		private const string NoteProperty = "Note";

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
		private const string DataStateProperty = "DataState";


		#endregion


		#region Event Handling

		protected override void OnDispose()
		{
			Name = null;
			Location = null;
			FormattedAddress = null;
			Note = null;
			if (Details != null)
			{
				var details = Details;
				Details = null;
				details.Dispose();
			}
			base.OnDispose();
		}

		#endregion
	}
}
