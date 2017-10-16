using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace dona.Forms.Model
{
    public class RandomInstitution : Institution
    {
        public RandomInstitution(Institution institution)
        {
            Name = institution.Name;
            Presentation = institution.Presentation;
            Webpage = institution.Webpage;
            CoverSource = institution.CoverSource;
            DonationInformation = institution.DonationInformation;
        }

        public int Amount { get; set; }
        public string FormattedAmount => $"${Amount}";
        public string FormattedName => $"{Name} {(!DonationInformation.DiscountsDonationFromCredit ? "*" : string.Empty)}";
    }
}