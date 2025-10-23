using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Models
{
    internal class Appointment
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int EmployeeId { get; set; }
        public Employee employee { get; set; }
        public DateTime Date { get; set; }
        public string location { get; set; }
        public string Notes { get; set; }
        public bool Status { get; set; }
        public string Type { get; set; }
        public int Time { get; set; }
    }
}
