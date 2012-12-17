using System;
using Codify.Attributes;
using Codify.WindowsPhone;
using Travlexer.WindowsPhone.ViewModels;

namespace Travlexer.WindowsPhone.Views
{
    [ViewModelType(typeof (HelpViewModel))]
    public partial class HelpView
    {
        public HelpView()
        {
            InitializeComponent();
        }

        private void OnReviewButtonClick(object sender, EventArgs e)
        {
            PhoneTasks.ShowMarketplaceReview();
        }
    }
}