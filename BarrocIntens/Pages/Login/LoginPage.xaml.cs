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
            // login validation
            if (string.IsNullOrWhiteSpace(namebox.Text) && string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                errorText.Text = "Add your login details";
            }
            else if (string.IsNullOrWhiteSpace(namebox.Text))
            {
                errorText.Text = "Add your login details";
            }
            else if (string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                errorText.Text = "Add your login details";
            }
            else
            {
                using (var db = new Data.AppDbContext())
                {
                    // login employee
                    var loginEmployee = db.Employees
    .FirstOrDefault(e => e.Name == namebox.Text && e.Password == passwordBox.Password);


                    if (loginEmployee != null)
                    {
                        Frame.Navigate(typeof(DashboardPage));
                    }
                    else
                    {
                        errorText.Text = "Name or password is invalid.";
                    }

                }
            }


        }
    }
}
