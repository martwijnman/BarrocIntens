using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    internal class Employee
    {
        public static Employee LoggedInEmployee { get; private set; }
        //public static User LoggedInUser { get; set; }
        public static event EventHandler OnEmployeeLoggedIn;
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(280)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Add your Email")]
        [EmailAddress]
        [MaxLength(280)]
        public string Email { get; set; }

        [Required]
        [MaxLength(150)]
        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string City { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }



        //[Required]
        //public int DepartmentId { get; set; }
        //public Department Department { get; set; }

        public List<Planning> Tasks { get; set; }

        internal static void SetLoggedInEmployee(Employee employee)
        {
            LoggedInEmployee = employee;

            // Roep de event aan, als iemand luistert, zodat pagina's daarop kunnen reageren.
            OnEmployeeLoggedIn?.Invoke(LoggedInEmployee, EventArgs.Empty);
        }


        // CRUD operations for Planning

        // Update
        public void Update(int employeeId, string name, string email, string password, int appointmentId) // voeg lijsten nog toe
        {
            using (var db = new AppDbContext())
            {
                var employee = db.Employees.FirstOrDefault(e => e.Id == employeeId);
                if (employee != null)
                {
                    employee.Name = name;
                    employee.Email = email;
                    employee.Password = password;;
                    // voeg lijsten nog toe
                    db.SaveChanges();
                }
            }
        }

        // Delete
        public void Delete(int employeeId)
        {
            using (var db = new AppDbContext())
            {
                var employee = db.Employees.FirstOrDefault(e => e.Id == employeeId);
                if (employee != null)
                {
                    db.Employees.Remove(employee);
                    db.SaveChanges();
                }
            }
        }




    }
}
