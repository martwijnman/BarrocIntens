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
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using BarrocIntens.Data;
using BarrocIntens.Data.Validation;
using System.ComponentModel.DataAnnotations;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Login
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        private void LoginClick(object sender, RoutedEventArgs e)
        {


            string email = emailbox.Text?.Trim();
            string password = passwordBox.Password;


            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                errorText.Text = "Email and password are required.";
                return;
            }

            using var db = new Data.AppDbContext();


            var user = db.Employees.FirstOrDefault(e => e.Email == email);


            if (user == null ||
                string.IsNullOrWhiteSpace(user.Password) ||
                !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                errorText.Text = "Invalid username or password.";
                return;
            }


            Data.Employee.SetLoggedInEmployee(user);
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values["IsLoggedIn"] = true;
            settings.Values["Name"] = user.Name;
            settings.Values["EmployeeId"] = user.Id;
            settings.Values["Role"] = user.Department;
            settings.Values["Password"] = password;
            Frame.Navigate(typeof(DashboardWindow), user.Id);

        }
    }
}

