using dona.Forms.Model;
using dona.Forms.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace dona.Forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InstitutionDetailsPage : ContentPage
    {
        public InstitutionDetailsPage(Institution institution)
        {
            InitializeComponent();
            this.BindingContext = new InstitutionDetailsPageViewModel(institution);
        }
    }

    
}
