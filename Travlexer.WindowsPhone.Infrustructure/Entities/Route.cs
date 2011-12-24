using System.Collections.Generic;
using Travlexer.WindowsPhone.Core;

namespace Travlexer.WindowsPhone.Infrustructure.Entities
{
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
