using BarrocIntens.Data;
using BarrocIntens.Pages.Customers;
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

namespace BarrocIntens.Pages.EmployeesCreation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class employeesView : Page
    {
        public employeesView()
        {
            InitializeComponent();

            ApplyFilters(); //Load all data beforehand
        }

        private void employeesNameSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyFilters();

        private void departmentSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyFilters();

        private void ApplyFilters()
        {
            using var db = new AppDbContext();

            var nameFilter = employeesNameSearchTextBox.Text?.ToLower() ?? string.Empty;
            var departmentFilter = departmentSearchTextBox.Text?.ToLower() ?? string.Empty;

            var query = db.Employees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(c => c.Name.ToLower().Contains(nameFilter));

            //if (!string.IsNullOrWhiteSpace(departmentFilter))
            //    query = query.Where(c => c.Department.ToLower().Contains(departmentFilter));

            employeesListView.ItemsSource = query
                .ToList();
        }

        private void employeesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var employee = (Employee)e.ClickedItem;
            Frame.Navigate(typeof(EmployeesDetailPage), employee.Id);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var selectedEmployee = (Employee)button.DataContext;

            if (selectedEmployee == null)
            {
                // This should never happen now
                return;
            }

            Frame.Navigate(typeof(EmployeesUpdatePage), selectedEmployee);
        }
    }
}
