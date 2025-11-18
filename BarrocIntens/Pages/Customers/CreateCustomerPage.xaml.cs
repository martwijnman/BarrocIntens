using BarrocIntens.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                ErrorTextBlock.Text = "Naam mag niet leeg zijn";
                return;
            }

            if (!IsValidEmail(EmailTextBox.Text))
            {
                ErrorTextBlock.Text = "Voer een geldig e-mail adres in.";
                return;
            }

            if (!IsValidPhone(PhoneTextBox.Text))
            {
                ErrorTextBlock.Text = "Voer een geldig telefoonnummer in.";
                return;
            }   

            if (string.IsNullOrWhiteSpace(CityTextBox.Text))
            {
                ErrorTextBlock.Text = "Voer een geldige stadsnaam in.";
                return;
            }

            if (BKRCheckBox.IsChecked == false)
            {
                ErrorTextBlock.Text = "BKR keuring moet voldaan zijn.";
                return;
            }

            else
            {
                ErrorTextBlock.Text = "";
            }

                var myCustomer = new BarrocIntens.Data.Customer()
                {
                    Name = NameTextBox.Text,
                    Email = EmailTextBox.Text,
                    PhoneNumber = PhoneTextBox.Text,
                    City = CityTextBox.Text,
                    BkrStatus = BKRCheckBox.IsChecked == true
                };

            using (var dbContext = new AppDbContext())
            {
                dbContext.Customers.Add(myCustomer);
                dbContext.SaveChanges();
            }

        }

    }
}
