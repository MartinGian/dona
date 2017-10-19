using System;
using Xamarin.Forms;

namespace dona.Forms.Model
{
    public class Institution
    {
        public string Name { get; set; }
        public string Presentation { get; set; }
        public Uri Webpage { get; set; }
        public ImageSource CoverSource { get; set; }
        public InstitutionDonationInformation DonationInformation { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as Institution;

            if (item == null)
            {
                return false;
            }

            return DonationInformation.Number.Equals(item.DonationInformation.Number);
        }

        public override int GetHashCode()
        {
            return DonationInformation.Number.GetHashCode();
        }
    }
}