using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    internal class Malfunction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        
        [Required(ErrorMessage = "Datum is verplicht")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "voeg een beschrijving toe")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Plaats een status")]
        public bool status { get; set; }

        [Required(ErrorMessage = "plaats een actie")]
        [MaxLength(280)]
        public string Action { get; set; }

        [Required(ErrorMessage = "urgentie is verplicht")]
        [MaxLength(280)]
        public string Urgency { get; set; }

    }
}
