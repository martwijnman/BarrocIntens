using BarrocIntens.Data;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class MainPage : Page
    {
        // Properties for chart series and axes
        public ISeries[] SalesSeries { get; set; }
        public Axis[] XAxes { get; set; }

        // Event handler when the page is fully loaded
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyDepartmentVisibility(); // Set button visibility based on department
        }

        public MainPage()
        {
            this.InitializeComponent(); // Initialize XAML components
            this.Loaded += MainPage_Loaded; // Attach Loaded event

            // Access local user settings
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;

            // --- CHART DATA ---
            var db = new AppDbContext(); // Open database context

            // Group quote items by product and sum totals
            var groupedData = db.QuoteItems
                .Include(q => q.Product) // Include related product data
                .GroupBy(q => q.Product.Name) // Group by product name
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotaalAantal = g.Sum(x => x.Total) // Sum total quantity
                })
                .ToList();

            // Extract product names for chart labels
            string[] productLabels = groupedData
                .Select(x => x.ProductName)
                .ToArray();

            // Extract total quantities for chart values
            int[] aantallen = groupedData
                .Select(x => x.TotaalAantal)
                .ToArray();

            // =============================
            // CREATE CHART SERIES
            // =============================
            SalesSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = aantallen, // Set chart values
                    Fill = new SolidColorPaint(SKColors.Yellow), // Column color
                    Stroke = new SolidColorPaint(SKColors.Black), // Border color

                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top, // Show labels on top
                    DataLabelsSize = 14, // Font size
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black), // Label color

                    DataLabelsFormatter = p => p.Coordinate.PrimaryValue.ToString() // Format label text
                }
            };

            // Set X-axis labels
            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = productLabels,
                    LabelsRotation = 30 // Rotate labels for readability
                }
            };

            DataContext = this; // Set data context for binding

            ApplyDepartmentVisibility(); // Set buttons visible based on employee department
        }

        // Triggered when navigating to this page
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // Example: get employee ID from navigation parameters
            //EmployeeId = (int)e.Parameter;
        }

        // Navigation button click handlers
        private void PlanningClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.Planning.CalenderPage)); // Go to Planning page
        }

        private void linkdiewerkt(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.Customers.CustomerView)); // Go to Customers page
        }

        private void EmployeeClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.EmployeesCreation.employeesView)); // Go to Employees page
        }

        private void ProductClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.Product.OverviewPage)); // Go to Products page
        }

        private void ContractClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.Contracts.TableContractsPage)); // Go to Contracts page
        }

        private void MonteurClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.Technicians.TechniciansViewPage)); // Go to Technicians page
        }

        // Helper functions to hide or show buttons
        private void HideAllButtons()
        {
            Hide(PlanningButton);
            Hide(CustomerButton);
            Hide(EmployeeButton);
            Hide(SalesButton);
            Hide(ContractButton);
            Hide(MonteurButton);
        }

        private void Hide(Button button)
        {
            button.Visibility = Visibility.Collapsed; // Make button invisible
            button.IsEnabled = false; // Disable button
        }

        private void Show(Button button)
        {
            button.Visibility = Visibility.Visible; // Make button visible
            button.IsEnabled = true; // Enable button
        }

        // Set button visibility based on employee department
        private void ApplyDepartmentVisibility()
        {
            var departmentId = Employee.LoggedInEmployee?.DepartmentId;
            if (departmentId == null)
                return;

            HideAllButtons(); // Hide all buttons first

            switch (departmentId)
            {
                case 1: // Technical department
                    Show(PlanningButton);
                    Show(MonteurButton);
                    break;

                case 2: // Customer service
                    Show(CustomerButton);
                    break;

                case 3: // Administration
                    Show(ContractButton);
                    break;

                case 4: // Sales
                    Show(SalesButton);
                    Show(CustomerButton);
                    break;

                case 5: // Management
                    Show(PlanningButton);
                    Show(CustomerButton);
                    Show(EmployeeButton);
                    Show(SalesButton);
                    Show(ContractButton);
                    Show(MonteurButton);
                    break;
            }
        }
    }
}

