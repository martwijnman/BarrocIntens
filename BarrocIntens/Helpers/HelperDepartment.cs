using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrocIntens.Helpers
{
    class HelperDepartment
    {
        public static bool HasAccess(params string[] allowedDepartments)
        {
            var employee = Data.Employee.LoggedInEmployee;

            if (employee == null)
                return false;

            return allowedDepartments.Contains(employee.Department);
        }
    }
}
