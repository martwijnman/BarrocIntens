using BarrocIntens.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Net.Http;
using System.Net.Mail;
using System.Text.RegularExpressions;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Customers
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateCustomerPage : Page
    {
        public CreateCustomerPage()
        {
            InitializeComponent();
        }


        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^\+?[\d\s\-\(\)]{7,20}$");
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Text = "";

            // Validatie
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            { ErrorTextBlock.Text = "Naam mag niet leeg zijn"; return; }

            if (!IsValidEmail(EmailTextBox.Text))
            { ErrorTextBlock.Text = "Voer een geldig e-mail adres in."; return; }

            if (!IsValidPhone(PhoneTextBox.Text))
            { ErrorTextBlock.Text = "Voer een geldig telefoonnummer in."; return; }

            if (string.IsNullOrWhiteSpace(CityTextBox.Text))
            { ErrorTextBlock.Text = "Voer een geldige stadsnaam in."; return; }

            if (BkrNumber.Value == null)
            { ErrorTextBlock.Text = "Voer een geldig BKR-nummer in."; return; }

            bool bkrStatus = false;

            try
            {
                using var client = new HttpClient { BaseAddress = new Uri("https://bkrnumbers.free.beeceptor.com/") };
                var jsonString = await client.GetStringAsync("");

                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = System.Text.Json.JsonSerializer.Deserialize<BkrNumber[]>(jsonString, options);

                if (data != null)
                {
                    var match = Array.Find(data, b => b.BkrNumberValue == BkrNumber.Value.ToString());
                    bkrStatus = match?.Status.Equals("positive", StringComparison.OrdinalIgnoreCase) ?? false;
                }
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = "Fout bij BKR-check: " + ex.Message;
                return;
            }

            var myCustomer = new BarrocIntens.Data.Customer
            {
                Name = NameTextBox.Text,
                Email = EmailTextBox.Text,
                PhoneNumber = PhoneTextBox.Text,
                City = CityTextBox.Text,
                BkrNummer = (int)BkrNumber.Value,
                BkrStatus = bkrStatus,
            };

            try
            {
                using var dbContext = new AppDbContext();
                dbContext.Customers.Add(myCustomer);
                await dbContext.SaveChangesAsync();
                CheckTextBlock.Text = "Klant succesvol opgeslagen ✔";
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = "Fout bij opslaan in database: " + ex.Message;
            }
        }

    }
}
