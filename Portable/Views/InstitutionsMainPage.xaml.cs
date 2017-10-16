using System;
using Acr.UserDialogs;
using dona.Forms.Dependencies;
using dona.Forms.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FFImageLoading.Forms;
using dona.Forms.Model;
using System.Threading.Tasks;

namespace dona.Forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InstitutionsMainPage : ContentPage
    {
        public InstitutionsMainPage()
        {
            InitializeComponent();
            BindingContext = InstitutionsMainPageViewModel.Instance;
            InstitutionsList.ItemSelected += (sender, e) => { ((ListView)sender).SelectedItem = null; };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is InstitutionsMainPageViewModel vm)
            {
                vm.InitializeInstitutionsFromLocal();
                vm.LoadInstitutions.Execute(null);
            }

#if RELEASE
            try
            {
                var networkInformation = DependencyService.Get<INetworkInformation>();
                var networkOperatorName = networkInformation.GetNetworkOperatorName();

                if (!networkOperatorName.Equals("Antel", StringComparison.OrdinalIgnoreCase))
                {
                    UserDialogs.Instance.AlertAsync(
                        "Parece que estas utilizando un operador que no es Antel. Las donaciones a través de Doná funcionan solamente con Antel.",
                        "Operador incorrecto");
                }
            }
            catch (Exception)
            {
                // ignore, this is just for check and notify
            }
#endif

        }
    }
}
