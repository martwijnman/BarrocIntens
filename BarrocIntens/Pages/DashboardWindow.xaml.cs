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
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            int EmployeeId = (int)e.Parameter;
            dashboardFrame.Navigate(typeof(MainPage), EmployeeId);
        }


        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item)
            {
                string tag = item.Tag.ToString();

                switch (tag)
                {
                    case "DashboardPage":
<<<<<<< HEAD:BarrocIntens/Pages/DashboardWindow.xaml.cs
                        dashboardFrame.Navigate(typeof(MainPage));
=======
                        // Home: clear frame content
                        if (ContentFrame.Content != null)
                            ContentFrame.Content = null;
>>>>>>> accounts-creation:BarrocIntens/Pages/DashboardPage.xaml.cs
                        break;

                    case "Customer.CreateCustomerPage":
<<<<<<< HEAD:BarrocIntens/Pages/DashboardWindow.xaml.cs
                        dashboardFrame.Navigate(typeof(Customers.CreateCustomerPage));
=======
                        // Navigate to customer page only if not already there
                        if (ContentFrame.Content == null || !(ContentFrame.Content is Customers.CreateCustomerPage))
                            ContentFrame.Navigate(typeof(Customers.CreateCustomerPage));
>>>>>>> accounts-creation:BarrocIntens/Pages/DashboardPage.xaml.cs
                        break;
                    case "Planning.CalenderPage":
                        dashboardFrame.Navigate(typeof(Planning.CalenderPage));
                        break;
                }
            }
        }
    }
}

