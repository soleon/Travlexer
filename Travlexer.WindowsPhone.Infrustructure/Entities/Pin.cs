using System;
using System.Collections.Generic;
using Travlexer.Core;

namespace Travlexer.WindowsPhone.Infrustructure.Entities
{
	public abstract class Place : EntityBase
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Place"/> class.
		/// </summary>
		/// <param name="location">The geographical location of this pin.</param>
		protected Place(Location location)
		{
			if (location == null)
			{
				throw new ArgumentNullException("location", "Location is required in order to create a pin.");
			}
			Location = location;
		}

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

		public Address Address
		{
			get { return _address; }
			set { SetProperty(ref _address, value, AddressProperty); }
		}

		private Address _address;
		private const string AddressProperty = "Address";

		public PlaceDetail Detail
		{
			get { return _detail; }
			set { SetProperty(ref _detail, value, DetailProperty); }
		}

		private PlaceDetail _detail;
		private const string DetailProperty = "Detail";

		#endregion


		#region Event Handling

		protected override void OnDispose()
		{
			Name = null;
			Location = null;
			Address = null;
			base.OnDispose();
		}

		#endregion
	}

	/// <summary>
	/// Represents additional information for a <see cref="Place"/>.
	/// </summary>
	public class PlaceDetail : EntityBase
	{
		#region Public Properties

		public string ContactNumberMain
		{
			get { return _contactNumberMain; }
			set { SetProperty(ref _contactNumberMain, value, ContactNumberMainProperty); }
		}

		private string _contactNumberMain;
		private const string ContactNumberMainProperty = "ContactNumberMain";

		public string ContactNumberSecondary
		{
			get { return _contactNumberSecondary; }
			set { SetProperty(ref _contactNumberSecondary, value, ContactNumberSecondaryProperty); }
		}

		private string _contactNumberSecondary;
		private const string ContactNumberSecondaryProperty = "ContactNumberSecondary";

		#endregion


		#region Event Handling

		protected override void OnDispose()
		{
			ContactNumberMain = null;
			ContactNumberSecondary = null;
			base.OnDispose();
		}

		#endregion
	}

	/// <summary>
	/// Represents a user placed pin on the map.
	/// </summary>
	public class Pin : Place
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Pin"/> class.
		/// </summary>
		/// <param name="location">The geographical location of this pin.</param>
		/// <param name="id">The ID of this pin.</param>
		/// <param name="name">The name of this pin.</param>
		public Pin(Location location, Guid id = default(Guid), string name = "My Pin")
			: base(location)
		{
			Id = id == default(Guid) ? Guid.NewGuid() : id;
			Name = name;
		}

		#endregion


		#region Public Properties

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

		#endregion


		#region Event Handling

		protected override void OnDispose()
		{
			Note = null;
			base.OnDispose();
		}

		#endregion
	}

	/// <summary>
	/// Represents a search result pin on the map.
	/// </summary>
	public class SearchResult : Place
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SearchResult"/> class.
		/// </summary>
		/// <param name="location">The geographical location of this search result.</param>
		public SearchResult(Location location) : base(location) { }
	}

	/// <summary>
	/// Defines the available icons for a <see cref="Place"/>.
	/// </summary>
	public enum PinIcon : byte
	{
		General = 0,
		Recreation,
		Vehicle,
		Drink,
		Fuel,
		Property,
		Restaurant,
		Shop
	}

	/// <summary>
	/// Defines the available colors for a <see cref="Place"/>.
	/// </summary>
	public enum PinColor : byte
	{
		Blue = 0,
		Magenta,
		Purple,
		Teal,
		Lime,
		Brown,
		Pink,
		Orange,
		Red,
		Green
	}

	/// <summary>
	/// Represents a geographical location.
	/// </summary>
	public class Location
	{
		#region Public Properties

		public double Latitude { get; set; }
		public double Longitued { get; set; }

		#endregion
	}

	/// <summary>
	/// Represents an address.
	/// </summary>
	public class Address
	{
		#region Public Properties

		public string FormattedAddress { get; set; }

		#endregion
	}

	public class Route : EntityBase
	{
		#region Public Properties

		public IEnumerable<Location> Points { get; set; }

		public string Name
		{
			get { return _name; }
			set { SetProperty(ref _name, value, NameProperty); }
		}

		private string _name;
		private const string NameProperty = "Name";

		#endregion
	}
}
