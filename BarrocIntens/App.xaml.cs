using BarrocIntens.Data;
using BarrocIntens.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;
        private Window? _dashboard;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            RestoreLoginState();


                _window = new MainWindow();
                _window.Activate();

        }

        private void RestoreLoginState()
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (settings.Values.TryGetValue("EmployeeId", out object value) && value is int id)
            {
                using var db = new AppDbContext();

                var employee = db.Employees.FirstOrDefault(e => e.Id == id);

                if (employee != null)
                {
                    Employee.SetLoggedInEmployee(employee);
                }
                else
                {
                    settings.Values.Remove("EmployeeId");
                    settings.Values["IsLoggedIn"] = false;
                }
            }
        }
    }
}
