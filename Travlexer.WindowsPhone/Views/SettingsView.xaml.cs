using System;
using Codify.Attributes;
using Codify.WindowsPhone;
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

        private void OnReviewButtonClick(object sender, EventArgs e)
        {
            PhoneTasks.ShowMarketplaceReview();
        }
    }
}