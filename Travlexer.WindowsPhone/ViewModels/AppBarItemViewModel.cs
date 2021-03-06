using System.Windows.Input;
using Codify.Entities;

namespace Travlexer.WindowsPhone.ViewModels
{
    public class AppBarItemViewModel : NotifyableEntity
    {
        public string Text
        {
            get { return _text; }
            set { SetValue(ref _text, value, TextProperty); }
        }

        private string _text;
        private const string TextProperty = "Text";

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetValue(ref _isEnabled, value, IsEnabledProperty); }
        }

        private bool _isEnabled = true;
        private const string IsEnabledProperty = "IsEnabled";

        public ICommand Command
        {
            get { return _command; }
            set { SetValue(ref _command, value, CommandProperty); }
        }

        private ICommand _command;
        private const string CommandProperty = "Command";

        public object CommandParameter
        {
            get { return _commandParameter; }
            set { SetValue(ref _commandParameter, value, CommandParameterProperty); }
        }

        private object _commandParameter;
        private const string CommandParameterProperty = "CommandParameter";
    }
}