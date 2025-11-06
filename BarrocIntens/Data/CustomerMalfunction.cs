using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    internal class CustomerMalfunction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Required]
        public int MalfunctionId { get; set; }
        public Malfunction Malfunction { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

    }
}