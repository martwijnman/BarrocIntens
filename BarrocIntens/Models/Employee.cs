using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Models
{
    internal class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(280)]
        public string Name { get; set; }

        [Required]
        [MaxLength(280)]
        public string Email { get; set; }

        [Required]
        [MaxLength(50)]
        public string Password { get; set; }


        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public Appointment Appointment { get; set; }

        public List<Appointment> Appointments { get; set; }

        public List<Planning> tasks { get; set; }

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
                    employee.Password = password;
                    employee.AppointmentId = appointmentId;
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
