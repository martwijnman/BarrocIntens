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
        public OverviewPage()
        {
            InitializeComponent();
            using (var db = new AppDbContext())
            {
                var products = db.Products.ToList();
                ProductView.ItemsSource = products;
            }
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

        private void GoToOrder(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(OrderPage), wallet);
        }
        private void ProductClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (Data.Product)button.DataContext;

            Frame.Navigate(typeof(DetailPage), product);
        }
    }
}
