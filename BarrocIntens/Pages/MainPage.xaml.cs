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
        public ISeries[] SalesSeries { get; set; }
        public Axis[] XAxes { get; set; }

        public MainPage()
        {
            this.InitializeComponent();

            // user login data
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            // Greeting
            //greetingText.Text $"Hallo, {}";

            // --- CHART DATA ---
            var db = new AppDbContext();

            var groupedData = db.QuoteItems
                .Include(q => q.Product)
                .GroupBy(q => q.Product.Name)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotaalAantal = g.Sum(x => x.Total)
                })
                .ToList();

            string[] productLabels = groupedData
                .Select(x => x.ProductName)
                .ToArray();

            int[] aantallen = groupedData
                .Select(x => x.TotaalAantal)
                .ToArray();

            // =============================
            // CHART
            // =============================
            SalesSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = aantallen,

                    Fill = new SolidColorPaint(SKColors.Yellow),
                    Stroke = new SolidColorPaint(SKColors.Black),

                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                    DataLabelsSize = 14,
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),

                    // GEEN INDEX, GEEN GEDOE
                    DataLabelsFormatter = p => p.Coordinate.PrimaryValue.ToString()
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = productLabels,
                    LabelsRotation = 30
                }
            };

            DataContext = this;
            //var LoggedInEmployee = db.Employees.FirstOrDefault(e => e.Id == EmployeeId);
            //Greeting.Text = $"Hello {LoggedInEmployee.Name}";
            ApplyDepartmentVisibility();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //EmployeeId = (int)e.Parameter;
            
        }

        private void PlanningClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.Planning.CalenderPage));
        }

        private void linkdiewerkt(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.Customers.CustomerView));
        }

        private void EmployeeClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.EmployeesCreation.employeesView));
        }

        private void ProductClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.Product.OverviewPage));
        }

        private void ContractClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.Contracts.TableContractsPage));
        }

        private void MonteurClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Pages.Technicians.TechniciansViewPage));
        }
        private void ApplyDepartmentVisibility()
        {
            var department = Employee.LoggedInEmployee?.Department;

            PlanningButton.Visibility = Visibility.Collapsed;
            CustomerButton.Visibility = Visibility.Collapsed;
            EmployeeButton.Visibility = Visibility.Collapsed;
            SalesButton.Visibility = Visibility.Collapsed;
            ContractButton.Visibility = Visibility.Collapsed;
            MonteurButton.Visibility = Visibility.Collapsed;

            switch (department)
            {
                case "Planning":
                    PlanningButton.Visibility = Visibility.Visible;
                    CustomerButton.Visibility = Visibility.Visible;
                    break;

                case "Sales":
                    SalesButton.Visibility = Visibility.Visible;
                    ContractButton.Visibility = Visibility.Visible;
                    CustomerButton.Visibility = Visibility.Visible;
                    break;

                case "HR":
                    EmployeeButton.Visibility = Visibility.Visible;
                    break;

                case "Maintenance":
                    PlanningButton.Visibility = Visibility.Visible;
                    MonteurButton.Visibility = Visibility.Visible;
                    break;

                case "Management":
                    // Management ziet alles 👑
                    PlanningButton.Visibility = Visibility.Visible;
                    CustomerButton.Visibility = Visibility.Visible;
                    EmployeeButton.Visibility = Visibility.Visible;
                    SalesButton.Visibility = Visibility.Visible;
                    ContractButton.Visibility = Visibility.Visible;
                    MonteurButton.Visibility = Visibility.Visible;
                    break;
            }
        }


    }

}
