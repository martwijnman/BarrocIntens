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
using BarrocIntens.Pages.Planning;
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
        using var db = new Data.AppDbContext();
        var customers = db.Customers.Where(c => c.BkrStatus == true).ToList();
        CustomerCheckbox.Items.Clear();

        foreach (var customer in customers)
        {
            CustomerCheckbox.Items.Add(customer.Name);
        }
    }

    private List<int> SelectedCustomerIds = new();
    private Dictionary<Data.Product, int> wallet = new Dictionary<Data.Product, int>();
    public void ToggleCustomer(object sender, RoutedEventArgs e)
    {

        using (var db = new AppDbContext())
        {
            // make the connectiontable
            // 1: list
            var btn = (ToggleButton)sender;
            var id = (int)btn.Tag;
            if (btn.IsChecked == true)
            {
                if (!SelectedCustomerIds.Contains(id))
                    SelectedCustomerIds.Add(id);
            }
            else
            {
                SelectedCustomerIds.Remove(id);
            }
        }


    }


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
    private void SaveQuote()
    {
        if (wallet.Count == 0)
            return;

        var db = new AppDbContext();

        string selectedName = CustomerCheckbox.SelectedItem as string;

        var quote = new Quote
        {
            Items = new List<QuoteItem>(),
            CustomerId = db.Customers.FirstOrDefault(c => c.Name == selectedName).Id,
            IsAccepted = false,
            IsRejected = false,
        };

        foreach(var kvp in wallet)
{
            var product = kvp.Key;
            var aantal = kvp.Value;

            var dbProduct = db.Products.FirstOrDefault(p => p.Id == product.Id);

            if (dbProduct == null)
                continue;

            quote.Items.Add(new QuoteItem
            {
                ProductId = dbProduct.Id,
                Total = aantal
            });

            //dbProduct.Stock -= aantal;
            //if (dbProduct.Stock <= dbProduct.MinimumStock)
            //{
            //    dbProduct.NotificationOutOfStock = true;
            //}
        }

        db.Quotes.Add(quote);
        db.SaveChanges();


        Frame.Navigate(typeof(Pages.Product.OverviewPage));
    }

    private void GenerateOrder()
    {
        if (wallet.Count == 0)
            return;

        PdfDocument document = new PdfDocument();
        document.Info.Title = "Offerte BarrocIntens.";

        PdfPage page = document.AddPage();
        page.Size = PdfSharp.PageSize.A4;

        XGraphics gfx = XGraphics.FromPdfPage(page);

        // COLORS ------------------------------------------------------
        XColor accentYellow = XColor.FromArgb(255, 255, 214, 0);
        XBrush yellowBrush = new XSolidBrush(accentYellow);
        XBrush darkBrush = XBrushes.Black;
        XBrush grayBrush = XBrushes.Gray;

        // FONTS -------------------------------------------------------
        XFont titleFont = new XFont("Segoe UI", 24);
        XFont headerFont = new XFont("Segoe UI", 12);
        XFont bodyFont = new XFont("Segoe UI", 11);

        double y = 40;

        // HEADER ------------------------------------------------------
        gfx.DrawRectangle(yellowBrush, 0, 0, page.Width, 80);
        gfx.DrawString("OFFERTÉ", titleFont, XBrushes.Black,
            new XRect(0, 25, page.Width - 40, 40),
            XStringFormats.TopRight);

        y = 110;

        // INFO --------------------------------------------------------
        gfx.DrawString($"Datum: {DateTime.Now:dd-MM-yyyy}", bodyFont, grayBrush, 40, y);
        y += 30;

        // TABLE HEADER -----------------------------------------------
        gfx.DrawRectangle(yellowBrush, 40, y, page.Width - 80, 28);

        gfx.DrawString("Product", headerFont, darkBrush, 45, y + 18);
        gfx.DrawString("Aantal", headerFont, darkBrush, 260, y + 18);
        gfx.DrawString("Prijs", headerFont, darkBrush, 340, y + 18);
        gfx.DrawString("Subtotaal", headerFont, darkBrush, 430, y + 18);

        y += 34;

        double total = 0;

        // TABLE BODY --------------------------------------------------
        foreach (var kvp in wallet)
        {
            var product = kvp.Key;
            int aantal = kvp.Value;

            double subtotal = product.Price * aantal;
            total += subtotal;

            gfx.DrawString(product.Name, bodyFont, darkBrush, 45, y);
            gfx.DrawString(aantal.ToString(), bodyFont, darkBrush, 260, y);
            gfx.DrawString($"€ {product.Price:F2}", bodyFont, darkBrush, 340, y);
            gfx.DrawString($"€ {subtotal:F2}", bodyFont, darkBrush, 430, y);

            y += 22;
        }

        y += 25;

        // TOTAL -------------------------------------------------------
        gfx.DrawLine(new XPen(accentYellow, 2), 340, y, page.Width - 40, y);
        y += 14;

        gfx.DrawString("Totaal", headerFont, darkBrush, 340, y);
        gfx.DrawString($"€ {total:F2}", headerFont, darkBrush, 430, y);

        y += 40;

        // FOOTER ------------------------------------------------------
        gfx.DrawLine(XPens.LightGray, 40, page.Height - 80, page.Width - 40, page.Height - 80);
        gfx.DrawString("BarrocIntens B.V.", headerFont, darkBrush, 40, page.Height - 60);
        gfx.DrawString("www.barrocintens.nl", bodyFont, grayBrush, 40, page.Height - 42);
        gfx.DrawString("info@barrocintens.nl", bodyFont, grayBrush, 40, page.Height - 28);

        // SAVE --------------------------------------------------------
        string filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "offerte.pdf"
        );

        document.Save(filePath);
    }
    private void GoBack_Button(object sender, RoutedEventArgs e) // button nog veranderen
    {
        Frame.GoBack();
    }


    private void MakeOrder(object sender, RoutedEventArgs e)
    {
        GenerateOrder();
        var button = (Button)sender;
        var db = new AppDbContext();

        bool found = false;
        var products = db.Products.ToList();        
        GenerateOrder();
        SaveQuote();
    }
}
