using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    internal class EmployeeMalfunction
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int MalfunctionId { get; set; }
        public Malfunction Malfunction { get; set; }
    }
}
