using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MVCDemo.Model
{

    public class CompanyDBContext : DbContext
    {

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        private readonly IConfiguration Configuration;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Configuration.GetConnectionString("CompanyDBConnection"));
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public CompanyDBContext(IConfiguration configuration, DbContextOptions<CompanyDBContext> options) : base(options)
                                => Configuration = configuration;

        public IEnumerable<Employee> GetEmployeesByDeptID(int id) => from Employee in Employees
                                                                     where Employee.DepartmentId == id
                                                                     select Employee;

        public int GetDepartmentIdByName(string name) => (from d in Departments where d.Name == name select d.id).FirstOrDefault();

        public Department GetDeparmentById(int id)
        {
            Department d = Departments.Find(id);
            if (null == d)
                d = new Department() { Name = "Unknown", Description = "Department is not assigned" };
            return d;
        }
        public async Task<bool> AddEmployeeRecord(Employee record)
        {
            await Employees.AddAsync(record);
            SaveChanges();
            return true;
        }


    }

    [Table("Employees")]
    public class Employee
    {

        [Key]
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int DepartmentId { get; set; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string Phone { get; set; }

    }
    [Table("Departments")]
    public class Department
    {
        [Key]
        [HiddenInput(DisplayValue = false)]
        public int id { get; set; }
        [DataType(DataType.Text)]
        [Required]
        public string Name { get; set; }
        [DataType(DataType.Text)]
        public string Description { get; set; }

    }


    public class EmployeesViewModel
    {
        public EmployeesViewModel(Employee employee, CompanyDBContext context)
        {
            EmployeeView = employee;
            var dept = context.Departments.Find(employee.DepartmentId);
            if (null == dept)
            {
                Dept = new Department() { Name = "UnAssigned", Description = "PlaceHolder for not existing department" };
            }
            else
                Dept = dept;
        }

        public Employee EmployeeView { get; set; } // OilPaint
        public Department Dept { get; set; } //Comments
    }
}