using Microsoft.EntityFrameworkCore;
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

namespace BarrocIntens.Pages.Planning
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailPage : Page
    {
        public DetailPage()
        {
            InitializeComponent();
        }
        public int SelectedPlanningId;
        public int selectedEmployeeId;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var db = new Data.AppDbContext();
            SelectedPlanningId = (int)e.Parameter;
            var selectedPlan = db.Plannings.FirstOrDefault(p => p.Id == SelectedPlanningId);
            var selectedEmloyee = db.Employees.FirstOrDefault(e => e.Id == selectedEmployeeId);
            Plan.Text = selectedPlan.Plan;
            Date.Text =   selectedPlan.Date.ToString("dd-MM-yyyy");
            Status.Text = selectedPlan.Status;  

            // filter for customers who are linked in the planningId
            var customers = db.CostumerPlannings
            .Include(c => c.Customer)
            .Where(p => p.PlanningId == SelectedPlanningId)
            .ToList()
            .Select(c => c.Customer.Name);

            Customers.Text = string.Join(", ", customers);
        }


        private void Back(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }


        private void GoUpdate(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EditPage), SelectedPlanningId);
        }
    }
}
