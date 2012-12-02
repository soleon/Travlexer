using Codify.Attributes;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Views
{
    [ViewModelType(typeof(SettingsViewModel))]
    public partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
        }
    }
}