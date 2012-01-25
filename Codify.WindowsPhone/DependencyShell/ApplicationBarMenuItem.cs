using Microsoft.Phone.Shell;

namespace Codify.WindowsPhone.DependencyShell
{
	public class ApplicationBarMenuItem : DependencyShellItemBase<IApplicationBarMenuItem>
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
