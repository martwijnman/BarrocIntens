using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Models
{
    internal class Bill
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Voeg een klant toe")]
        public int employeeId { get; set; }
        public Employee Employee { get; set; }

        [Required(ErrorMessage = "Voeg een klant toe")]
        public int costumerId { get; set; }
        public Customer Customer { get; set; }

        [Required(ErrorMessage = "Voeg een bedrag toe")]
        public int totalAmount { get; set; }



    }
}
