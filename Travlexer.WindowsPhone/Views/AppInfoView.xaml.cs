using System;
using Codify.Attributes;
using Codify.WindowsPhone;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Views
{
    [ViewModelType(typeof (AppInfoViewModel))]
    public partial class AppInfoView
    {
        public AppInfoView()
        {
            InitializeComponent();
        }

        private void OnReviewButtonClick(object sender, EventArgs e)
        {
            PhoneTasks.ShowMarketplaceReview();
        }
    }
}