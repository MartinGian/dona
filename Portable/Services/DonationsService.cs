using System.Threading.Tasks;
using dona.Forms.Model;
using System.Collections.Generic;

namespace dona.Forms.Services
{
    public class DonationsService
    {
        private readonly ISmsService _smsService;

        public DonationsService()
        {
            _smsService = new SmsService();
        }

        public async Task<DonationResult> DonateAsync(Institution institution, int amount)
        {
            var donationResult = new DonationResult { DonatedAmount = 0, Institution = institution, SelectedAmount = amount };

            await Task.Run(async () =>
            {
                var smsCount = amount / institution.DonationInformation.MinimumDonation;
                for (var i = 1; i <= smsCount; i++)
                {
                    await _smsService.SendSmsAsync(institution.DonationInformation.Number, institution.DonationInformation.Message);
                    donationResult.DonatedAmount += institution.DonationInformation.MinimumDonation;
                }
            });

            return donationResult;
        }

        public async Task<IList<DonationResult>> DonateInstitutionsAsync(IList<RandomInstitution> institutions)
        {
            var donationResults = new List<DonationResult>();

            foreach (var institution in institutions)
            {
                donationResults.Add(await this.DonateAsync(institution, institution.Amount));
            }

            return donationResults;
        }
    }
}
