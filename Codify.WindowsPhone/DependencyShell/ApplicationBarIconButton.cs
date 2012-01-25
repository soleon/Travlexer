using System;
using System.Windows;
using Microsoft.Phone.Shell;

namespace Codify.WindowsPhone.DependencyShell
{
	public class ApplicationBarIconButton : DependencyShellItemBase<IApplicationBarIconButton>
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationBarIconButton"/> class.
		/// </summary>
		public ApplicationBarIconButton()
		{
			Item = new Microsoft.Phone.Shell.ApplicationBarIconButton();
			Item.Click += OnExecuteCommand;
		}

		#endregion


		#region Public Properties

		/// <summary>
		/// The URI of the icon to use for the button.
		/// </summary>
		/// <returns>
		/// Type: <see cref="T:System.Uri"/>.
		/// </returns>
		public Uri IconUri
		{
			get { return (Uri) GetValue(IconUriProperty); }
			set { SetValue(IconUriProperty, value); }
		}

		public static readonly DependencyProperty IconUriProperty = DependencyProperty.Register(
			"IconUri",
			typeof (Uri),
			typeof (ApplicationBarIconButton),
			new PropertyMetadata(null, OnIconUriChanged));

		#endregion


		#region Event Handling

		/// <summary>
		/// Called when <see cref="IconUri"/> is changed.
		/// </summary>
		private static void OnIconUriChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var b = (ApplicationBarIconButton) sender;
			if (b.Item == null)
			{
				return;
			}
			var uri = (Uri) e.NewValue;
			b.Item.IconUri = uri;
		}

		#endregion
	}
}
