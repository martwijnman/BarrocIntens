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
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using System.Configuration;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
            }
            LoginCheck();
            //contentFrame.Navigate(typeof(Pages.Login.LoginPage));
        }
        public void LoginCheck()
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;

            bool isLoggedIn =
                settings.Values.ContainsKey("IsLoggedIn") &&
                settings.Values["IsLoggedIn"] is bool b &&
                b;

            if (isLoggedIn == true)
            {
                contentFrame.Navigate(typeof(Pages.DashboardWindow));
            }
            else
            {
                contentFrame.Navigate(typeof(Pages.Login.LoginPage));
            }
        }
    }
}
