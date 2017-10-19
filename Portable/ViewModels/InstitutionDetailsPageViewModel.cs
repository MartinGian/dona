using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Acr.UserDialogs;
using dona.Forms.Model;
using dona.Forms.Services;
using System.Collections.Generic;
using dona.Forms.Exceptions;
using dona.Forms.Views;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace dona.Forms.ViewModels
{
    public class InstitutionDetailsPageViewModel : INotifyPropertyChanged
    {
        private readonly CreditService _creditService;
        private readonly DonationsService _donationsService;

        public Institution Institution { get; }

        public string DonateBtnText => IsDonating ? "Donando..." : $"Donar ${_donationAmount}";
        public string SubstractFromDonationAmountFormatted => $"- ${Institution.DonationInformation.MinimumDonation}";
        public string AddToDonationAmountFormatted => $"+ ${Institution.DonationInformation.MinimumDonation}";

        #region Commands

        public Command AddToDonationAmount { get; }
        public Command SubstractFromDonationAmount { get; }
        public Command Donate { get; }
        public Command GetCredit { get; }

        #endregion

        #region View Properties

        private bool _isDonating;
        public bool IsDonating
        {
            get => _isDonating;
            set
            {
                _isDonating = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DonateBtnText));
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
                OnPropertyChanged(nameof(DonateBtnText));
                AddToDonationAmount.ChangeCanExecute();
                SubstractFromDonationAmount.ChangeCanExecute();
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

        public InstitutionDetailsPageViewModel(Institution institution)
        {
            Institution = institution;
            _isGettingCredit = false;
            _donationAmount = institution.DonationInformation.MinimumDonation;
            _creditService = CreditService.Instance;
            _donationsService = new DonationsService();

            AddToDonationAmount = new Command(() => DonationAmount += Institution.DonationInformation.MinimumDonation, AddDonationAmountCommandCanExecute);
            SubstractFromDonationAmount = new Command(() => DonationAmount -= Institution.DonationInformation.MinimumDonation, () => DonationAmount > Institution.DonationInformation.MinimumDonation);
            Donate = new Command(async () => await Donate_Handler(), () => !IsDonating);
            GetCredit = new Command(async () => await GetCredit_Handler());
        }

        private bool AddDonationAmountCommandCanExecute()
        {
            if (Credit == null) return true;
            if (Credit is UnlimitedCredit) return true;
            return DonationAmount + Institution.DonationInformation.MinimumDonation <= Credit.Amount;
        }

        private async Task Donate_Handler()
        {
            IsDonating = true;
            Donate.ChangeCanExecute();

            var ok = await UserDialogs.Instance.ConfirmAsync($"Está a punto de realizar una donación de ${DonationAmount}.", "Confirmar donación", "Donar", "Cancelar");
            if (ok)
            {
                try
                {
                    var donationResult = await _donationsService.DonateAsync(Institution, DonationAmount);

                    var mdp = Application.Current.MainPage as MasterDetailPage;
                    if (mdp != null)
                        await mdp.Detail.Navigation.PushAsync(
                            new DonationResultPage(new List<DonationResult> { donationResult }));
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
            else
            {
                IsDonating = false;
                Donate.ChangeCanExecute();
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
