using System;
using System.Windows.Input;
using Codify.ViewModels;

namespace Travlexer.WindowsPhone.ViewModels
{
	/// <summary>
	/// Represents the view model for a Codify.WindowsPhone.ShellExtension.ApplicationBarIconButton.
	/// </summary>
	public class AppBarButtonViewModel : ViewModelBase<MapViewModel>
	{
		public string Text
		{
			get { return _text; } 
			set { SetProperty(ref _text, value, TextProperty); }
		}

		private string _text;
		private const string TextProperty = "Text";

		public bool IsEnabled
		{
			get { return _isEnabled; } 
			set { SetProperty(ref _isEnabled, value, IsEnabledProperty); }
		}

		private bool _isEnabled = true;
		private const string IsEnabledProperty = "IsEnabled";

		public Uri IconUri
		{
			get { return _iconUri; } 
			set { SetProperty(ref _iconUri, value, IconUriProperty); }
		}

		private Uri _iconUri;
		private const string IconUriProperty = "IconUri";

		public ICommand Command
		{
			get { return _command; } 
			set { SetProperty(ref _command, value, CommandProperty); }
		}

		private ICommand _command;
		private const string CommandProperty = "Command";

		public object CommandParameter
		{
			get { return _commandParameter; } 
			set { SetProperty(ref _commandParameter, value, CommandParameterProperty); }
		}

		private object _commandParameter;
		private const string CommandParameterProperty = "CommandParameter";
	}
}