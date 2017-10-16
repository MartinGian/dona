using dona.Forms.Model;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MainMasterDetailPage = dona.Forms.Views.MasterDetail.MainMasterDetailPage;
using Plugin.Share.Abstractions;
using Plugin.Share;

namespace dona.Forms.ViewModels
{
    public class DonationResultPageViewModel
    {
        public string DonationMessage { get; }

        private readonly IList<DonationResult> _donationResults;

        public Command GoToHomePage { get; }
        public Command Publish { get; }

        public bool ShareButtonVisible => CrossShare.IsSupported;

        public DonationResultPageViewModel(IList<DonationResult> donationResults)
        {
            _donationResults = donationResults;

            DonationMessage = GetDonationMessage(donationResults);
            GoToHomePage = new Command(GoBackToMainPage);
            Publish = new Command(PublishDonation);
        }

        public void GoBackToMainPage()
        {
            Application.Current.MainPage = new MainMasterDetailPage();
        }

        public void PublishDonation()
        {
            string institutionNameOrEmpty = _donationResults.Count == 1 ? $" a {_donationResults.First().Institution.Name}" : string.Empty;
            CrossShare.Current.Share(new ShareMessage
            {
                Title = "Doná Uruguay!",
                Text = $"Acabo de donar{institutionNameOrEmpty} a través de Doná. ¡Vos también podes donar! Descargala de Google Play. Visitá nuestra página http://fb.com/donauruguay",
                Url = "http://play.google.com/store/apps/details?id=donauruguay"
            });
        }

        private static string GetDonationMessage(IList<DonationResult> results)
        {
            var sb = new StringBuilder();

            foreach (var result in results)
            {
                if (result.DonatedAmount == result.SelectedAmount)
                {
                    sb.AppendLine($"¡Donaste ${result.DonatedAmount} a {result.Institution.Name}!");
                }
                else if (result.DonatedAmount > 0)
                {
                    sb.AppendLine($"Tu saldo fue suficiente para donar ${result.DonatedAmount} a {result.Institution.Name}");
                }
                else
                {
                    sb.AppendLine($"No hemos podido realizar la donación a {result.Institution.Name}. Chequea tu saldo o tu línea y vuelve a intentarlo.");
                }
            }

            return sb.ToString();
        }
    }
}