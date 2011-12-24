using System;
using Travlexer.WindowsPhone.Core;

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
}