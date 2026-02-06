using BarrocIntens.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Linq;

namespace BarrocIntens.Pages.EmployeesCreation
{
    public sealed partial class EmployeesDetailPage : Page
    {
        private int empId;

        public EmployeesDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter == null)
            {
                // Safety: avoid blank page if something goes wrong
                employeeNameTextBlock.Text = "Geen medewerker geselecteerd";
                return;
            }

            empId = (int)e.Parameter;
            LoadEmployeeDetails();
        }

        private void LoadEmployeeDetails()
        {
            using var db = new AppDbContext();
            var employee = db.Employees
                .Where(e => e.Id == empId)
                .Include(e => e.Department)
                .Select(emp => new
                {
                    emp.Name,
                    emp.Email,
                    emp.PhoneNumber,
                    emp.City,
                    emp.Department,
                    Tasks = emp.Tasks // List<Planning>
                })
                .FirstOrDefault();

            if (employee == null)
            {
                employeeNameTextBlock.Text = "Medewerker niet gevonden";
                return;
            }

            employeeNameTextBlock.Text = $"Naam: {employee.Name}";
            employeeEmailTextBlock.Text = $"Email: {employee.Email}";
            EmployeePhoneTextBlock.Text = $"Telefoon: {employee.PhoneNumber}";
            EmployeeCityTextBlock.Text = $"Stad: {employee.City}";
            EmployeeDepartmentTextBlock.Text = employee.Department?.Name ?? "No Department";

            // Handle tasks safely
            if (employee.Tasks == null || employee.Tasks.Count == 0)
            {
                EmployeeTasksTextBlock.Text = "Taken: Geen taken toegewezen";
            }
            else
            {
                EmployeeTasksTextBlock.Text =
                    "Taken: " + string.Join(", ", employee.Tasks.Select(t => t.Plan));
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
