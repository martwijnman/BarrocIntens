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
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BarrocIntens.Pages.Planning
{
    public sealed partial class CreatePage : Page
    {
        public CreatePage()
        {
            InitializeComponent();
            LoadEmployees();
            LoadCustomers();
        }

        private List<int> SelectedCustomerIds = new();
        private List<int> SelectedEmployeeIds = new();

        private void LoadEmployees()
        {
            using var db = new AppDbContext();
            foreach (var employee in db.Employees.ToList())
            {
                var btn = new ToggleButton
                {
                    Content = employee.Name,
                    Tag = employee.Id,
                    Margin = new Thickness(4, 0, 4, 0)
                };
                btn.Click += ToggleEmployee;
                EmployeeSelector.Children.Add(btn);
            }
        }

        private void LoadCustomers()
        {
            using var db = new AppDbContext();
            foreach (var customer in db.Customers.ToList())
            {
                var btn = new ToggleButton
                {
                    Content = customer.Name,
                    Tag = customer.Id,
                    Margin = new Thickness(4, 0, 4, 0)
                };
                btn.Click += ToggleCustomer;
                CitizenSelector.Children.Add(btn);
            }
        }

        private void ToggleCustomer(object sender, RoutedEventArgs e)
        {
            var btn = (ToggleButton)sender;
            var id = (int)btn.Tag;

            if (btn.IsChecked == true)
            {
                if (!SelectedCustomerIds.Contains(id)) SelectedCustomerIds.Add(id);
            }
            else
            {
                SelectedCustomerIds.Remove(id);
            }
        }

        private void ToggleEmployee(object sender, RoutedEventArgs e)
        {
            var btn = (ToggleButton)sender;
            var id = (int)btn.Tag;

            if (btn.IsChecked == true)
            {
                if (!SelectedEmployeeIds.Contains(id)) SelectedEmployeeIds.Add(id);
            }
            else
            {
                SelectedEmployeeIds.Remove(id);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();

            var planning = new Data.Planning
            {
                Date = DateOnly.FromDateTime(PlanningDatePicker.SelectedDate?.DateTime ?? DateTime.Now),
                Plan = PlanningTextBox.Text,
                Location = LocationTextBox.Text,
                Description = DescriptionTextBox.Text,
                Status = StatusComboBox.SelectedItem?.ToString(),
                Category = CategoryComboBox.SelectedItem?.ToString()
            };

            var context = new ValidationContext(planning);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(planning, context, results, true))
            {
                ErrorTextBlock.Text = string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
                return;
            }

            db.Plannings.Add(planning);
            db.SaveChanges();

            // LINK CUSTOMERS & EMPLOYEES
            foreach (int customerId in SelectedCustomerIds)
            {
                db.CostumerPlannings.Add(new CostumerPlanning
                {
                    CustomerId = customerId,
                    PlanningId = planning.Id
                });
            }

            foreach (int employeeId in SelectedEmployeeIds)
            {
                db.PlanningEmployees.Add(new PlanningEmployee
                {
                    EmployeeId = employeeId,
                    PlanningId = planning.Id
                });
            }

            db.SaveChanges();
            Frame.Navigate(typeof(CalenderPage));
        }
    }
}