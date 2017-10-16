using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Acr.UserDialogs;
using dona.Forms.Exceptions;
using dona.Forms.Model;
using dona.Forms.Services;
using dona.Forms.Views;
using Xamarin.Forms;

namespace dona.Forms.ViewModels
{
    public class RandomDonationPageViewModel : INotifyPropertyChanged
    {

        private readonly CreditService _creditService;
        private readonly DonationsService _donationsService;
        private int _minCommonDonationAmount = 10;
        private readonly IInstitutionsService _institutionsService;

        private IList<RandomInstitution> _randomInstitutions;
        public IList<RandomInstitution> RandomInstitutions
        {
            get => _randomInstitutions;
            private set
            {
                _randomInstitutions = value;
                OnPropertyChanged();
            }
        }

        public string GenerarDonacionFormatted => $"Generar donación de ${_donationAmount}";
        public string DonationAmountFormatted => IsDonating ? "Donando..." : $"Donar ${_generatedDonationAmount}";
        public string SubstractFromDonationAmountFormatted => $"- ${_minCommonDonationAmount}";
        public string AddToDonationAmountFormatted => $"+ ${_minCommonDonationAmount}";


        #region Commands

        public Command AddToDonationAmount { get; }
        public Command SubstractFromDonationAmount { get; }
        public Command Donate { get; }
        public Command GenerateDonation { get; }
        public Command GetCredit { get; }

        #endregion

        #region View Properties



        private bool _onlyDiscountsDonationFromCredit;
        public bool OnlyDiscountsDonationFromCredit
        {
            get => _onlyDiscountsDonationFromCredit;
            set
            {
                _onlyDiscountsDonationFromCredit = value;
                OnPropertyChanged();
            }
        }

        private bool _isDonating;
        public bool IsDonating
        {
            get => _isDonating;
            set
            {
                _isDonating = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DonationAmountFormatted));
            }
        }

        private int _donationAmount;
        public int DonationAmount
        {
            get => _donationAmount;
            set
            {
                _donationAmount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(GenerarDonacionFormatted));
                AddToDonationAmount.ChangeCanExecute();
                Donate.ChangeCanExecute();
                SubstractFromDonationAmount.ChangeCanExecute();
            }
        }

        private int _generatedDonationAmount;
        public int GeneratedDonationAmount
        {
            get => _generatedDonationAmount;
            set
            {
                _generatedDonationAmount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DonationAmountFormatted));
            }
        }

        private Credit _credit;
        public Credit Credit
        {
            get => _credit;
            set
            {
                if (value != null)
                {
                    _credit = value;
                    CreditButtonVisible = false;
                    CreditLabelVisible = true;
                    OnPropertyChanged();
                }
            }
        }

        private bool _creditButtonVisible = true;
        public bool CreditButtonVisible
        {
            get => _creditButtonVisible;
            set
            {
                _creditButtonVisible = value;
                OnPropertyChanged();
            }
        }

        private bool _isGettingCredit;
        public bool IsGettingCredit
        {
            get => _isGettingCredit;
            set
            {
                _isGettingCredit = value;
                OnPropertyChanged();
            }
        }

        private bool _creditLabelVisible;
        public bool CreditLabelVisible
        {
            get => _creditLabelVisible;
            set
            {
                _creditLabelVisible = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public RandomDonationPageViewModel()
        {
            _isGettingCredit = false;
            _donationAmount = 10;
            _onlyDiscountsDonationFromCredit = true;
            _creditService = CreditService.Instance;
            _donationsService = new DonationsService();
            _institutionsService = InstitutionsService.Instance;

            if (_onlyDiscountsDonationFromCredit)
                CheckCacheCredit();

            AddToDonationAmount = new Command(() => DonationAmount += _minCommonDonationAmount, AddDonationAmountCommandCanExecute);
            SubstractFromDonationAmount = new Command(() => DonationAmount -= _minCommonDonationAmount, () => DonationAmount > _minCommonDonationAmount);
            Donate = new Command(async () => await Donate_Handler(), () => !IsDonating);
            GetCredit = new Command(async () => await GetCredit_Handler(), () => !_isGettingCredit);
            GenerateDonation = new Command(Generate_Donation_Handler);
        }

        private bool AddDonationAmountCommandCanExecute()
        {
            if (Credit == null) return true;
            if (Credit is UnlimitedCredit) return true;
            return DonationAmount + _minCommonDonationAmount <= Credit.Amount;
        }

        private void CheckCacheCredit()
        {
            var credit = _creditService.GetCachedCredit();
            if (credit != null)
            {
                Credit = credit;
                CreditButtonVisible = false;
                CreditLabelVisible = true;
                AddToDonationAmount.ChangeCanExecute();
            }
        }

        private async Task Donate_Handler()
        {

            var ok = await UserDialogs.Instance.ConfirmAsync(
                $"Está a punto de realizar una donación de ${GeneratedDonationAmount}.", "Confirmar donación", "Donar", "Cancelar");

            if (ok)
            {

                IsDonating = true;
                Donate.ChangeCanExecute();
                

                try
                {
                    var donationResult = await _donationsService.DonateInstitutionsAsync(_randomInstitutions);
                    var mdp = Application.Current.MainPage as MasterDetailPage;
                    if (mdp != null)
                        await mdp.Detail.Navigation.PushAsync(new DonationResultPage(donationResult));
                    UserDialogs.Instance.HideLoading();
                }
                catch (CannotSendSmsException ex)
                {
                    UserDialogs.Instance.ShowError(ex.Message);
                }
                catch (Exception)
                {
                    UserDialogs.Instance.ShowError("Error al procesar su donación, vuelva a intentarlo");
                }
                finally
                {
                    IsDonating = false;
                    Donate.ChangeCanExecute();
                }
            }
        }

        private void Generate_Donation_Handler()
        {

            UserDialogs.Instance.ShowLoading("Estamos generando su donación", MaskType.Black);

            try
            {
                GeneratedDonationAmount = DonationAmount;
                RandomInstitutions = _institutionsService.GetRandomInstitutionList(_minCommonDonationAmount, _donationAmount, _onlyDiscountsDonationFromCredit);
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception)
            {
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.ShowError("Error al generar su donación, vuelva a intentarlo");
            }
        }

        private static readonly Action GetCreditErrorMessage = () => UserDialogs.Instance.ShowError("No se ha podido obtener el saldo. Verifique si enviando SALDO al 226 recibe un mensaje con el saldo correspondiente", timeoutMillis: 5 * 1000);

        private async Task GetCredit_Handler()
        {
            IsGettingCredit = true;
            CreditButtonVisible = false;

            try
            {
                var credit = await _creditService.GetCreditAsync();
                if (credit == null)
                {
                    GetCreditErrorMessage();
                    CreditButtonVisible = true;
                }
                else
                    Credit = credit;
            }
            catch (CannotSendSmsException ex)
            {
                CreditButtonVisible = true;
                UserDialogs.Instance.ShowError(ex.Message);
            }
            catch (Exception)
            {
                CreditButtonVisible = true;
                GetCreditErrorMessage();
            }
            finally
            {
                IsGettingCredit = false;
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
