using dona.Forms.Model;
using System.Collections.Generic;
using dona.Forms.ViewModels;
using Xamarin.Forms;

namespace dona.Forms.Views
{
    public partial class DonationResultPage : ContentPage
    {
        public DonationResultPage(IList<DonationResult> donationResults)
        {
            InitializeComponent();
            BindingContext = new DonationResultPageViewModel(donationResults);
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = this.BindingContext as DonationResultPageViewModel;
            vm.GoBackToMainPage();
            return false;
        }
    }
}