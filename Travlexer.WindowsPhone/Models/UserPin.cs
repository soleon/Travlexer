using System;

namespace Travlexer.WindowsPhone.Models
{
	public class UserPin : Place
	{
		#region Constants

		private const string DefaultName = "My Pin";

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="UserPin"/> class.
		/// </summary>
		/// <param name="location">The geographical location of this pin.</param>
		/// <param name="id">The ID of this pin.</param>
		/// <param name="name">The name of this pin.</param>
		public UserPin(Location location, Guid id = default(Guid), string name = DefaultName)
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
}