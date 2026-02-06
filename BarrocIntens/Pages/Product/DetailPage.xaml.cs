using BarrocIntens.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
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
            var product = db.Products.Include(p => p.Deliverer).FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                // Optional: Show an error or navigate back
                return;
            }


            // product d4tails
            ProductImage.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{product.Image}", UriKind.RelativeOrAbsolute));
            NameText.Text = product.Name;
            CategoryText.Text = product.Category;
            PriceText.Text = $"�{product.Price:F2}";
            StockText.Text = product.Stock.ToString();
            MinimumStockText.Text = $"(Minimum: {product.MinimumStock})";
            StockWarning.Visibility = product.Stock < product.MinimumStock ? Visibility.Visible : Visibility.Collapsed;
            DelivererText.Text = product.Deliverer.Name;
            IsMachineText.Text = product.IsMachine ? "Machine" : "Onderdeel";
            NotificationText.Text = product.NotificationOutOfStock ? "Yes" : "No";
            ExtraInfo.Text = product.ExtraInformation;



            // matrials for the product
            var matrials = db.ProductMaterials
                .Include(c => c.Material)
                .Where(p => p.ProductId == product.Id)
                .ToList()
                .Select(m => new
                {
                    Name = m.Material.Name,
                    Image = $"Assets/Materials/{m.Material.Id}.png", // bv. 1.png, 2.png...
                    StockText = $"Stock: {m.Material.Stock}",
                    StockColor = m.Material.Stock < m.Material.MinimumStock
                        ? Colors.Red
                        : Colors.Green
                });

            MaterialsList.ItemsSource = matrials;
        }
        private void GoBack_Button(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            // Frame.Navigate(typeof(EditProductPage), CurrentProduct.Id);
        }
    }

}
