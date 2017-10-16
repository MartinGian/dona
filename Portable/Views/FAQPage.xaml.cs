using System;
using Acr.UserDialogs;
using Xamarin.Forms;

namespace dona.Forms.Views
{
    public partial class FAQPage : ContentPage
    {
        public FAQPage()
        {
            InitializeComponent();
        }

        private void GoToFacebookPage(object sender, EventArgs e)
        {
            try
            {
                Device.OpenUri(new Uri("fb://page/2170143916545877"));
            }
            catch (Exception)
            {
                try
                {
                    Device.OpenUri(new Uri("https://www.facebook.com/donauruguay"));
                }
                catch (Exception)
                {
                    UserDialogs.Instance.Alert("Error al abrir el link de nuestra fanpage facebook.com/donauruguay");
                }
            }
        }
    }
}