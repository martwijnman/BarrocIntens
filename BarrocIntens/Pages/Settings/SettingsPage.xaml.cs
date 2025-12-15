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
using System.Windows.Forms;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }
        private void ChangePassword_Button(object sender, RoutedEventArgs e)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            using var db = new Data.AppDbContext();
            var employee = db.Employees.FirstOrDefault(emp => emp.Id == (int)settings.Values["EmployeeId"]);

            if (settings.Values["Password"] != Current_Password.Password)
            {
                errorText.Text = "Huidige Wachtoord is niet correct";
            }
            employee.Password = BCrypt.Net.BCrypt.HashPassword(New_Password.Password);
            db.SaveChanges();
            //errorText.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
            errorText.Text = "Wachtwoord is succesvol gewijzigd";
        }
        private void Logout_Button(object sender, RoutedEventArgs e)
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values["IsLoggedIn"] = false;
            

            // Window sluiten nog maken
            
            var mainWindow = new MainWindow();
            mainWindow.Activate();
        }
    }
}
