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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.EmployeesCreation;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class employeesCreate : Page
{
    public employeesCreate()
    {
        InitializeComponent();
    }

    private List<int> SelectedDepartmentIds = new();
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

        var db = new AppDbContext();
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

        if (string.IsNullOrWhiteSpace(generatePasswordBox.Password))
        {
            ErrorTextBlock.Text = "Voer een geldig adres in.";
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

        // Department check
        if (employeeDepartmentComboBox.SelectedItem is not ComboBoxItem departmentItem)
        {
            ErrorTextBlock.Text = "Medewerkers moeten een afdeling hebben";
            return;
        }

        // Fetch all departments to fill the dropdown
        employeeDepartmentComboBox.ItemsSource = db.Departments.ToList();


        // Get password depending on visibility
        string plainPassword = passwordVisible
            ? passwordVisibleBox.Text
            : generatePasswordBox.Password;

        // Hash it
        string hashedPassword = HashPassword(plainPassword);

        var myEmployee = new BarrocIntens.Data.Employee()
        {
            Name = employeeNameTextBox.Text,
            Email = employeeEmailTextBox.Text, // <-- your email textbox
            Password = hashedPassword,         // <-- hashed password
            PhoneNumber = employeePhoneTextBox.Text,
            City = employeeCityTextBox.Text,
            DepartmentId = 1 // voeg nog department toe
        };

        // Clear all inputs
        employeeNameTextBox.Text = "";
        passwordVisibleBox.Text = "";
        employeePhoneTextBox.Text = "";
        employeeCityTextBox.Text = "";
        employeeDepartmentComboBox.SelectedIndex = -1; // Deselects anything
        generatePasswordBox.Password = "";
        passwordVisibleBox.Text = "";
        passwordVisibleBox.Visibility = Visibility.Collapsed;
        generatePasswordBox.Visibility = Visibility.Visible;
        passwordVisible = false;

        // Optional: show a success message
        CheckTextBlock.Text = "Medewerker succesvol opgeslagen!";

        using (var dbContext = new AppDbContext())
        {
            dbContext.Employees.Add(myEmployee);
            dbContext.SaveChanges();
        }
    }




    private void GeneratePassword_Click(object sender, RoutedEventArgs e)
    {
        string newPass = GeneratePassword(6);

        generatePasswordBox.Password = newPass;
        passwordVisibleBox.Text = newPass;
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

    private void backButton_Click(object sender, RoutedEventArgs e)
    {
        Frame.GoBack();
    }
}
