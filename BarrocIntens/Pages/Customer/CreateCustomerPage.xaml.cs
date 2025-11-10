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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Customer
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
            }

            if (IsValidEmail(EmailTextBox.Text))
            {
                return;
            }
            else
            {
                ErrorTextBlock.Text = "Voer een geldig e-mail adres in.";
            }

            if (IsValidPhone(PhoneTextBox.Text))
            {
                return;
            }
            else
            {
                ErrorTextBlock.Text = "Voer een geldig telefoonnummer in.";
            }     

            if (string.IsNullOrWhiteSpace(CityTextBox.Text))
            {
                ErrorTextBlock.Text = "Voer een geldige stadsnaam in.";
            }

            if (BKRCheckBox.IsChecked == false)
            {
                ErrorTextBlock.Text = "BKR keuring moet voldaan zijn.";
            }

        }

    }
}
