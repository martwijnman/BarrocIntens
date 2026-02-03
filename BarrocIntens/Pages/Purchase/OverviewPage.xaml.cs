using BarrocIntens.Data;
using LiveChartsCore;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Purchase;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class OverviewPage : Page
{
    /// <summary>
    /// I made this for the Charts
    /// </summary>
    public ISeries[] SalesSeries { get; set; }
    public ObservableCollection<double> RevenueValues { get; set; }

    // I made this to put the products in the wallet, because I need 
    private Dictionary<Data.Product, int> walletProduct = new();
    private Dictionary<Data.Matrial, int> walletMatrial = new();




    // oplossing voor refresh


    public OverviewPage()
    {
        InitializeComponent();
        LoadData();
        ApplyFiltersProducts();
        ApplyFiltersMatrials();
        DrawRevenueChart();
    }


    private void LoadData()
    {
        using var db = new AppDbContext();

        ProductListView.ItemsSource = db.Products
            //.Where(p => p.Stock < p.MinimumStock)
            .Include(p => p.Deliverer)
            .ToList();

        MatrialListView.ItemsSource = db.Matrials
            //.Where(m => m.Stock < m.MinimumStock)
            .ToList();

        DelivererListView.ItemsSource = db.Deliverers.ToList();
    }
    /// <summary>
    ///  Deze functie heb ik ontwikkeld, omdat ik veel dialogues voor deze pagina moet aanmaken.
    ///  Met deze functie hoef ik niet vaak te kopie pasten, maar kan ik de code hergebruiken.
    /// </summary>
    private async Task<bool> Confirm(string message)
    {
        var dialog = new ContentDialog
        {
            Title = "Bevestiging vereist ⚠️",
            Content = message,
            PrimaryButtonText = "Accepteren",
            SecondaryButtonText = "Annuleren",
            XamlRoot = XamlRoot
        };

        return await dialog.ShowAsync() == ContentDialogResult.Primary;
    }


    /// <summary>
    /// Dit heb ik gemaakt zodat de gebruiker naar de detailpage kan gaan, en de info van het product kan zien.
    /// Ook gebruik ik de shortcut om zo snel mogelijk te editten.
    /// </summary>

    private async void ProductListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not Data.Product product) return;
        if (XamlRoot == null) return;

        var stackPanel = new StackPanel
        {
            Spacing = 10
        };

        // Toon huidige waarden als tekst
        var infoText = new TextBlock
        {
            Text = $"Huidige waarden:",
            FontWeight = FontWeights.Bold
        };

        var currentInfo = new TextBlock
        {
            Text = $"Naam: {product.Name}\nVoorraad: {product.Stock}\nMinimum: {product.MinimumStock}",
            Margin = new Thickness(0, 0, 0, 10)
        };

        // Bewerk velden
        var nameTextBox = new TextBox
        {
            Header = "Nieuwe naam:",
            Text = product.Name,
            PlaceholderText = "Voer nieuwe naam in"
        };

        var stockTextBox = new TextBox
        {
            Header = "Nieuwe voorraad:",
            Text = product.Stock.ToString(),
            InputScope = new InputScope { Names = { new InputScopeName(InputScopeNameValue.Number) } }
        };

        var minStockTextBox = new TextBox
        {
            Header = "Nieuw minimum voorraad:",
            Text = product.MinimumStock.ToString(),
            InputScope = new InputScope { Names = { new InputScopeName(InputScopeNameValue.Number) } }
        };

        stackPanel.Children.Add(infoText);
        stackPanel.Children.Add(currentInfo);
        stackPanel.Children.Add(nameTextBox);
        stackPanel.Children.Add(stockTextBox);
        stackPanel.Children.Add(minStockTextBox);

        var dialog = new ContentDialog
        {
            Title = $"Bewerk: {product.Name}",
            Content = stackPanel,
            PrimaryButtonText = "Opslaan",
            SecondaryButtonText = "Annuleren",
            CloseButtonText = "Sluiten",
            XamlRoot = XamlRoot
        };

        dialog.PrimaryButtonClick += async (sender, args) =>
        {
            var db = new AppDbContext();
            var selectedProduct = db.Matrials.FirstOrDefault(p => p.Id == product.Id);
            // Validatie en opslaan logica hier
            if (!int.TryParse(stockTextBox.Text, out int stock))
            {
                args.Cancel = true;
                return;
            }

            selectedProduct.Name = nameTextBox.Text;
            selectedProduct.Stock = stock;
            selectedProduct.MinimumStock = int.Parse(minStockTextBox.Text);



            // Opslaan naar database
            db.SaveChanges();
        };

        await dialog.ShowAsync();
        LoadData();
    }

    private async void MatrialListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not Data.Matrial matrial) return;
        if (XamlRoot == null) return;

        var stackPanel = new StackPanel
        {
            Spacing = 10
        };

        // Toon huidige waarden als tekst
        var infoText = new TextBlock
        {
            Text = $"Huidige waarden:",
            FontWeight = FontWeights.Bold
        };

        var currentInfo = new TextBlock
        {
            Text = $"Naam: {matrial.Name}\nVoorraad: {matrial.Stock}\nMinimum: {matrial.MinimumStock}",
            Margin = new Thickness(0, 0, 0, 10)
        };

        // Bewerk velden
        var nameTextBox = new TextBox
        {
            Header = "Nieuwe naam:",
            Text = matrial.Name,
            PlaceholderText = "Voer nieuwe naam in"
        };

        var stockTextBox = new TextBox
        {
            Header = "Nieuwe voorraad:",
            Text = matrial.Stock.ToString(),
            InputScope = new InputScope { Names = { new InputScopeName(InputScopeNameValue.Number) } }
        };

        var minStockTextBox = new TextBox
        {
            Header = "Nieuw minimum voorraad:",
            Text = matrial.MinimumStock.ToString(),
            InputScope = new InputScope { Names = { new InputScopeName(InputScopeNameValue.Number) } }
        };

        stackPanel.Children.Add(infoText);
        stackPanel.Children.Add(currentInfo);
        stackPanel.Children.Add(nameTextBox);
        stackPanel.Children.Add(stockTextBox);
        stackPanel.Children.Add(minStockTextBox);

        var dialog = new ContentDialog
        {
            Title = $"Bewerk: {matrial.Name}",
            Content = stackPanel,
            PrimaryButtonText = "Opslaan",
            SecondaryButtonText = "Annuleren",
            CloseButtonText = "Sluiten",
            XamlRoot = XamlRoot
        };

        dialog.PrimaryButtonClick += async (sender, args) =>
        {
            var db = new AppDbContext();
            var selectedMatrial = db.Matrials.FirstOrDefault(m => m.Id == matrial.Id);
            // Validatie en opslaan logica hier
            if (!int.TryParse(stockTextBox.Text, out int stock))
            {
                args.Cancel = true;
                return;
            }

            selectedMatrial.Name = nameTextBox.Text;
            selectedMatrial.Stock = stock;
            selectedMatrial.MinimumStock = int.Parse(minStockTextBox.Text);



            // Opslaan naar database
            db.SaveChanges();
        };

        await dialog.ShowAsync();
        LoadData();
    }

    private async void DelivererListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not Data.Deliverer deliverer) return;
        if (XamlRoot == null) return;

        var dialog = new ContentDialog
        {
            Title = $"Leverancier: {deliverer.Name}",
            Content = $"Naam: {deliverer.Name}\n",
            CloseButtonText = "Sluiten",
            XamlRoot = XamlRoot
        };

        await dialog.ShowAsync();
    }

    /// <summary>
    ///  Deze functie heb ik ontwikkeld, omdat ik het zichtbaarder wilde maken voor de klant waar de tekorten het grootst zijn.
    /// </summary>
    private void DrawRevenueChart()
    {
        using var db = new AppDbContext();

        var shortages = db.Products
            .Where(p => p.Stock < p.MinimumStock)
            .Select(p => new
            {
                Name = p.Name,
                Shortage = p.MinimumStock - p.Stock
            })
            .Concat(
                db.Matrials
                    .Where(m => m.Stock < m.MinimumStock)
                    .Select(m => new
                    {
                        Name = m.Name,
                        Shortage = m.MinimumStock - m.Stock
                    })
            )
            .ToList();

        // Maak één serie voor alle tekorten
        StockChart.Series = new ISeries[]
        {
        new ColumnSeries<int>
        {
            Values = shortages.Select(s => s.Shortage).ToArray(),
            Name = "Voorraad tekorten", // één naam voor de hele serie
            Fill = new SolidColorPaint(SKColors.Yellow)
        }
        };

        // X-as labels zijn de product/materiaal namen
        StockChart.XAxes = new Axis[]
        {
        new Axis
        {
            Labels = shortages.Select(s => s.Name).ToArray()
        }
        };
    }

    private async void CreateProduct_Button(object sender, RoutedEventArgs e)
    {
        //Frame.Navigate(typeof(Pages.Product.CreatePage));
        // Maak de dialog
        var dialog = new ContentDialog
        {
          Title = "Nieuw Product",
          CloseButtonText = "Annuleren",
          PrimaryButtonText = "Opslaan",
          XamlRoot = this.XamlRoot
        };

        // Maak de inhoud van de dialog
        var grid = new Grid
        {
            RowDefinitions =
        {
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto },
            new RowDefinition { Height = GridLength.Auto },
        },
            Margin = new Thickness(0, 0, 0, 0)
        };

        var nameBox = new TextBox { PlaceholderText = "Naam" };
        var categoryBox = new TextBox { PlaceholderText = "Categorie" };
        var priceBox = new TextBox { PlaceholderText = "Prijs" };
        var stockBox = new TextBox { PlaceholderText = "Voorraad" };
        var minStockBox = new TextBox { PlaceholderText = "Minimale voorraad" };
        var imageBox = new TextBox { PlaceholderText = "Image pad" };

        grid.Children.Add(nameBox); Grid.SetRow(nameBox, 0);
        grid.Children.Add(categoryBox); Grid.SetRow(categoryBox, 1);
        grid.Children.Add(priceBox); Grid.SetRow(priceBox, 2);
        grid.Children.Add(stockBox); Grid.SetRow(stockBox, 3);
        grid.Children.Add(minStockBox); Grid.SetRow(minStockBox, 4);
        grid.Children.Add(imageBox); Grid.SetRow(imageBox, 5);

        dialog.Content = grid;

        // Wacht op de gebruiker
        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            using var db = new AppDbContext();
            var product = new Data.Product
            {
                Name = nameBox.Text,
                Category = categoryBox.Text,
                Price = double.TryParse(priceBox.Text, out var p) ? p : 0,
                Stock = int.TryParse(stockBox.Text, out var s) ? s : 0,
                MinimumStock = int.TryParse(minStockBox.Text, out var m) ? m : 0,
                DelivererId = 1, // optioneel, kan later dynamisch
                NotificationOutOfStock = false,
                Image = imageBox.Text
            };

            // Validatie
            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(product, context, results, true))
            {
                var errors = results.Select(r => r.ErrorMessage).ToList();
                await new ContentDialog
                {
                    Title = "Fouten",
                    Content = string.Join(Environment.NewLine, errors),
                    CloseButtonText = "Sluiten",
                    XamlRoot = this.XamlRoot
                }.ShowAsync();
                return;
            }

            db.Products.Add(product);
            db.SaveChanges();
        }
    }
    // bestellingen
    private async void OrderASpecificProduct_Button(object sender, RoutedEventArgs e)
    {
        var db = new AppDbContext();
        if (sender is not Button button) return;
        if (button.DataContext is not Data.Product product) return;
        if (XamlRoot == null) return;

        var dialog = new ContentDialog
        {
            Title = "Materiaal bestellen",
            Content = $"Wilt u 1x '{product.Name}' bestellen?",
            PrimaryButtonText = "Bestellen",
            SecondaryButtonText = "Annuleren",
            XamlRoot = XamlRoot
        };

        if (await dialog.ShowAsync() != ContentDialogResult.Primary)
            return;


        //var dbMatrial = (Data.Matrial)button.DataContext;
        var dbMatrial = db.Matrials.First(m => m.Id == product.Id);

        dbMatrial.Stock++;

        if (walletMatrial.ContainsKey(dbMatrial))
            walletMatrial[dbMatrial]++;
        else
            walletMatrial[dbMatrial] = 1;


        //dbMatrial.Stock += 1;

        //db.SaveChanges();
        LoadData();
    }


    private async void OrderASpecificMatrial_Button(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        if (button.DataContext is not Matrial matrial) return;
        if (XamlRoot == null) return;

        var dialog = new ContentDialog
        {
            Title = "Materiaal bestellen",
            Content = $"Wilt u 1x '{matrial.Name}' bestellen?",
            PrimaryButtonText = "Bestellen",
            SecondaryButtonText = "Annuleren",
            XamlRoot = XamlRoot
        };

        if (await dialog.ShowAsync() != ContentDialogResult.Primary)
            return;

        using var db = new AppDbContext();

        var dbMatrial = db.Matrials.First(m => m.Id == matrial.Id);
        dbMatrial.Stock += 1;

        db.SaveChanges();
        LoadData();
    }

    private async void OrderAll_Button(object sender, RoutedEventArgs e)
    {
        using var db = new AppDbContext();

        var tekort = db.Matrials.Where(m => m.Stock < m.MinimumStock).ToList();
        if (!tekort.Any()) return;

        if (!await Confirm("Alle voorraadtekorten aanvullen?"))
            return;

        foreach (var m in tekort)
            m.Stock = m.MinimumStock;

        await db.SaveChangesAsync();
        LoadData();
        DrawRevenueChart();
    }
    // snelkoppeling per leverancier
    private async Task OrderMaterialsAndProducts(Data.Deliverer deliverer)
    {
        if (this.XamlRoot == null)
            return;

        using var db = new AppDbContext();

        var matrials = db.Matrials
            .Where(m => m.MinimumStock > m.Stock)
            .ToList();

        var products = db.Products
            .Where(p => p.DelivererId == deliverer.Id && p.MinimumStock > p.Stock)
            .ToList();

        if (!matrials.Any() && !products.Any())
            return;

        var message = "Weet u zeker dat u de volgende items wilt bestellen?\n\n";

        if (matrials.Any())
        {
            message += "Materialen:\n";
            foreach (var matrial in matrials)
            {
                int buy = matrial.MinimumStock - matrial.Stock;
                message += $"• {matrial.Name} → bestel {buy} stuks\n";
            }
            message += "\n";
        }

        if (products.Any())
        {
            message += "Producten:\n";
            foreach (var product in products)
            {
                int buy = product.MinimumStock - product.Stock;
                message += $"• {product.Name} → bestel {buy} stuks\n";
            }
        }

        var dialog = new ContentDialog
        {
            Title = "Bestelling bevestigen ⚠️",
            Content = message,
            PrimaryButtonText = "Bestellen",
            SecondaryButtonText = "Annuleren",
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result != ContentDialogResult.Primary)
            return;

        foreach (var matrial in matrials)
        {
            int buy = matrial.MinimumStock - matrial.Stock;
            if (buy > 0)
                matrial.Stock += buy;
        }

        foreach (var product in products)
        {
            int buy = product.MinimumStock - product.Stock;
            if (buy > 0)
                product.Stock += buy;
        }

        await db.SaveChangesAsync();
        LoadData();
    }
    private async void Shortcut_Company_Button(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        if (button.DataContext is not Deliverer deliverer) return;
        if (XamlRoot == null) return;

        var dialog = new ContentDialog
        {
            Title = $"Bestellen bij {deliverer.Name}",
            Content = "Alle voorraadtekorten bij deze leverancier worden aangevuld.",
            PrimaryButtonText = "Bestellen",
            SecondaryButtonText = "Annuleren",
            XamlRoot = XamlRoot
        };

        if (await dialog.ShowAsync() != ContentDialogResult.Primary)
            return;

        using var db = new AppDbContext();

        var products = db.Products
            .Where(p => p.DelivererId == deliverer.Id && p.Stock < p.MinimumStock);

        foreach (var p in products)
            p.Stock = p.MinimumStock;

        await db.SaveChangesAsync();

        LoadData();
        DrawRevenueChart();
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
            LoadData();
        }

        // weigeren
        else
        {
            return;
        }
    }


    // uitvoering
    private void Order_AllProduct_Click(object sender, RoutedEventArgs e)
    {
        if (XamlRoot == null) return;

    var dialog = new ContentDialog
    {
        Title = "Alles bestellen",
        Content = "Wilt u alle voorraadtekorten aanvullen?",
        PrimaryButtonText = "Ja",
        SecondaryButtonText = "Nee",
        XamlRoot = XamlRoot
    };

    //if (await dialog.ShowAsync() != ContentDialogResult.Primary)
    //    return;

    using var db = new AppDbContext();

    var products = db.Products.Where(p => p.Stock < p.MinimumStock);
    foreach (var p in products)
        p.Stock = p.MinimumStock;

    db.SaveChanges();

    LoadData();

    }
    private void Order_AllMatrial_Click(object sender, RoutedEventArgs e)
    {
        if (XamlRoot == null) return;

        var dialog = new ContentDialog
        {
            Title = "Alles bestellen",
            Content = "Wilt u alle voorraadtekorten aanvullen?",
            PrimaryButtonText = "Ja",
            SecondaryButtonText = "Nee",
            XamlRoot = XamlRoot
        };

        //if (await dialog.ShowAsync() != ContentDialogResult.Primary)
        //    return;

        using var db = new AppDbContext();

        var matrials = db.Matrials.Where(m => m.Stock < m.MinimumStock);
        foreach (var m in matrials)
            m.Stock = m.MinimumStock;

        db.SaveChanges();

        LoadData();

    }

    // ik heb dit gemaakt om het bestellen makkelijker te maken , zodat iemand iets specifiek kan bestellen
    public async Task OrderProduct(Data.Product product)
    {
        if (!await Confirm($"Bestel {product.MinimumStock - product.Stock} stuks van {product.Name}?"))
            return;

        using var db = new AppDbContext();
        var dbProduct = await db.Products.FirstAsync(p => p.Id == product.Id);
        dbProduct.Stock = dbProduct.MinimumStock;

        await db.SaveChangesAsync();
        LoadData();
        DrawRevenueChart();
    }

    public async Task OrderMatrial(Matrial matrial)
    {
        if (!await Confirm($"Bestel {matrial.MinimumStock - matrial.Stock} stuks van {matrial.Name}?"))
            return;

        using var db = new AppDbContext();
        var dbMatrial = await db.Matrials.FirstAsync(m => m.Id == matrial.Id);
        dbMatrial.Stock = dbMatrial.MinimumStock;

        await db.SaveChangesAsync();
        LoadData();
        DrawRevenueChart();
    }


    /// <summary>
    ///  Deze code heb ik geschreven, omdat ik het makkelijker ga maken om te zoeken.
    /// </summary>
    private void Product_TextChanged(object sender, TextChangedEventArgs e)
    => ApplyFiltersProducts();

    private void Matrial_TextChanged(object sender, TextChangedEventArgs e)
    => ApplyFiltersMatrials();

    private void ApplyFiltersProducts()
    {
        using var db = new AppDbContext();

        var productFilter = FilterTextBoxProduct.Text?.ToLower() ?? string.Empty;

        var selectedStatus =
        (StatusComboBoxProduct.SelectedItem as ComboBoxItem)?.Content?.ToString();

        var queryProduct = db.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(productFilter))
            queryProduct = queryProduct.Where(c => c.Name.ToLower().Contains(productFilter));

        switch (selectedStatus)
        {
            case "Tekort":
                queryProduct = queryProduct.Where(m => m.Stock < m.MinimumStock);
                break;

            case "Op voorraad":
                queryProduct = queryProduct.Where(m => m.Stock >= m.MinimumStock);
                break;

            case "Alles":
            default:
                break;
        }

        ProductListView.ItemsSource = queryProduct
            .ToList();

    }

    private void ApplyFiltersMatrials()
    {
        using var db = new AppDbContext();

        var matrialFilter = FilterTextBoxMatrial.Text?.ToLower() ?? string.Empty;

        var selectedStatus =
            (StatusComboBoxMatrial.SelectedItem as ComboBoxItem)?.Content?.ToString();

        var queryMatrial = db.Matrials.AsQueryable();

        if (!string.IsNullOrWhiteSpace(matrialFilter))
            queryMatrial = queryMatrial
                .Where(c => c.Name.ToLower().Contains(matrialFilter));

        switch (selectedStatus)
        {
            case "Tekort":
                queryMatrial = queryMatrial.Where(m => m.Stock < m.MinimumStock);
                break;

            case "Op voorraad":
                queryMatrial = queryMatrial.Where(m => m.Stock >= m.MinimumStock);
                break;

            case "Alles":
            default:
                break;
        }

        MatrialListView.ItemsSource = queryMatrial.ToList();
    }

    private void StatusMatrial_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyFiltersMatrials();
    }
    private void StatusProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyFiltersProducts();
    }

    // I made this to send a notification to ask that they want to approve the order
    public string Email { get; set; }
    List<dynamic> items = new();

    private async void ApproveOrder_Button(object sender, RoutedEventArgs e)
    {
        if (XamlRoot == null) return;

        using var db = new AppDbContext();

        if (!walletProduct.Any() && !walletMatrial.Any())
            return;

        var dialog = new ContentDialog
        {
            Title = "Offerte versturen",
            Content = "Offertes worden per leverancier verstuurd.",
            PrimaryButtonText = "Versturen",
            CloseButtonText = "Annuleren",
            XamlRoot = XamlRoot
        };

        if (await dialog.ShowAsync() != ContentDialogResult.Primary)
            return;

        var items = new List<OfferItem>();

        // 🔹 producten
        foreach (var kvp in walletProduct)
        {
            items.Add(new OfferItem
            {
                DelivererId = kvp.Key.DelivererId,
                Name = kvp.Key.Name,
                Amount = kvp.Value,
                Price = kvp.Key.Price
            });
        }

        // 🔹 materialen (zelfde leverancier-id gebruiken!)
        foreach (var kvp in walletMatrial)
        {
            items.Add(new OfferItem
            {
                DelivererId = kvp.Key.DelivererId, // ← moet bestaan in Matrial
                Name = kvp.Key.Name,
                Amount = kvp.Value,
                Price = kvp.Key.Price
            });
        }

        var grouped = items.GroupBy(i => i.DelivererId);

        foreach (var group in grouped)
        {
            var deliverer = db.Deliverers.First(d => d.Id == group.Key);

            var pdfPath = MakeOfferPDF(deliverer, group.ToList());

            SendOfferEmail(deliverer.Email, pdfPath);
        }
    }
    private string MakeOfferPDF(Deliverer deliverer, List<OfferItem> items)
    {
        PdfDocument document = new PdfDocument();
        document.Info.Title = "Offerte";

        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);

        XFont font = new XFont("Segoe UI", 11);
        XFont bold = new XFont("Segoe UI", 14);

        double y = 40;

        gfx.DrawString($"Offerte voor {deliverer.Name}", bold, XBrushes.Black, 40, y);
        y += 30;

        foreach (var item in items)
        {
            gfx.DrawString(
                $"{item.Name} – {item.Amount} x €{item.Price:F2}",
                font,
                XBrushes.Black,
                40,
                y
            );
            y += 20;
        }

        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            $"offerte_{deliverer.Id}_{DateTime.Now:yyyyMMddHHmm}.pdf"
        );

        document.Save(path);
        return path;
    }
    private void SendOfferEmail(string email, string pdfPath)
    {
        var mail = new MailMessage
        {
            From = new MailAddress("noreply@barrocintens.nl"),
            Subject = "Nieuwe offerte",
            Body = "Beste leverancier,\n\nIn de bijlage vindt u een offerte.\n\nMet vriendelijke groet,\nBarrocIntens"
        };

        mail.To.Add(email);
        mail.Attachments.Add(new Attachment(pdfPath));

        var smtp = new SmtpClient("smtp.jouwdomein.nl")
        {
            Port = 587,
            EnableSsl = true,
            Credentials = new NetworkCredential("EMAIL", "WACHTWOORD")
        };

        //smtp.Send(mail);
    }


}
