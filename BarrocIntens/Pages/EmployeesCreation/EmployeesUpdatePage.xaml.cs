using BarrocIntens.Data;
using BarrocIntens.Pages.Customers;
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

namespace BarrocIntens.Pages.EmployeesCreation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EmployeesUpdatePage : Page
    {
        private bool _passwordGenerated = false;

        private Employee _employee;
        public EmployeesUpdatePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var db = new AppDbContext();
            base.OnNavigatedTo(e);

            _employee = (Employee)e.Parameter;

            // PRE-FILL FIELDS
            employeeNameTextBox.Text = _employee.Name;
            employeeEmailTextBox.Text = _employee.Email;
            employeePhoneTextBox.Text = _employee.PhoneNumber;
            employeeCityTextBox.Text = _employee.City;
            // Fill ComboBox
            var departments = db.Departments.ToList();
            employeeDepartmentComboBox.ItemsSource = departments;

            // Pre-select the employee's department
            employeeDepartmentComboBox.SelectedItem = departments
                .FirstOrDefault(d => d.Id == _employee.DepartmentId); // vervang in de toggle
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
            if (string.IsNullOrWhiteSpace(employeeNameTextBox.Text))
            {
                ErrorTextBlock.Text = "Naam mag niet leeg zijn";
                return;
            }

            if (string.IsNullOrWhiteSpace(employeeEmailTextBox.Text))
            {
                ErrorTextBlock.Text = "Email mag niet leeg zijn";
                return;
            }

            if (!IsValidPhone(employeePhoneTextBox.Text))
            {
                ErrorTextBlock.Text = "Voer een geldig telefoonnummer in.";
                return;
            }

            if (string.IsNullOrWhiteSpace(employeeCityTextBox.Text))
            {
                ErrorTextBlock.Text = "Voer een geldige stadsnaam in.";
                return;
            }

            if (employeeDepartmentComboBox.SelectedItem is not Department selectedDept)
            {
                ErrorTextBlock.Text = "Medewerkers moeten een afdeling hebben";
                return;
            }

            if (!_passwordGenerated){
                ErrorTextBlock.Text = "Medewerker moet een wachtwoord hebben";
                return;
            }

            // Get password depending on visibility
            string plainPassword = passwordVisible
                ? passwordVisibleBox.Text
                : generatePasswordBox.Password;

            // Hash it
            string hashedPassword = HashPassword(plainPassword);

            // Update the existing employee
            _employee.Name = employeeNameTextBox.Text;
            _employee.Email = employeeEmailTextBox.Text;
            _employee.Password = hashedPassword;
            _employee.PhoneNumber = employeePhoneTextBox.Text;
            _employee.City = employeeCityTextBox.Text;
            _employee.DepartmentId = 1; // vervang met toggle

            // Save changes
            using (var db = new AppDbContext())
            {
                db.Employees.Update(_employee);
                db.SaveChanges();
                Frame.Navigate(typeof(employeesView));
            }

            CheckTextBlock.Text = "Medewerker succesvol opgeslagen!";
        }




        private void GeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            string newPass = GeneratePassword(6);

            generatePasswordBox.Password = newPass;
            passwordVisibleBox.Text = newPass;

            _passwordGenerated = true;
        }

        private string GeneratePassword(int length)
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string symbols = "!@#$%^&*()_-+=<>?";

            string allChars = upper + lower + digits + symbols;
            var random = new Random();
            char[] password = new char[length];

            for (int i = 0; i < length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            return new string(password);
        }


        private bool passwordVisible = false;

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            passwordVisible = !passwordVisible;

            if (passwordVisible)
            {
                // Show TextBox, hide PasswordBox
                passwordVisibleBox.Text = generatePasswordBox.Password;
                passwordVisibleBox.Visibility = Visibility.Visible;
                generatePasswordBox.Visibility = Visibility.Collapsed;

                togglePasswordButton.Content = "🙈"; // change icon
            }
            else
            {
                // Show PasswordBox, hide TextBox
                generatePasswordBox.Password = passwordVisibleBox.Text;
                generatePasswordBox.Visibility = Visibility.Visible;
                passwordVisibleBox.Visibility = Visibility.Collapsed;

                togglePasswordButton.Content = "👁"; // change icon
            }
        }

        private void PasswordHiddenBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!passwordVisible)
            {
                passwordVisibleBox.Text = generatePasswordBox.Password;
            }
        }

        private void PasswordVisibleBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (passwordVisible)
            {
                generatePasswordBox.Password = passwordVisibleBox.Text;
            }
        }

        private string HashPassword(string password)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();

            var employee = db.Employees.Find(_employee.Id);
            if (employee == null)
            {
                ErrorTextBlock.Text = "medewerker niet gevonden.";
                return;
            }

            db.Employees.Remove(employee);
            db.SaveChanges();
            Frame.Navigate(typeof(employeesView));
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
