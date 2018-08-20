using MyApp.Data;
using Shared;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerPage : ContentPage
    {
        public Customer Customer { get; set; }
        public CustomerPage(Shared.Customer item)
        {
            InitializeComponent();

            Customer = item;

            BindingContext = Customer;
        }

        public CustomerPage() : this(new Shared.Customer())
        {

        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Customer.Id) || Customer.LastUpdated > DateTime.MinValue)
            {
                await DatabaseRepository.Instance.UpdateAsync(Customer);
            }
            else
                await DatabaseRepository.Instance.InsertAsync(Customer);
        }

        private void btnSaveSync_Clicked(object sender, EventArgs e)
        {

        }
    }
}
