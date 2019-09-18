using System;
using System.Collections.Generic;
using System.Linq;

namespace MVCDemo.Model
{
    public class CreateModel
    {
        public Employee Employee { get; set; }
        public List<Department> Depts { get; set; }
        public string DepartmentName { get; set; }
    }
}
