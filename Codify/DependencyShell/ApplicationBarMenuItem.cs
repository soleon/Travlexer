using Microsoft.Phone.Shell;

namespace Codify.DependencyShell
{
	public class ApplicationBarMenuItem : ApplicationBarItemBase<IApplicationBarMenuItem>
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationBarMenuItem"/> class.
		/// </summary>
		public ApplicationBarMenuItem()
		{
			Item = new Microsoft.Phone.Shell.ApplicationBarMenuItem();
			Item.Click += OnExecuteCommand;
		}

		#endregion
	}
}
