using System;
using dona.Forms.Model;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace dona.Forms.Views.MasterDetail
{
    public partial class MainMasterDetailPage : MasterDetailPage
    {
        private MasterDetailMenuItem _lastSelectedItem;

        public MainMasterDetailPage()
        {
            InitializeComponent();

            MasterPage.ListView.ItemSelected += (sender, e) => { ((ListView)sender).SelectedItem = null; };
            MasterPage.ListView.ItemSelected += async (o, args) => await ListView_ItemSelected(o, args);

            // just for initialize the first time
            _lastSelectedItem = new MasterDetailMenuItem { Id = 0 };
        }

        private async Task ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterDetailMenuItem;
            if (item == null)
                return;

            if (_lastSelectedItem.Id != item.Id)
            {
                var page = await Task.Run(() => item.NewPage());
                Detail = new NavigationPage(page);
                _lastSelectedItem = item;
            }

            IsPresented = false;
        }
    }
}