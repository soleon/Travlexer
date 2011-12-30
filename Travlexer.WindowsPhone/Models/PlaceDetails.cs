using Travelexer.WindowsPhone.Core.Models;

namespace Travlexer.WindowsPhone.Models
{
	public class PlaceDetails : ModelBase
	{
		#region Public Properties

		public string ContactNumber
		{
			get { return _contactNumber; }
			set { SetProperty(ref _contactNumber, value, ContactNumberProperty); }
		}

		private string _contactNumber;
		private const string ContactNumberProperty = "ContactNumber";

		#endregion


		#region Event Handling

		protected override void OnDispose()
		{
			ContactNumber = null;
			base.OnDispose();
		}

		#endregion
	}
}
