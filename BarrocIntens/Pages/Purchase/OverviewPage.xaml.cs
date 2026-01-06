using BarrocIntens.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Purchase;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class OverviewPage : Page
{
    public OverviewPage()
    {
        InitializeComponent();
        var db = new Data.AppDbContext();
        var products = db.Products.OrderBy(p => p.NotificationOutOfStock).Include(p => p.Deliverer).ToList();
        var matrials = db.Matrials.OrderBy(p => p.NotificationOutOfStock).ToList();
        var deliverers = db.Deliverers.Include(d => d.ProductDeliverer).ThenInclude(pd => pd.Product).Where(d => d.ProductDeliverer.Product.Stock < d.ProductDeliverer.Product.MinimumStock).ToList();

        DelivererListView.ItemsSource = deliverers;
        MatrialListView.ItemsSource = matrials;
        ProductListView.ItemsSource = products;
    }
    private void CreateProduct_Button(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(Pages.Purchase.CreatePage));
    }
    private void OrderASpecificMatrial_Button(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.DataContext is not Data.Product product)
            return;


        using var db = new Data.AppDbContext();

        var matrial = db.Matrials.First(p => p.Id == product.Id);
        matrial.Stock += 1;

        db.SaveChanges();
    }
    private void OrderProduct_Click(object sender, RoutedEventArgs e)
    {
        var product = (sender as Button)?.DataContext as Data.Matrial;

        if (product != null)
        {
            
        }
    }


    // alle producten bestellen

    //check goedkeuring
    private async Task OrderProductList()
    {
        using var db = new AppDbContext();

        var matrials = db.Matrials
            .Where(p => p.MinimumStock > p.Stock)
            .ToList();

        if (!matrials.Any() || this.XamlRoot == null)
            return;

        string message = "Weet u zeker dat u deze producten wilt bestellen?\n\n";

        foreach (var matrial in matrials)
        {
            int buy = matrial.MinimumStock - matrial.Stock;
            message += $"• {matrial.Name} → bestel {buy} stuks\n";
        }

        var dialog = new ContentDialog
        {
            Title = "Waarschuwing ⚠️",
            Content = message,
            PrimaryButtonText = "Accepteren",
            SecondaryButtonText = "Weigeren",
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();


        // acceoteren
        if (result == ContentDialogResult.Primary)
        {
            foreach (var matrial in matrials)
            {
                matrial.Stock = matrial.MinimumStock;
            }

            await db.SaveChangesAsync();
        }

        // weigeren
        else
        {
            return;
        }
    }


    // uitvoering
    private void Order_All_Click(object sender, RoutedEventArgs e)
    {
        // check goedkeuring
        OrderProductList();

    }
}
