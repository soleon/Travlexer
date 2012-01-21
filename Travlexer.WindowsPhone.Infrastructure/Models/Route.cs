using System.Collections.Generic;
using Codify.WindowsPhone.Models;

namespace Travlexer.WindowsPhone.Infrastructure.Models
{
	public class Route : ModelBase
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
