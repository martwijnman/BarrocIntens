using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardWindow : Page
    {
        public DashboardWindow()
        {
            InitializeComponent();
            dashboardFrame.Navigate(typeof(MainPage));
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item)
            {
                string tag = item.Tag.ToString();

                // Navigate based on the tag
                switch (tag)
                {
                    case "DashboardPage":
                        dashboardFrame.Navigate(typeof(MainPage));
                        break;
                    case "Customer.CreateCustomerPage":
                        dashboardFrame.Navigate(typeof(Customers.CreateCustomerPage));
                        break;
                    case "Planning.CalenderPage":
                        dashboardFrame.Navigate(typeof(Planning.CalenderPage));
                        break;
                }
            }
        }
    }
}
