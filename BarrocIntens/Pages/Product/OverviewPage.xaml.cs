using BarrocIntens.Data;
using BarrocIntens.Pages.Product;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using PdfSharp.Pdf;
using PdfSharp.Quality;

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
                    .Where(p => p.Category != "Tools")
                    .OrderBy(p => p.Name)
                    .ToList();

                // Use the filtered list for the UI
                ProductView.ItemsSource = AllProducts;
            }
            this.Loaded += OverviewPage_Loaded;
        }

        private Dictionary<Data.Product, int> wallet = new();

        public int TimesSelected { get; set; } = 0;

        private void OverviewPage_Loaded(object sender, RoutedEventArgs e)
        {
            Errors("emptystock");
        }


        private void PlusClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (Data.Product)button.DataContext;

            if (product.Stock <= 0)
                return;

            product.Stock--;
            product.WalletCount++;

            if (wallet.ContainsKey(product))
                wallet[product]++;
            else
                wallet[product] = 1;
            ProductView.ItemsSource = null;
            ProductView.ItemsSource = AllProducts;

        }



        private void MinClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (Data.Product)button.DataContext;

            if (!wallet.ContainsKey(product))
                return;

            wallet[product]--;
            product.Stock++;
            product.WalletCount--;

            if (wallet[product] <= 0)
                wallet.Remove(product);
            ProductView.ItemsSource = null;
            ProductView.ItemsSource = AllProducts;
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
                //Frame.Navigate(typeof(OrderPage), wallet);
                OpenOrderDialog(wallet);
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

            // Start with a query that ALREADY excludes the toolkit
            var query = db.Products
                .Where(p => p.Category != "Tools");

            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(c => c.Name.ToLower().Contains(nameFilter));

            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(c => c.Name.ToLower().Contains(nameFilter));

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




        // deze code heb ik geschrijven, omdat ik het makkelijker wilde maken om een order te maken. Zonder om naar een andere pagina te gaan
        private async void OpenOrderDialog(Dictionary<Data.Product, int> wallet)
        {
            if (wallet.Count == 0)
            {
                await new ContentDialog
                {
                    Title = "Wallet leeg",
                    Content = "U heeft nog geen producten geselecteerd!",
                    CloseButtonText = "Sluiten",
                    XamlRoot = this.XamlRoot
                }.ShowAsync();
                return;
            }

            // Database klanten ophalen
            List<Data.Customer> customers;
            using (var db = new AppDbContext())
            {
                customers = db.Customers.Where(c => c.BkrStatus == true).ToList();
            }

            // Dialog opbouwen
            var dialog = new ContentDialog
            {
                Title = "Nieuwe Order",
                CloseButtonText = "Annuleren",
                PrimaryButtonText = "Opslaan",
                XamlRoot = this.XamlRoot,
            };

            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto }, // klanten
                    new RowDefinition { Height = GridLength.Auto }, // wallet
                },
                Margin = new Thickness(0, 0, 0, 0)
            };

            // Klant selectie ComboBox
            var customerCombo = new ComboBox
            {
                PlaceholderText = "Selecteer klant",
                ItemsSource = customers.Select(c => c.Name).ToList(),
                Margin = new Thickness(0, 0, 0, 10)
            };
            grid.Children.Add(customerCombo);
            Grid.SetRow(customerCombo, 0);

            // Wallet overzicht
            var walletList = new ListView
            {
                ItemsSource = wallet.Select(kvp =>
                    $"{kvp.Key.Name}  |  Aantal: {kvp.Value}  |  Subtotaal: €{kvp.Key.Price * kvp.Value:0.00}"
                ).ToList(),
                Margin = new Thickness(0, 10, 0, 0),
                Height = 200
            };

            grid.Children.Add(walletList);
            Grid.SetRow(walletList, 1);

            dialog.Content = grid;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                string selectedName = customerCombo.SelectedItem as string;
                if (string.IsNullOrEmpty(selectedName))
                {
                    await new ContentDialog
                    {
                        Title = "Fout",
                        Content = "Selecteer een klant!",
                        CloseButtonText = "Sluiten",
                        XamlRoot = this.XamlRoot
                    }.ShowAsync();
                    return;
                }

                using var db = new AppDbContext();

                var customer = db.Customers.First(c => c.Name == selectedName);

                // Quote aanmaken
                var quote = new Quote
                {
                    CustomerId = customer.Id,
                    Items = new List<QuoteItem>(),
                    IsAccepted = false,
                    IsRejected = false
                };

                // I will also 

                foreach (var kvp in wallet)
                {
                    var product = db.Products.First(p => p.Id == kvp.Key.Id);

                    quote.Items.Add(new QuoteItem
                    {
                        ProductId = product.Id,
                        Total = kvp.Value
                    });

                    product.Stock -= kvp.Value;
                    //if (product.Stock <= product.MinimumStock)
                    //    product.NotificationOutOfStock = true;
                }

                db.Quotes.Add(quote);
                db.SaveChanges();

                await new ContentDialog
                {
                    Title = "Succes",
                    Content = $"Order voor {customer.Name} is opgeslagen!",
                    CloseButtonText = "Sluiten",
                    XamlRoot = this.XamlRoot
                }.ShowAsync();
                GenerateOrder();
            }
        }

        // I made this to show to the customer how many are in stock, and how many he selected
        // So he can see how many he can order
        public string GetWalletCount(object dataContext)
        {
            if (dataContext is Data.Product product &&
                wallet.ContainsKey(product))
            {
                return $"👜 In wallet: {wallet[product]}";
            }

            return "👜 In wallet: 0";
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
    } 
}
