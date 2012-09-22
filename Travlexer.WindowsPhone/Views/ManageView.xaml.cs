using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Codify.Attributes;
using Codify.Extensions;
using Codify.WindowsPhone.ShellExtension;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Views
{
    [ViewModelType(typeof(ManageViewModel))]
    public partial class ManageView
    {
        private readonly ApplicationBarIconButton _pinSelectedSearchResultButton;

        public ManageView()
        {
            InitializeComponent();

            _pinSelectedSearchResultButton = new ApplicationBarIconButton
            {
                IconUri = new Uri("\\Assets\\AddPlace.png", UriKind.Relative),
                Text = "to personal"
            };
            _pinSelectedSearchResultButton.SetBinding(ApplicationBarIconButton.CommandProperty, new Binding("CommandPinSelectedSearchResult") { Mode = BindingMode.OneTime });

            SetBinding(SelectedManagementSectionProperty, new Binding("SelectedManagementSection") {Mode = BindingMode.TwoWay});
        }

        private void OnManagementPivotSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var obj = e.AddedItems[0] as DependencyObject;
            if (obj == null) return;
            var section = DataExtensions.GetManagementSection(obj);
            SelectedManagementSection = section;

            switch (section)
            {
                case ManagementSections.SearchResults:
                    AppBar.Buttons.Add(_pinSelectedSearchResultButton);
                    break;
                default:
                    AppBar.Buttons.Remove(_pinSelectedSearchResultButton);
                    break;
            }
        }


        #region SelectedManagementSection

        public ManagementSections SelectedManagementSection
        {
            get { return (ManagementSections) GetValue(SelectedManagementSectionProperty); }
            set { SetValue(SelectedManagementSectionProperty, value); }
        }

        public static readonly DependencyProperty SelectedManagementSectionProperty = DependencyProperty.Register(
            "SelectedManagementSection",
            typeof (ManagementSections),
            typeof (ManageView),
            null);

        #endregion
    }
}