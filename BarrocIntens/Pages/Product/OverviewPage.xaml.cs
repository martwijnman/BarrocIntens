using BarrocIntens.Data;
using BarrocIntens.Pages.Product;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Product
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    
    public sealed partial class OverviewPage : Page
    {
        private List<Data.Product> AllProducts;

        public OverviewPage()
        {
            InitializeComponent();
            using (var db = new AppDbContext())
            {
                AllProducts = db.Products
                .OrderBy(p => p.Name)
                .ToList();

                var products = db.Products.ToList();
                ProductView.ItemsSource = products;
                //FilterComboBox.ItemsSource = db.Genres.OrderBy(g => g.Name).ToList();
            }
            //ShowStockEmpty(this.XamlRoot);
        }

        private Dictionary<Data.Product, int> wallet = new();

        private int timesSelected = 0;
        private void PlusClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (Data.Product)button.DataContext;

            if (product.Stock <= 0)
                return;

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

            if (!wallet.ContainsKey(product))
                return;

            wallet[product]--;
            product.Stock++;

            if (wallet[product] <= 0)
                wallet.Remove(product);
        }

        private async void WalletError()
        {
            var dialog = new ContentDialog
            {
                Title = "Melding",
                Content = "U heeft nog geen producten geselecteerd",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }
        private void GoToOrder(object sender, RoutedEventArgs e)
        {
            if (wallet.Count > 0)
            {
                Frame.Navigate(typeof(OrderPage), wallet);
            }
            else
            {
                WalletError();
            }
        }
        private void ProductClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (Data.Product)button.DataContext;

            Frame.Navigate(typeof(DetailPage), product);
        }
        private void Filter_Changed(object sender, object e)
        {
            using (var db = new AppDbContext())
            {
                var query = db.Products.AsQueryable();


                if (ProductSearchTextbox.Text != "")
                {
                    string search = ProductSearchTextbox.Text;
                    query = query.Where(g => g.Name.Contains(search));
                }

                if (StockCheckbox.SelectedValue as string == "Op voorraad")
                {
                    var productIds = db.Products
                                    .Where(p => p.Stock >= 1)
                                    .Select(p => p.Id);
                    query = query.Where(p => productIds.Contains(p.Id));
                }
                if (StockCheckbox.SelectedValue as string == "Niet op voorraad")
                {
                    var productIds = db.Products
                                    .Where(p => p.Stock == 0)
                                    .Select(p => p.Id);
                    query = query.Where(p => productIds.Contains(p.Id));
                }


                if (PriceFilter.Value > 0)
                {
                    query = query.Where(p => p.Price >= PriceFilter.Value);
                }
               

                ProductView.ItemsSource = query.ToList();
            }
        }
        private async void ShowStockEmpty(Microsoft.UI.Xaml.XamlRoot root)
        {
            var db = new AppDbContext();

            var products = db.Products
                .Where(p => p.Stock < p.MinimumStock)
                .ToList();

            if (!products.Any())
                return;

            string content = "⚠ Producten met te lage voorraad:\n\n";

            foreach (var product in products)
            {
                content += $"• {product.Name} → bestel {product.MinimumStock - product.Stock} stuks\n";
            }

            var dialog = new ContentDialog
            {
                Title = "Voorraad waarschuwing",
                Content = content,
                CloseButtonText = "Sluiten",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }


    }
}
