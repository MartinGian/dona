using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;

namespace dona.Forms.Model
{
    public class RemoteInstitution
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Presentation")]
        public string Presentation { get; set; }

        [JsonProperty(PropertyName = "Webpage")]
        public string Webpage { get; set; }

        [JsonProperty(PropertyName = "Number")]
        public string Number { get; set; }

        [JsonProperty(PropertyName = "Message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "MinimumDonation")]
        public string MinimumDonation { get; set; }

        [JsonProperty(PropertyName = "DiscountsDonationFromCredit")]
        public string DiscountsDonationFromCredit { get; set; }

        [JsonProperty(PropertyName = "ImageUrl")]
        public string ImageUrl { get; set; }

        [Version]
        public string AzureVersion { get; set; }
    }
}
