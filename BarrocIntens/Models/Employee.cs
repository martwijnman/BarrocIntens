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
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vul naam in")]
        [MaxLength(280)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is verplicht")]
        [MaxLength(280)]
        public string Email { get; set; }

        [Required(ErrorMessage = "voer een wachtwoord in")]
        [MaxLength(50)]
        public string Password { get; set; }


        [Required (ErrorMessage = "Afspraak moet gemaakt worden")]
        public int AppointmentId { get; set; }

        public Appointment Appointment { get; set; }

        public List<Appointment> Appointments { get; set; }

        [Required]
        public List<Planning> tasks { get; set; }




    }
}
