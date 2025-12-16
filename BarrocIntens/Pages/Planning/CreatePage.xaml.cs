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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreatePage : Page
    {
        public CreatePage()
        {
            InitializeComponent();
            LoadEmployees();
        }

        // lists of workers and customersS
        private List<int> SelectedCustomerIds = new();
        private List<int> SelectedEmployeeIds = new();

        private void LoadEmployees()
        {
            using var db = new AppDbContext();
            EmployeesListBox.ItemsSource = db.Employees.ToList();
        }

        private void CreateButton(object sender, RoutedEventArgs e)
        {
            using (var db = new Data.AppDbContext())
            {
                var planning = new Data.Planning
                {
                    Date = DateOnly.FromDateTime((date.SelectedDate?.DateTime) ?? DateTime.Now),
                    Plan = PlanTextbox.Text,
                    Location = LocationTextbox.Text,
                    Description = DescriptionTextbox.Text,
                    Status = StatusCheckbox.SelectedItem?.ToString(),
                    Category = CategoryCheckbox.SelectedItem?.ToString()
                };
                db.Plannings.Add(planning);



                // making a validation
                var context = new ValidationContext(planning);
                var results = new List<ValidationResult>();

                if (!Validator.TryValidateObject(planning, context, results, true))
                {
                    var errors = new List<string>();
                    foreach (var validationResult in results)
                    {
                        errors.Add(validationResult.ErrorMessage);
                    }
                    errorText.Text = string.Join(Environment.NewLine, errors);

                }

                if (Validator.TryValidateObject(planning, context, results, true))
                {
                    db.SaveChanges();
                    // add soon the planning connection between customer and employee
                    foreach (int SelectedCustomerId in SelectedCustomerIds)
                    {
                        // making invidual customers for the planning
                        db.CostumerPlannings.Add(new CostumerPlanning
                        {
                            CustomerId = SelectedCustomerId,
                            PlanningId = planning.Id,
                        });
                        db.SaveChanges();
                    }
                    foreach (int SelectedEmployeeId in SelectedEmployeeIds)
                    {
                        // making invidual customers for the planning
                        db.PlanningEmployees.Add(new PlanningEmployee
                        {
                            EmployeeId = SelectedEmployeeId,
                            PlanningId = planning.Id,
                        });
                        db.SaveChanges();
                    }
                    Frame.Navigate(typeof(Pages.Planning.CalenderPage));
                }
            }
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            using var db = new Data.AppDbContext();
            var customers = db.Customers.ToList();
            var employees  = db.Employees.ToList();

            foreach (var customer in customers)
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

            foreach (var employee in employees)
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

        public void ToggleCustomer(object sender, RoutedEventArgs e)
        {

            using (var db = new AppDbContext())
            {
                // make the connectiontable
                // 1: list
                var btn = (ToggleButton)sender;
                var id = (int)btn.Tag;
                if (btn.IsChecked == true)
                {
                    if (!SelectedEmployeeIds.Contains(id))
                        SelectedEmployeeIds.Add(id);
                }
                else
                {
                    SelectedEmployeeIds.Remove(id);
                }
            }


        }
        public void ToggleEmployee(object sender, RoutedEventArgs e)
        {
            
            using (var db = new AppDbContext())
            {
                // make the connectiontable
                // 1: list
                var btn = (ToggleButton)sender;
                var id = (int)btn.Tag;
                if (btn.IsChecked == true)
                {
                    if (!SelectedEmployeeIds.Contains(id))
                        SelectedEmployeeIds.Add(id);
                }
                else
                {
                    SelectedEmployeeIds.Remove(id);
                }
            }


        }

        private void EmployeesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;

            if (listBox.SelectedItem is Employee selectedEmployee)
            {
                EmployeesListBox.SelectedItem = selectedEmployee.Id;
            }
        }
    }
}
