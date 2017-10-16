using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using dona.Forms.Model;
using dona.Forms.Services;
using dona.Forms.Views;
using Xamarin.Forms;

namespace dona.Forms.ViewModels
{
    public class InstitutionsMainPageViewModel : INotifyPropertyChanged
    {
        public static InstitutionsMainPageViewModel Instance = new InstitutionsMainPageViewModel();
        private readonly IInstitutionsService _institutionsService;

        private IList<Institution> _institutions;
        public IList<Institution> Institutions
        {
            get => _institutions;
            private set
            {
                _institutions = value;
                OnPropertyChanged();
            }
        }

        public ICommand InstitutionSelected { get; }
        public ICommand LoadInstitutions { get; }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        private InstitutionsMainPageViewModel()
        {
            _institutionsService = InstitutionsService.Instance;
            InstitutionSelected = new Command(async param => await InstitutionSelected_Handler(param));
            LoadInstitutions = new Command(async () => await LoadInstitutions_Handler());
            Institutions = new List<Institution>();
        }

        public void InitializeInstitutionsFromLocal()
        {
            Institutions = _institutionsService.GetInstitutionsFromLocal();
        }

        private static async Task InstitutionSelected_Handler(object param)
        {
            var institution = param as Institution;

            var mdp = Application.Current.MainPage as MasterDetailPage;
            if (mdp != null)
                await mdp.Detail.Navigation.PushAsync(new InstitutionDetailsPage(institution));
        }

        private async Task LoadInstitutions_Handler()
        {
            if (!IsRefreshing)
            {
                IsRefreshing = true;
                try
                {
                    var institutions = await Task.Run(async () => await _institutionsService.SyncAndGetInstitutions());
                    Institutions = institutions;
                }
                finally
                {
                    IsRefreshing = false;
                }
            }
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
