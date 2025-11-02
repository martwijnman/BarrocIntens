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




    }
}
