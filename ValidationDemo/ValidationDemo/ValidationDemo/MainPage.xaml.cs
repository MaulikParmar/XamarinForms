using System;
using Xamarin.Forms;

namespace ValidationDemo
{
    public partial class MainPage : ContentPage
    {
        private Customer objCustomer;

        public MainPage()
        {
            InitializeComponent();

            // Customer
            objCustomer = new Customer();
            this.BindingContext = objCustomer;
        }

        private void OnClicked(object sender, EventArgs e)
        {
            objCustomer.Validate();

            if (objCustomer.HasErrors)
            {
                // Error message
                objCustomer.ScrollToControlProperty(objCustomer.GetFirstInvalidPropertyName);
            }
            else
            {
                // No error
            }
        }
    }
}
