using System;
using System.Collections.Generic;

namespace MVCDemo.Model
{
    public class DepartmentModel
    {
        public Department department { get; set; }
        public List<Employee> emploees { get; set; }
    }
}