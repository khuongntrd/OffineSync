using Shared;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

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

        public CustomerPage():this(new Shared.Customer())
        {

        }

        private void btnSave_Clicked(object sender, EventArgs e)
        {

        }

        private void btnSaveSync_Clicked(object sender, EventArgs e)
        {

        }
    }
}
