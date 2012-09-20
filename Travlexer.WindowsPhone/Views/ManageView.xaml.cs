using System.Windows;
using System.Windows.Controls;
using Codify.Attributes;
using Codify.Extensions;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Views
{
    [ViewModelType(typeof (ManageViewModel))]
    public partial class ManageView
    {
        public ManageView()
        {
            InitializeComponent();
        }

        private void OnManagementPivotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (DataContext as ManageViewModel).UseIfNotNull(
                d => (e.AddedItems[0] as DependencyObject).UseIfNotNull(
                    obj => d.SelectedManagementSection = DataExtensions.GetManagementSection(obj)));
        }
    }
}