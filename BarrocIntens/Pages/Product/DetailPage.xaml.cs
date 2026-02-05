using BarrocIntens.Data;
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
using Microsoft.UI.Xaml.Media.Imaging;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Product
{
    public sealed partial class DetailPage : Page
    {

        public DetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is int productId)
            {
                var db = new AppDbContext();

                    LoadProduct(productId);
                
            }
        }

        private void LoadProduct(int productId)
        {
            var db = new AppDbContext();
            var product = db.Products.FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                // Optional: Show an error or navigate back
                return;
            }



            ProductImage.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{product.Image}", UriKind.RelativeOrAbsolute));
            NameText.Text = product.Name;
            CategoryText.Text = product.Category;
            PriceText.Text = $"€{product.Price:F2}";
            StockText.Text = product.Stock.ToString();
            MinimumStockText.Text = $"(Minimum: {product.MinimumStock})";
            StockWarning.Visibility = product.Stock < product.MinimumStock ? Visibility.Visible : Visibility.Collapsed;
            DelivererText.Text = product.Deliverer;
            IsMachineText.Text = product.IsMachine ? "Yes" : "No";
            NotificationText.Text = product.NotificationOutOfStock ? "Yes" : "No";
            ExtraInfo.Text = product.ExtraInformation;


        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            // Frame.Navigate(typeof(EditProductPage), CurrentProduct.Id);
        }
    }

}
