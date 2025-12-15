using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    class PlanningEmployee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PlanningId { get; set; }
        public Planning Planning { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
