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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item)
            {
                string tag = item.Tag.ToString();

                switch (tag)
                {
                    case "DashboardPage":
                        // Home: clear frame content
                        if (ContentFrame.Content != null)
                            ContentFrame.Content = null;
                        break;

                    case "Customers.CreateCustomerPage":
                        // Navigate to customer page only if not already there
                        if (ContentFrame.Content == null || !(ContentFrame.Content is Customers.CreateCustomerPage))
                            ContentFrame.Navigate(typeof(Customers.CreateCustomerPage));
                        break;
                    case "Planning.CalenderPage":
                        ContentFrame.Navigate(typeof(Planning.CalenderPage));
                        break;
                    case "EmployeesCreation.employeesCreate":
                        ContentFrame.Navigate(typeof(EmployeesCreation.employeesCreate));
                        break;
                    case "EmployeesCreation.employeesView":
                        ContentFrame.Navigate(typeof(EmployeesCreation.employeesView));
                        break;
                    case "Customers.CustomerView":
                        ContentFrame.Navigate(typeof(Customers.CustomerView));
                        break;
                }
            }
        }
    }
}

