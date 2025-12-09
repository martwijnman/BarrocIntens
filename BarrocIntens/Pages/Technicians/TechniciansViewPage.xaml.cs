using BarrocIntens.Data;
using BarrocIntens.Pages.EmployeesCreation;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Technicians
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TechniciansViewPage : Page
    {
        private object query;

        public TechniciansViewPage()
        {
            InitializeComponent();

            ApplyFilters(); //Load all data beforehand
            var db = new AppDbContext();


        }

        private void TechniciansNameSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyFilters();

        private void ApplyFilters()
        {
            using var db = new AppDbContext();

            var nameFilter = TechniciansNameSearchTextBox.Text?.ToLower() ?? string.Empty;

            var query = db.Employees.Where(c => c.Department == "Maintenance").
                AsQueryable();

            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(c => c.Name.ToLower().Contains(nameFilter));

            TechniciansListView.ItemsSource = query
                .ToList();
        }

        private void TechniciansListView_ItemClick(object sender, ItemClickEventArgs e)
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
