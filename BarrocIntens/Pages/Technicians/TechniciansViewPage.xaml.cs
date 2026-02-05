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
using System.Linq;

namespace BarrocIntens.Pages.Technicians
{
    public sealed partial class TechniciansViewPage : Page
    {
        // Wallet to track selected products
        private Dictionary<Data.Product, int> wallet = new();

        public TechniciansViewPage()
        {
            InitializeComponent();

            // Load technicians
            ApplyFilters();

            // Load only the toolkit product for the Technicians page
            LoadToolkitProduct();
        }

        // =====================
        // Technicians Filtering
        // =====================
        private void TechniciansNameSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyFilters();

        private void ApplyFilters()
        {
            using var db = new AppDbContext();

            var nameFilter = TechniciansNameSearchTextBox.Text?.ToLower() ?? string.Empty;

            var query = db.Employees
                .Where(c => c.Department == "Maintenance")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(c => c.Name.ToLower().Contains(nameFilter));

            TechniciansListView.ItemsSource = query.ToList();
        }

        private void TechniciansListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var employee = (Employee)e.ClickedItem;
            Frame.Navigate(typeof(EmployeesDetailPage), employee.Id);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var selectedEmployee = (Employee)button.DataContext;

            if (selectedEmployee == null) return;

            Frame.Navigate(typeof(EmployeesUpdatePage), selectedEmployee);
        }

        private void AddCalendarItemButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.Planning.CreatePage));
        }

        private async void DayItemListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedCalendarItem = (Data.Planning)e.ClickedItem;
            int clickedCalendarItemId = clickedCalendarItem.Id;
            Frame.Navigate(typeof(Pages.Planning.DetailPage), clickedCalendarItemId);
        }

        // =====================
        // Toolkit Product Logic
        // =====================
        private void LoadToolkitProduct()
        {
            using var db = new AppDbContext();

            //All the products pulled into this page need to be from the catagory 'Tools', 
            //because the tools are the products that the Technicians use.

            // This pulls every product with the catagory 'Tools'.
            var toolkit = db.Products
                            .Where(p => p.Category == "Tools")
                            .ToList();

            ProductView.ItemsSource = toolkit;
            ShowToolkitStockWarning();
        }

        private async void ShowToolkitStockWarning()
        {
            using var db = new AppDbContext();
            var toolkit = db.Products.FirstOrDefault(p => p.Category == "Tools");
            if (toolkit == null) return;

            if (toolkit.Stock <= 0)
            {
                var dialog = new ContentDialog
                {
                    Title = "Waarschuwing",
                    Content = "⚠ Toolkit is uitverkocht!",
                    CloseButtonText = "Sluiten",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }

        // =====================
        // Product Buttons
        // =====================
        private void PlusClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (Data.Product)button.DataContext;

            if (product.Stock <= 0) return;

            product.Stock--;

            if (wallet.ContainsKey(product))
                wallet[product]++;
            else
                wallet[product] = 1;
        }

        private void MinClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (Data.Product)button.DataContext;

            if (!wallet.ContainsKey(product)) return;

            wallet[product]--;
            product.Stock++;

            if (wallet[product] <= 0)
                wallet.Remove(product);
        }

        private void ProductClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (Data.Product)button.DataContext;

            // Navigate to the Product DetailPage
            Frame.Navigate(typeof(Pages.Product.DetailPage), product.Id);
        }
    }
}
