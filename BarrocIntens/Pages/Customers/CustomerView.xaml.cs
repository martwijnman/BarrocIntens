using BarrocIntens.Data;
using BarrocIntens.Pages.EmployeesCreation;
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
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class CustomerView : Page
    {
        public CustomerView()
        {
            InitializeComponent();

            ApplyFilters(); //Load all data beforehand
        }

        private void customersNameSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyFilters();

        private void ApplyFilters()
        {
            using var db = new AppDbContext();

            var nameFilter = customersNameSearchTextBox.Text?.ToLower() ?? string.Empty;

            var query = db.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(c => c.Name.ToLower().Contains(nameFilter));

            customersListView.ItemsSource = query
                .ToList();
        }

        private void customersListView_ItemClick(object sender, ItemClickEventArgs c)
        {
            var customer = (BarrocIntens.Data.Customer)c.ClickedItem;
            Frame.Navigate(typeof(Customers.CustomerDetailPage), customer.Id);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var selectedCustomer = (Customer)button.DataContext;

            if (selectedCustomer == null)
            {
                // This should never happen now
                return;
            }

            Frame.Navigate(typeof(UpdateCustomerPage), selectedCustomer);
        }

        private void CreationButton_Click(object sender, RoutedEventArgs e)
        {
            OpenCreateCustomerDialog();
        }

        // create button
        private async void OpenCreateCustomerDialog()
        {
            // ContentDialog aanmaken
            var dialog = new ContentDialog
            {
                Title = "Nieuwe Klant",
                PrimaryButtonText = "Opslaan",
                CloseButtonText = "Annuleren",
                XamlRoot = this.XamlRoot
            };

            // Grid voor velden
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

            // Velden
            var nameBox = new TextBox { PlaceholderText = "Naam", Margin = new Thickness(0, 4, 0, 4) };
            var emailBox = new TextBox { PlaceholderText = "Email", Margin = new Thickness(0, 4, 0, 4) };
            var phoneBox = new TextBox { PlaceholderText = "Telefoonnummer", Margin = new Thickness(0, 4, 0, 4) };
            var cityBox = new TextBox { PlaceholderText = "Stad", Margin = new Thickness(0, 4, 0, 4) };
            var bkrNumberBox = new NumberBox { PlaceholderText = "BKR Nummer", Margin = new Thickness(0, 4, 0, 4) };

            var errorText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 4, 0, 4)
            };

            grid.Children.Add(nameBox); Grid.SetRow(nameBox, 0);
            grid.Children.Add(emailBox); Grid.SetRow(emailBox, 1);
            grid.Children.Add(phoneBox); Grid.SetRow(phoneBox, 2);
            grid.Children.Add(cityBox); Grid.SetRow(cityBox, 3);
            grid.Children.Add(bkrNumberBox); Grid.SetRow(bkrNumberBox, 4);
            grid.Children.Add(errorText); Grid.SetRow(errorText, 5);

            dialog.Content = grid;

            // Toon de dialog
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                errorText.Text = "";

                // Validatie
                if (string.IsNullOrWhiteSpace(nameBox.Text)) { errorText.Text = "Naam mag niet leeg zijn"; return; }
                if (!IsValidEmail(emailBox.Text)) { errorText.Text = "Voer een geldig e-mail adres in."; return; }
                if (!IsValidPhone(phoneBox.Text)) { errorText.Text = "Voer een geldig telefoonnummer in."; return; }
                if (string.IsNullOrWhiteSpace(cityBox.Text)) { errorText.Text = "Voer een geldige stadsnaam in."; return; }
                if (bkrNumberBox.Value == null) { errorText.Text = "Voer een geldig BKR-nummer in."; return; }

                bool bkrStatus = false;

                try
                {
                    using var client = new HttpClient { BaseAddress = new Uri("https://bkrnumbers.free.beeceptor.com/") };
                    var jsonString = await client.GetStringAsync("");
                    var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var data = System.Text.Json.JsonSerializer.Deserialize<BkrNumber[]>(jsonString, options);

                    if (data != null)
                    {
                        var match = Array.Find(data, b => b.BkrNumberValue == bkrNumberBox.Value.ToString());
                        bkrStatus = match?.Status.Equals("positive", StringComparison.OrdinalIgnoreCase) ?? false;
                    }
                }
                catch (Exception ex)
                {
                    errorText.Text = "Fout bij BKR-check: " + ex.Message;
                    return;
                }

                // Customer opslaan
                var customer = new BarrocIntens.Data.Customer
                {
                    Name = nameBox.Text,
                    Email = emailBox.Text,
                    PhoneNumber = phoneBox.Text,
                    City = cityBox.Text,
                    BkrNummer = (int)bkrNumberBox.Value,
                    BkrStatus = bkrStatus
                };

                try
                {
                    using var db = new AppDbContext();
                    db.Customers.Add(customer);
                    db.SaveChanges();

                    await new ContentDialog
                    {
                        Title = "Succes",
                        Content = $"Klant {customer.Name} is succesvol opgeslagen ✔",
                        CloseButtonText = "Sluiten",
                        XamlRoot = this.XamlRoot
                    }.ShowAsync();
                }
                catch (Exception ex)
                {
                    errorText.Text = "Fout bij opslaan in database: " + ex.Message;
                }
            }
        }

        // Helper methoden
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch { return false; }
        }

        private bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^\+?[\d\s\-\(\)]{7,20}$");
        }
    }
}