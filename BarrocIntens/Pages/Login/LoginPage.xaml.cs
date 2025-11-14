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
            

            var employee = new Employee
            {
                Email = emailbox.Text,
                Password = passwordBox.Password
            };

            var context = new ValidationContext(employee);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(employee, context, results, true))
            {
                var errors = new List<string>();
                foreach (var validationResult in results)
                {
                    errors.Add(validationResult.ErrorMessage);
                }
                errorText.Text = string.Join(Environment.NewLine, errors);
            }


            using (var db = new Data.AppDbContext())
            {
                var loginEmployee = db.Employees
                    .FirstOrDefault(e => e.Email == employee.Email && e.Password == employee.Password);

                if (loginEmployee != null)
                {
                    Frame.Navigate(typeof(DashboardWindow));
                }
            }
        }
    }
}
