using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Models
{
    internal class Appointment
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Voeg een klant toe")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int EmployeeId { get; set; }
        public Employee employee { get; set; }
        
        [Required(ErrorMessage = "Zet een datum")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Zet een datum")]
        public string location { get; set; }

        [Required(ErrorMessage = "voer de notities in")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "Zet een status")]
        public bool Status { get; set; }

        [Required(ErrorMessage = "Zet een type")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Zet een tijd")]
        public int Time { get; set; }

    }
}
