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
using System.Diagnostics.Contracts;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static System.Runtime.InteropServices.JavaScript.JSType;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardWindow : Page
    {
        public int EmployeeId;
        public DashboardWindow()
        {
            InitializeComponent();
            // add with the facture an autofill if products are out of term
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
                    case "Customer.CustomerView":
                        dashboardFrame.Navigate(typeof(Customers.CustomerView));
                        break;
                    case "Planning.CalenderPage":
                        dashboardFrame.Navigate(typeof(Planning.CalenderPage));
                        break;
                    case "Product.OverviewPage":
                        dashboardFrame.Navigate(typeof(Product.OverviewPage));
                        break;
                    case "EmployeesCreation.employeesCreate":
                        dashboardFrame.Navigate(typeof(EmployeesCreation.employeesCreate));
                        break;
                    case "EmployeesCreation.employeesView":
                        dashboardFrame.Navigate(typeof(EmployeesCreation.employeesView));
                        break;
                    case "Customers.CustomerView":
                        dashboardFrame.Navigate(typeof(Customers.CustomerView));
                        break;

                    case "Contracts.OverviewPage":
                        dashboardFrame.Navigate(typeof(Contracts.TableContractsPage));
                        break;
                    case "Technicians.TechniciansViewPage":
                        dashboardFrame.Navigate(typeof(Technicians.TechniciansViewPage));
                        break; 
                    case "Settings":
                        dashboardFrame.Navigate(typeof(Settings.SettingsPage));
                        break;
                    case "Purchase.OverviewPage":
                        dashboardFrame.Navigate(typeof(Purchase.OverviewPage));
                        break;
                }
            }
        }
    }
}
