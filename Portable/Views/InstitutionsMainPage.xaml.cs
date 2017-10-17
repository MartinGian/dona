using System;
using Acr.UserDialogs;
using dona.Forms.Dependencies;
using dona.Forms.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FFImageLoading.Forms;
using dona.Forms.Model;
using System.Threading.Tasks;
using dona.Forms.Helpers;

namespace dona.Forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InstitutionsMainPage : ContentPage
    {
        private readonly INetworkInformation _networkInformation = DependencyService.Get<INetworkInformation>();

        public InstitutionsMainPage()
        {
            InitializeComponent();
            BindingContext = InstitutionsMainPageViewModel.Instance;
            InstitutionsList.ItemSelected += (sender, e) => { ((ListView)sender).SelectedItem = null; };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Task.Run(() =>
            {
                if (BindingContext is InstitutionsMainPageViewModel vm)
                {
                    vm.InitializeInstitutionsFromLocalAsync();
                    vm.LoadInstitutions.Execute(null);
                }
            });

#if RELEASE
            Task.Run(() =>
            {
                try
                {
                    var networkOperatorName = _networkInformation.GetNetworkOperatorName();
                    if (!networkOperatorName.Equals("Antel", StringComparison.OrdinalIgnoreCase))
                    {
                        UiHelper.InvokeOnTheMainThread(() =>
                        {
                            UserDialogs.Instance.AlertAsync(
                                "Parece que estas utilizando un operador que no es Antel. Las donaciones a través de Doná funcionan solamente con Antel.",
                                "Operador incorrecto");
                        });
                    }
                }
                catch (Exception)
                {
                    // ignore, this is just for check and notify
                }
            });
#endif

        }
    }
}
