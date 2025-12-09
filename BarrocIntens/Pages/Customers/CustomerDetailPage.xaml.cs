using BarrocIntens.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Customers
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomerDetailPage : Page
    {
        private int customerId;
        public CustomerDetailPage()
        {
            using (var db = new Data.AppDbContext())
            {
                InitializeComponent();

                FactureListView.ItemsSource = db.Factures
                    .Include(f => f.Quote)
                    .ThenInclude(q => q.Customer)
                    .Where(f => f.Quote.CustomerId == customerId)
                    .ToList();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter == null)
            {
                // Safety: avoid blank page if something goes wrong
                CustomerNameTextBlock.Text = "Geen klant geselecteerd";
                return;
            }

            customerId = (int)e.Parameter;
            LoadCustomerDetails();
        }

        private void LoadCustomerDetails()
        {
            using var db = new AppDbContext();
            var customer = db.Customers
                .Where(e => e.Id == customerId)
                .Select(customer => new
                {
                    customer.Name,
                    customer.Email,
                    customer.PhoneNumber,
                    customer.City,
                    customer.BkrStatus
                })
                .FirstOrDefault();

            if (customer == null)
            {
                CustomerNameTextBlock.Text = "Klant niet gevonden";
                return;
            }

            CustomerNameTextBlock.Text = $"Naam: {customer.Name}";
            CustomerEmailTextBlock.Text = $"Email: {customer.Email}";
            CustomerPhoneTextBlock.Text = $"Telefoon: {customer.PhoneNumber}";
            CustomerCityTextBlock.Text = $"Stad: {customer.City}";
            CustomerBkrStatusTextBlock.Text = $"BKR status: {customer.BkrStatus}";
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
