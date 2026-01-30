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
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

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
            }
            this.Loaded += OverviewPage_Loaded;


        }

        private Dictionary<Data.Product, int> wallet = new();

        private int timesSelected = 0; 

        private void OverviewPage_Loaded(object sender, RoutedEventArgs e)
        {
            Errors("emptystock");
        }

        private void GoCreate_Button(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.Product.CreatePage));
        }

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




        private async Task Errors(string type)
        {
            

            string error = "";


            if (type == "emptywallet")
            {
                error = "U heeft nog geen producten geselecteerd";
            }
            else if (type == "emptystock")
            {
                using var db = new AppDbContext();

                var products = db.Products
                    .Where(p => p.NotificationOutOfStock == true)
                    .ToList();

                if (!products.Any())
                    return;

                error = "⚠ Producten met te lage voorraad:\n\n";

                foreach (var product in products)
                {
                    int buy = product.MinimumStock - product.Stock;
                    error += $"• {product.Name} → bestel {buy} stuks\n";
                    Console.WriteLine(error);
                }
            }


            var dialog = new ContentDialog
            {
                Title = "Waarschuwing",
                Content = error,
                CloseButtonText = "Sluiten",
                XamlRoot = this.XamlRoot
            };
            if (this.XamlRoot == null || string.IsNullOrWhiteSpace(error))
                return;
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
                Errors("emptywallet");
            }
        }

        private void ProductClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (Data.Product)button.DataContext;

            Frame.Navigate(typeof(DetailPage), product.Id);
        }
        private void ProductSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    => ApplyFilters();

        private void ApplyFilters()
        {
            using var db = new AppDbContext();

            var nameFilter = ProductSearchTextBox.Text?.ToLower() ?? string.Empty;

            var query = db.Products.AsQueryable();



            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(c => c.Name.ToLower().Contains(nameFilter));


            if (!double.IsEvenInteger(0))
            {
                query = query.Where(p => p.Price <= PriceFilter.Value);
            }

            ProductView.ItemsSource = query
                .ToList();
        }

        private void Filter_Changed(object sender, object e)
        {
            using (var db = new AppDbContext())
            {
                var query = db.Products.AsQueryable();

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
                ProductView.ItemsSource = query.ToList();
            }
        }
    }
}
