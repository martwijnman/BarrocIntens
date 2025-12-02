using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
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

        public MainPage()
        {
            this.InitializeComponent();

            // --- CHART DATA ---
            SalesSeries = new ISeries[]
            {
                new LineSeries<int>
                {
                    Values = new int[]
                    {
                        60,110,70,100,75,90,80,100,70,80,120,80
                    },

                    Stroke = new SolidColorPaint(new SKColor(255, 255, 0)),
                    Fill = null,

                    LineSmoothness = 0.7,
                    GeometrySize = 10,
                    GeometryStroke = null,
                    GeometryFill = new SolidColorPaint(new SKColor(255, 255, 0))
                }
            };
        }

        // --- JOUW NAVIGATIE ---  
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
    }

}
