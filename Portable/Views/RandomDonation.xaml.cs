using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using dona.Forms.ViewModels;

namespace dona.Forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RandomDonation : ContentPage
    {
        public RandomDonation()
        {
            InitializeComponent();
            this.BindingContext = new RandomDonationPageViewModel();
            InstitutionsList.ItemSelected += (sender, e) => { ((ListView)sender).SelectedItem = null; };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is RandomDonationPageViewModel vm)
            {
                vm.GenerateDonation.Execute(null);
            }

        }
    }
}