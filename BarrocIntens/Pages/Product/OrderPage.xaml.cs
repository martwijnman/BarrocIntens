using BarrocIntens.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PdfSharp.Drawing;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Quality;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static System.Runtime.InteropServices.JavaScript.JSType;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Product;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class OrderPage : Page
{
    public OrderPage()
    {
        InitializeComponent();
        System.Diagnostics.Debug.WriteLine("WALLET COUNT: " + wallet.Count);
    }

    private Dictionary<Data.Product, int> wallet = new Dictionary<Data.Product, int>();

    private void RefreshWallet()
    {
        

        WalletListView.ItemsSource = null;

        WalletListView.ItemsSource = wallet.Select(kvp => new
        {
            Product = kvp.Key,
            Aantal = kvp.Value
        }).ToList();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is Dictionary<Data.Product, int> passedWallet)
        {
            wallet = passedWallet;

            RefreshWallet();
        }


        // Database kan je nog gebruiken indien nodig
        var db = new Data.AppDbContext();
    }
    private void GenerateOrder()
    {
        // source: Itslearning
        if (wallet.Count == 0)
            return; // niets om te genereren

        PdfDocument document = new PdfDocument();
        document.Info.Title = "Factuur";

        // Nieuwe pagina
        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);

        // Fonts
        XFont titleFont = new XFont("Arial", 20);
        XFont headerFont = new XFont("Arial", 14);
        XFont bodyFont = new XFont("Arial", 12);

        double yPoint = 40;

        // Titel
        gfx.DrawString("Order", titleFont, XBrushes.Black, new XRect(0, yPoint, page.Width, 40), XStringFormats.TopCenter);
        yPoint += 60;

        // Tabel headers
        gfx.DrawString("Product", headerFont, XBrushes.Black, 40, yPoint);
        gfx.DrawString("Aantal", headerFont, XBrushes.Black, 250, yPoint);
        gfx.DrawString("Prijs", headerFont, XBrushes.Black, 350, yPoint);
        yPoint += 25;
        
        foreach (var kvp in wallet)
        {
            var product = kvp.Key;
            int aantal = kvp.Value;

            gfx.DrawString(product.Name, bodyFont, XBrushes.Black, 40, yPoint);
            gfx.DrawString(aantal.ToString(), bodyFont, XBrushes.Black, 250, yPoint);
            gfx.DrawString($"€{product.Price * aantal:F2}", bodyFont, XBrushes.Black, 350, yPoint);
            yPoint += 20;
        }

        // Totale prijs
        double total = wallet.Sum(x => x.Key.Price * x.Value);
        yPoint += 20;
        gfx.DrawString($"Totaal: €{total:F2}", headerFont, XBrushes.Black, 40, yPoint);

        // Opslaan
        string filePath = Path.Combine(Path.GetTempPath(), "order.pdf");
        document.Save(filePath);
        Console.WriteLine($"PDF opgeslagen: {filePath}");
    }

    private void MakeOrder(object sender, RoutedEventArgs e)
    {
        GenerateOrder();
        var button = (Button)sender;
        var db = new AppDbContext();

        bool found = false;
        var products = db.Products.ToList();

        foreach (var product in products)
        {
            foreach (var kvp in wallet.ToList())
            {
                var existingProduct = kvp.Key;
                if (existingProduct.Id == product.Id)
                {
                    wallet[existingProduct] += 1;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                wallet[product] = 1;
            }

            if (product.Stock > 0)
            {
                product.Stock -= 1;
            }
            }
        
        GenerateOrder();

    }
}
