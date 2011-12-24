using System.ComponentModel;
using Travlexer.WindowsPhone.Infrustructure.Entities;

namespace Travlexer.WindowsPhone
{
	public class DesignTime
	{
#if DEBUG
		public UserPin UserPin
		{
			get
			{
				if (!DesignerProperties.IsInDesignTool)
				{
					return null;
				}
				return _userPin ?? (_userPin = new UserPin(
					new Location(-33.91, 151.21), name: "Jason's New Place")
					{
						Address = new Address { FormattedAddress = "235 South Dowling St, Darlington NSW 2017, Australia" }
					});
			}
		}
		private UserPin _userPin;
#endif
	}
}
