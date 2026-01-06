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
public sealed partial class OverviewQuotePage : Page
{
    public OverviewQuotePage()
    {
        InitializeComponent();
        using (var db = new Data.AppDbContext())
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (settings.Values["Role"] == "Owner")
            {
                QuoteListView.ItemsSource = db.Quotes
                .Include(q => q.Customer)
                .Include(q => q.Items)
                .Where(q => q.IsAccepted == false && q.IsRejected == false)
                .ToList();
            }
            else
            {
                QuoteListView.ItemsSource = db.Quotes
                .Include(q => q.Customer)
                .Include(q => q.Items)
                .ThenInclude(i => i.Product)
                .Where(q => !q.IsAccepted && !q.IsRejected)
                .AsEnumerable()
                .Where(q => q.Items.Sum(item => item.Total * item.Product.Price) <= 5000)
                .ToList();
            }
            
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e) // accepteren
    {
        if (sender is Button btn && btn.DataContext is Quote quote)
        {
            MakePDFFacture(quote);
            SendEmail(quote); // pas dit aan om email te sturen. uncomment: smtp.Send(mail);
            SaveFacture(quote);
        }
    }

    private void Button_Click_1(object sender, RoutedEventArgs e) // weigeren
    {
        var db = new Data.AppDbContext();
        if (sender is Button btn && btn.DataContext is Quote quote)
        {
            var quoteForSave = db.Quotes.FirstOrDefault(q => q.Id == quote.Id);
            quoteForSave.IsAccepted = false;
            db.SaveChanges();
            SendEmail(quote); // pas dit aan om email te sturen. uncomment: smtp.Send(mail);
            Frame.Navigate(typeof(Pages.Contracts.OverviewQuotePage));
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

        var quoteForSave = db.Quotes.FirstOrDefault(q => q.Id == quote.Id);
        quoteForSave.IsAccepted = true;
        db.SaveChanges();

        foreach (var item in quoteItems)
        {
            price += item.Total * item.Product.Price;

        }

        db.Factures.Add(new Facture
        {
            QuoteId = quote.Id,
            TotalPrice = price,
            IsPaid = false,
        });
        db.SaveChanges();
        Frame.Navigate(typeof(Pages.Contracts.OverviewQuotePage));
    }





    // pdf produceren
    private void MakePDFFacture(Quote quote)
    {
        using var db = new AppDbContext();

        var quoteItems = db.QuoteItems
            .Where(qi => qi.QuoteId == quote.Id)
            .Include(qi => qi.Product)
            .ToList();

        PdfDocument document = new PdfDocument();
        document.Info.Title = $"Factuur #{quote.Id}";

        PdfPage page = document.AddPage();
        page.Size = PdfSharp.PageSize.A4;

        XGraphics gfx = XGraphics.FromPdfPage(page);

        // COLORS -------------------------------------------------------
        XColor accentYellow = XColor.FromArgb(255, 255, 214, 0);
        XBrush yellowBrush = new XSolidBrush(accentYellow);
        XBrush grayBrush = XBrushes.Gray;
        XBrush darkBrush = XBrushes.Black;

        // FONTS --------------------------------------------------------
        XFont titleFont = new XFont("Segoe UI", 26);
        XFont sectionFont = new XFont("Segoe UI", 14);
        XFont normalFont = new XFont("Segoe UI", 11);
        XFont boldFont = new XFont("Segoe UI", 11);

        double y = 40;

        // HEADER -------------------------------------------------------
        gfx.DrawRectangle(yellowBrush, 0, 0, page.Width, 90);

        try
        {
            XImage logo = XImage.FromFile("Assets/logo.png");
            gfx.DrawImage(logo, 40, 20, 120, 50);
        }
        catch
        {
            gfx.DrawString("BarrocIntens.", boldFont, XBrushes.Black, 40, 50);
        }

        gfx.DrawString($"FACTUUR #{quote.Id}", titleFont, XBrushes.Black,
            new XRect(0, 30, page.Width - 40, 40), XStringFormats.TopRight);

        y = 120;

        // BEDRIJF ------------------------------------------------------
        gfx.DrawString("Bedrijfsgegevens", sectionFont, darkBrush, 40, y);
        y += 20;

        gfx.DrawString("BarrocIntens B.V.", boldFont, darkBrush, 40, y);
        y += 16;
        gfx.DrawString("info@barrocintens.nl", normalFont, grayBrush, 40, y);
        y += 30;

        // KLANT --------------------------------------------------------
        gfx.DrawString("Klantgegevens", sectionFont, darkBrush, 40, y);
        y += 20;

        gfx.DrawString($"Naam: {quote.Customer.Name}", normalFont, darkBrush, 40, y);
        y += 16;
        gfx.DrawString($"Factuurdatum: {quote.CreatedAt:dd-MM-yyyy}", normalFont, darkBrush, 40, y);
        y += 40;

        // PRODUCTEN ----------------------------------------------------
        gfx.DrawString("Overzicht producten", sectionFont, darkBrush, 40, y);
        y += 25;

        // TABEL HEADER
        gfx.DrawRectangle(yellowBrush, 40, y, page.Width - 80, 28);

        gfx.DrawString("Product", boldFont, XBrushes.Black, 45, y + 18);
        gfx.DrawString("Aantal", boldFont, XBrushes.Black, 260, y + 18);
        gfx.DrawString("Prijs", boldFont, XBrushes.Black, 340, y + 18);
        gfx.DrawString("Totaal", boldFont, XBrushes.Black, 430, y + 18);

        y += 32;

        double totalPrice = 0;

        foreach (var item in quoteItems)
        {
            double rowTotal = item.Total * item.Product.Price;
            totalPrice += rowTotal;

            gfx.DrawString(item.Product.Name, normalFont, darkBrush, 45, y);
            gfx.DrawString(item.Total.ToString(), normalFont, darkBrush, 260, y);
            gfx.DrawString($"€ {item.Product.Price:F2}", normalFont, darkBrush, 340, y);
            gfx.DrawString($"€ {rowTotal:F2}", boldFont, darkBrush, 430, y);

            y += 22;
        }

        y += 30;

        // TOTAAL -------------------------------------------------------
        gfx.DrawLine(new XPen(accentYellow, 2), 340, y, page.Width - 40, y);
        y += 14;

        gfx.DrawString("Totaalbedrag", sectionFont, darkBrush, 340, y);
        gfx.DrawString($"€ {totalPrice:F2}", sectionFont, darkBrush, 430, y);

        y += 40;

        // GELDIGHEID ---------------------------------------------------
        gfx.DrawString(
            $"Geldig van {DateOnly.FromDateTime(DateTime.Now).AddDays(5)} tot {DateOnly.FromDateTime(DateTime.Now).AddDays(370)}",
            normalFont,
            grayBrush,
            40,
            y
        );

        // FOOTER -------------------------------------------------------
        gfx.DrawLine(XPens.LightGray, 40, page.Height - 80, page.Width - 40, page.Height - 80);

        gfx.DrawString("BarrocIntens B.V.", boldFont, darkBrush, 40, page.Height - 60);
        gfx.DrawString("www.barrocintens.nl", normalFont, grayBrush, 40, page.Height - 42);
        gfx.DrawString("info@barrocintens.nl", normalFont, grayBrush, 40, page.Height - 28);

        // SAVE ---------------------------------------------------------
        string fileName = $"factuur_{quote.Id}.pdf";
        string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            fileName
        );

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
