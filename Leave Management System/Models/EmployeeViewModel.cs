using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Leave_Management_System.Models
{
    public class EmployeeViewModel
    {
        public IEnumerable<Employee> Employees { get; set; }
        public IEnumerable<Role> Roles { get; set; }
        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<Office> Offices  { get; set; }
    }
}