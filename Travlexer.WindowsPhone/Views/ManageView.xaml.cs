using Codify.Attributes;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Views
{
    [ViewModelType(typeof(ManageViewModel))]
    public partial class ManageView
    {
        public ManageView()
        {
            InitializeComponent();
        }
    }
}