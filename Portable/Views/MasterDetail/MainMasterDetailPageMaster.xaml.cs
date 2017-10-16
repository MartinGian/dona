using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using dona.Forms.Model;
using Xamarin.Forms;

namespace dona.Forms.Views.MasterDetail
{
    public partial class MainMasterDetailPageMaster : ContentPage
    {
        public ListView ListView;

        public MainMasterDetailPageMaster()
        {
            InitializeComponent();

            BindingContext = new MainMasterDetailPageMasterViewModel();
            ListView = MenuItemsListView;
        }

        class MainMasterDetailPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MasterDetailMenuItem> MenuItems { get; set; }

            public MainMasterDetailPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<MasterDetailMenuItem>(new[]
                {
                    new MasterDetailMenuItem { Id = 0, Title = "Inicio", NewPage = () => new InstitutionsMainPage()},
                    new MasterDetailMenuItem { Id = 1, Title = "Donación aleatoria", NewPage = () => new RandomDonation()},
                    new MasterDetailMenuItem { Id = 2, Title = "¿Cómo funciona?", NewPage = () => new FAQPage()}

                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}