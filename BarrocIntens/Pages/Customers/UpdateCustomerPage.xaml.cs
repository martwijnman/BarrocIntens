using BarrocIntens.Data;
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
using System.Net.Mail;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Customers
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UpdateCustomerPage : Page
    {
        private Customer _customer;

        private int customerId;
        public UpdateCustomerPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _customer = (Customer)e.Parameter;

            // PRE-FILL FIELDS
            NameTextBox.Text = _customer.Name;
            EmailTextBox.Text = _customer.Email;
            PhoneTextBox.Text = _customer.PhoneNumber;
            CityTextBox.Text = _customer.City;
            BKRCheckBox.IsChecked = _customer.BkrStatus;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // VALIDATION (same as create)
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

            // UPDATE EXISTING CUSTOMER
            using (var db = new AppDbContext())
            {
                var customerInDb = db.Customers.Find(_customer.Id);

                customerInDb.Name = NameTextBox.Text;
                customerInDb.Email = EmailTextBox.Text;
                customerInDb.PhoneNumber = PhoneTextBox.Text;
                customerInDb.City = CityTextBox.Text;
                customerInDb.BkrStatus = BKRCheckBox.IsChecked == true;

                db.SaveChanges();
                Frame.Navigate(typeof(CustomerView));
            }
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

        private void DeleteBox_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();

            var customer = db.Customers.Find(_customer.Id);
            if (customer == null)
            {
                ErrorTextBlock.Text = "customer niet gevonden.";
                return;
            }

            db.Customers.Remove(customer);
            db.SaveChanges();
            Frame.Navigate(typeof(CustomerView));
        }
    }
}
