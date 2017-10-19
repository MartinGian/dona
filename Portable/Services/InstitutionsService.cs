using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dona.Forms.Model;
using Xamarin.Forms;
using System.Linq;
using dona.Forms.Common;

namespace dona.Forms.Services
{
    public interface IInstitutionsService
    {
        Task<IList<Institution>> SyncAndGetInstitutions();
        Task<IList<Institution>> GetInstitutionsFromLocalAsync();
        Task<IList<RandomInstitution>> GetRandomInstitutionListAsync(int minimumCommonDonation, int donationAmount, bool onlyDiscountsDonationFromCredit);
    }

    public class InstitutionsService : IInstitutionsService
    {
        private static readonly IRemoteDataService RemoteDataService = AzureDataService.Instance;

        // Singleton implementation
        public static InstitutionsService Instance => new InstitutionsService();
        private InstitutionsService() { }

        public async Task Initialize()
        {
            await RemoteDataService.Initialize();
        }

        public async Task<IList<Institution>> SyncAndGetInstitutions()
        {
            if (DateTime.UtcNow - RemoteDataService.DateOfLastTimeInstitutionsUpdate > TimeSpan.FromDays(1))
                await RemoteDataService.SyncInstitutions();

            return await GetInstitutionsFromLocalAsync();
        }

        public async Task<IList<Institution>> GetInstitutionsFromLocalAsync()
        {
            var remoteInstitutions = await RemoteDataService.GetCachedInstitutionsAsync();
            var institutions = MapRemoteInstitutions(remoteInstitutions);
            institutions.Sort(new InstitutionsComprarer());
            return institutions;
        }

        public async Task<IList<RandomInstitution>> GetRandomInstitutionListAsync(int minimumCommonDonation, int donationAmount, bool onlyDiscountsDonationFromCredit)
        {
            var filteredInstitutions = await GetInstitutionsFromLocalAsync();
            if (onlyDiscountsDonationFromCredit)
            {
                filteredInstitutions = filteredInstitutions.Where(institution => institution.DonationInformation.DiscountsDonationFromCredit).ToList();
            }

            var institutionsCount = filteredInstitutions.Count;
            var donationsCount = donationAmount / minimumCommonDonation;
            var randomInstitutions = new HashSet<RandomInstitution>();

            if (donationsCount >= institutionsCount)
            {

                var ratio = donationsCount / institutionsCount;
                var donation = minimumCommonDonation * ratio;

                foreach (var institution in filteredInstitutions)
                {
                    var randomInstitution = new RandomInstitution(institution);
                    randomInstitution.Amount = donation;
                    randomInstitutions.Add(randomInstitution);
                    donationsCount -= ratio;
                }
            }

            var i = 0;
            var r = new Random();
            while (i < donationsCount)
            {
                var index = r.Next(institutionsCount);

                var institution = filteredInstitutions.ElementAt(index);
                var randomInstitution = new RandomInstitution(institution);
                randomInstitution.Amount = minimumCommonDonation;

                //If size < total, and randomInstitution not already randomly selected
                if (randomInstitutions.Count < institutionsCount && !randomInstitutions.Contains(randomInstitution))
                {
                    randomInstitutions.Add(randomInstitution);
                    i++;
                }
                //Else, keep getting random institution


                //If all institutions selected, then replace
                if (randomInstitutions.Count >= institutionsCount)
                {
                    var amount = randomInstitutions.Single(p => p.Equals(randomInstitution)).Amount;
                    randomInstitution.Amount = amount + minimumCommonDonation;
                    randomInstitutions.Remove(randomInstitution);
                    randomInstitutions.Add(randomInstitution);
                    i++;
                }
            }

            return randomInstitutions.ToList();
        }

        private static Institution MapRemoteInstitution(RemoteInstitution remoteInstitution)
        {
            var institution = new Institution();
            institution.Name = remoteInstitution.Name;
            institution.CoverSource = ImageSource.FromUri(new Uri(remoteInstitution.ImageUrl));
            institution.Presentation = remoteInstitution.Presentation;
            institution.Webpage = !string.IsNullOrEmpty(remoteInstitution.Webpage) ? new Uri(remoteInstitution.Webpage) : null;
            institution.DonationInformation = new InstitutionDonationInformation
            {
                DiscountsDonationFromCredit = bool.Parse(remoteInstitution.DiscountsDonationFromCredit),
                Message = remoteInstitution.Message,
                MinimumDonation = int.Parse(remoteInstitution.MinimumDonation),
                Number = remoteInstitution.Number
            };

            return institution;
        }

        private static List<Institution> MapRemoteInstitutions(IEnumerable<RemoteInstitution> remoteInstitutions)
        {
            return remoteInstitutions.Select(MapRemoteInstitution).ToList();
        }
    }
}