using Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyApp
{
    public partial class MainPage : ContentPage
    {
     public static  ObservableCollection<Customer> Customers { get; set; }

        public MainPage()
        {
            InitializeComponent();

            Customers = new ObservableCollection<Customer>();
        }

        protected override void OnAppearing()
        {
            if (Customers.Count == 0)
            {
                RefreshAsync();
            }
            base.OnAppearing();
        }

        private async Task RefreshAsync()
        {
            btnAdd.IsEnabled = btnSync.IsEnabled = btnRefresh.IsEnabled = false;

            Customers = new ObservableCollection<Customer>(await Data.DatabaseRepository.Instance.FindAsync(x => true));

            MyListView.ItemsSource = Customers;

            btnAdd.IsEnabled = btnSync.IsEnabled = btnRefresh.IsEnabled = true;
        }

        private async void btnAdd_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CustomerPage());
        }

        private async void btnSync_Clicked(object sender, EventArgs e)
        {
            btnAdd.IsEnabled = btnSync.IsEnabled = btnRefresh.IsEnabled = false;
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading();

            await Services.SyncService.Instance.Sync();

            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            btnAdd.IsEnabled = btnSync.IsEnabled = btnRefresh.IsEnabled = true;
        }

        private async void btnRefresh_Clicke(object sender, EventArgs e)
        {
            await RefreshAsync();
        }

        private async void MyListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
                await Navigation.PushAsync(new CustomerPage((Customer)e.Item));

        }
    }
}
