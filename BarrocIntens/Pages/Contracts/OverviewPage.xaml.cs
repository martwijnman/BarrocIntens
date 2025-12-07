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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Net;
using System.Net.Mail;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Contracts;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class OverviewPage : Page
{
    public OverviewPage()
    {
        InitializeComponent();
        using (var db = new Data.AppDbContext())
        {
            QuoteListView.ItemsSource = db.Quotes
                .Include(q => q.Customer)
                .Include(q => q.Items)
                .Where(q => q.IsAccepted == false || q.IsRejected == false)
                .ToList();
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e) // accepteren
    {
        if (sender is Button btn && btn.DataContext is Quote quote)
        {
            quote.IsAccepted = true;
            MakePDFFacture(quote);
            SendEmail(quote); // pas dit aan om email te sturen. uncomment: smtp.Send(mail);
            SaveFacture(quote);
        }
    }

    private void Button_Click_1(object sender, RoutedEventArgs e) // weigeren
    {
        if (sender is Button btn && btn.DataContext is Quote quote)
        {
            quote.IsRejected = true;
            SendEmail(quote); // pas dit aan om email te sturen. uncomment: smtp.Send(mail);
        }
    }

    private void Button_Click_2(object sender, RoutedEventArgs e) // details laten zien
    {
        if (sender is Button btn && btn.DataContext is Quote quote)
        {
            ShowQuoteDetails(quote);
        }
    }






    // Accepted button
    private void SendEmail(Quote quote)
    {
        if (quote.IsAccepted == true && quote.IsRejected == false)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("sender@example.com");
            mail.To.Add("recipient@example.com");
            mail.Subject = $"Offerte {quote.Id} goedgekeurd!";
            mail.Body =
            $@"Geachte {quote.Customer.Name},

            Uw offerte met nummer {quote.Id} is succesvol goedgekeurd!
            Wij gaan direct voor u aan de slag met de verdere verwerking.

            U ontvangt binnenkort een factuur en aanvullende details over de bestelling.

            Met vriendelijke groet,  
            Team BarrocIntens";

            mail.IsBodyHtml = false; // Set to true for HTML content

            // Configure SMTP client
            SmtpClient smtp = new SmtpClient("smtp.example.com", 587);
            smtp.Credentials = new NetworkCredential("username", "password");
            smtp.EnableSsl = true;
            //smtp.Send(mail);

        }
        else if(quote.IsAccepted == false && quote.IsRejected == true)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("sender@example.com");
            mail.To.Add("recipient@example.com");
            mail.Subject = $"Offerte {quote.Id} geweigerd!";
            mail.Body =
            $@"Geachte {quote.Customer.Name},

            Uw offerte met nummer {quote.Id} is helaas afgoedgekeurd!
            U kunt een nieuwe afspraak maken met ons, dan gaan wij u spoedig mogelijk helpen.

            Sorry voor het ongemak.

            Met vriendelijke groet,  
            Team BarrocIntens";
            mail.IsBodyHtml = false; // Set to true for HTML content

            // Configure SMTP client
            SmtpClient smtp = new SmtpClient("smtp.example.com", 587);
            smtp.Credentials = new NetworkCredential("username", "password");
            smtp.EnableSsl = true;
            //smtp.Send(mail);
        }
    }
    private void SaveFacture(Quote quote)
    {
        var db = new AppDbContext();
        var quoteItems = db.QuoteItems
            .Where(qi => qi.QuoteId == quote.Id)
            .Include(qi => qi.Product)
            .ToList();
        double price = 0;

        foreach (var item in quoteItems)
        {
            price += item.Total * item.Product.Price;

        }

        db.Factures.Add(new Facture
        {
            QuoteId = quote.Id,
            TotalPrice = price
        });
        Frame.Navigate(typeof(Pages.Contracts.OverviewPage));
    }








    // pdf produceren
    private void MakePDFFacture(Quote quote)
    {
        var db = new AppDbContext();

        var quoteItems = db.QuoteItems
            .Where(qi => qi.QuoteId == quote.Id)
            .Include(qi => qi.Product)
            .ToList();

        PdfDocument document = new PdfDocument();
        document.Info.Title = $"Factuur #{quote.Id}";
        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);

        // Fonts
        XFont titleFont = new XFont("Verdana", 22);
        XFont normalFont = new XFont("Verdana", 12);
        XFont boldFont = new XFont("Verdana", 12);

        double y = 40;

        // LOGO ----------------------------------------------------------
        try
        {
            XImage logo = XImage.FromFile("/Assets/logo.png"); // <-- PAS DIT AAN NAAR JOUW LOGO
            gfx.DrawImage(logo, 20, 20, 120, 60);
        }
        catch
        {
            gfx.DrawString("BarrocIntens.", boldFont, XBrushes.Gray, new XRect(20, 20, 200, 40), XStringFormats.TopLeft);
        }

        y = 100;

        // FACTUUR TITEL ------------------------------------------------
        gfx.DrawString($"FACTUUR #{quote.Id}", titleFont, XBrushes.Black,
            new XRect(0, y, page.Width, 40), XStringFormats.TopCenter);

        y += 50;

        // BARROCINTENS INFO ---------------------------------------------------
        gfx.DrawString("Bedrijfsgegevens.", boldFont, XBrushes.Black, 20, y);
        y += 20;
        gfx.DrawString("Naam: BarrocIntens B.V.", normalFont, XBrushes.Black, 20, y);
        y += 20;
        gfx.DrawString("Email: info@barrocintens.nl", normalFont, XBrushes.Black, 20, y);
        gfx.DrawString("Email: info@barrocintens.nl", normalFont, XBrushes.Black, 20, y);
        y += 40;

        // KLANT INFO ---------------------------------------------------
        gfx.DrawString("Klantgegevens", boldFont, XBrushes.Black, 20, y);
        y += 20;
        gfx.DrawString($"Naam: {quote.Customer.Name}", normalFont, XBrushes.Black, 20, y);
        y += 20;
        gfx.DrawString($"Factuurdatum: {quote.CreatedAt:dd/MM/yyyy}", normalFont, XBrushes.Black, 20, y);
        y += 40;

        // PRODUCT TABEL ------------------------------------------------
        gfx.DrawString("Producten", boldFont, XBrushes.Black, 20, y);
        y += 25;

        // Tabel headers
        gfx.DrawRectangle(XPens.Black, 20, y, page.Width - 40, 25);
        gfx.DrawString("Product", boldFont, XBrushes.Black, new XRect(25, y + 5, 200, 20), XStringFormats.TopLeft);
        gfx.DrawString("Aantal", boldFont, XBrushes.Black, new XRect(230, y + 5, 80, 20), XStringFormats.TopLeft);
        gfx.DrawString("Prijs", boldFont, XBrushes.Black, new XRect(310, y + 5, 80, 20), XStringFormats.TopLeft);
        gfx.DrawString("Totaal", boldFont, XBrushes.Black, new XRect(390, y + 5, 100, 20), XStringFormats.TopLeft);

        y += 25;

        double totalPrice = 0;

        foreach (var item in quoteItems)
        {
            double rowTotal = item.Total * item.Product.Price;
            totalPrice += rowTotal;

            gfx.DrawRectangle(XPens.Gray, 20, y, page.Width - 40, 25);

            gfx.DrawString(item.Product.Name, normalFont, XBrushes.Black, new XRect(25, y + 5, 200, 20), XStringFormats.TopLeft);
            gfx.DrawString($"{item.Total}", normalFont, XBrushes.Black, new XRect(230, y + 5, 80, 20), XStringFormats.TopLeft);
            gfx.DrawString($"€{item.Product.Price:F2}", normalFont, XBrushes.Black, new XRect(310, y + 5, 80, 20), XStringFormats.TopLeft);
            gfx.DrawString($"€{rowTotal:F2}", normalFont, XBrushes.Black, new XRect(390, y + 5, 100, 20), XStringFormats.TopLeft);

            y += 25;
        }

        y += 20;

        // TOTALEN -------------------------------------------------------
        gfx.DrawString("Totaalbedrag:", boldFont, XBrushes.Black, 20, y);
        gfx.DrawString($"€{totalPrice:F2}", boldFont, XBrushes.Black, 150, y);
        y += 10;
        gfx.DrawString($"Van {DateOnly.FromDateTime(DateTime.Now).AddDays(5)} tot {DateOnly.FromDateTime(DateTime.Now).AddDays(370)}", boldFont, XBrushes.Gray, 5, y);

        y += 40;

        // FOOTER -------------------------------------------------------
        gfx.DrawLine(XPens.Black, 20, page.Height - 80, page.Width - 20, page.Height - 80);
        gfx.DrawString("BarrocIntens. B.V.", boldFont, XBrushes.Black, 20, page.Height - 65);
        gfx.DrawString("www.barrocintens.nl", normalFont, XBrushes.DarkGray, 20, page.Height - 45);
        gfx.DrawString("info@barrocintens.nl", normalFont, XBrushes.DarkGray, 20, page.Height - 30);

        // SAVE ---------------------------------------------------------
        string fileName = $"factuur_{quote.Id}.pdf";
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
        document.Save(path);
    }




    // detail button
    private async void ShowQuoteDetails(Quote quote)
    {
        var db = new AppDbContext();
        var quoteItems = db.QuoteItems
            .Where(qi => qi.QuoteId == quote.Id)
            .Include(qi => qi.Product)
            .ToList();

        string tableHeader = $"{"Product Naam",-20} {"Aantal",8}\n";
        string tableRows = "";

        foreach (var item in quoteItems)
        {
            tableRows += $"{item.Product.Name,-20} {item.Total,5}\n";
        }

        string quoteContent = $"Klant: {quote.Customer.Name}\nGemaakt op: {quote.CreatedAt:dd/MM/yyyy}\n\nProducten:\n{tableHeader}{tableRows}";

        var dialog = new ContentDialog
        {
            Title = "Factuur Details",
            Content = quoteContent,
            CloseButtonText = "Sluiten",
            XamlRoot = this.XamlRoot
        };

        await dialog.ShowAsync();
    }
}
