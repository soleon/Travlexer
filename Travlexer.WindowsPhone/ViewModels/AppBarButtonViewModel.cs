using System;

namespace Travlexer.WindowsPhone.ViewModels
{
	/// <summary>
	/// Represents the view model for a Codify.WindowsPhone.ShellExtension.ApplicationBarIconButton.
	/// </summary>
	public class AppBarButtonViewModel : AppBarItemViewModel
	{
		public Uri IconUri
		{
			get { return _iconUri; } 
			set { SetValue(ref _iconUri, value, IconUriProperty); }
		}

		private Uri _iconUri;
		private const string IconUriProperty = "IconUri";
	}
}