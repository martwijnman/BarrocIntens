using BarrocIntens.Data;
using BarrocIntens.Pages.EmployeesCreation;
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
    public sealed partial class CustomerView : Page
    {
        public CustomerView()
        {
            InitializeComponent();

            ApplyFilters(); //Load all data beforehand
        }

        private void customersNameSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyFilters();

        private void ApplyFilters()
        {
            using var db = new AppDbContext();

            var nameFilter = customersNameSearchTextBox.Text?.ToLower() ?? string.Empty;

            var query = db.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(c => c.Name.ToLower().Contains(nameFilter));

            customersListView.ItemsSource = query
                .ToList();
        }

        private void customersListView_ItemClick(object sender, ItemClickEventArgs c)
        {
            var customer = (BarrocIntens.Data.Customer)c.ClickedItem;
            Frame.Navigate(typeof(Customers.CustomerDetailPage), customer.Id);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var selectedCustomer = (Customer)button.DataContext;

            if (selectedCustomer == null)
            {
                // This should never happen now
                return;
            }

            Frame.Navigate(typeof(UpdateCustomerPage), selectedCustomer);
        }
    }
}