using Travlexer.WindowsPhone.Core;

namespace Travlexer.WindowsPhone.Infrustructure.Entities
{
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
}