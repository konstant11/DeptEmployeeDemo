using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MVCDemo.Model;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MVCDemo.Controllers
{
    public class EmployeeController : Controller
    {
        public readonly CompanyDBContext companyDB;
        public EmployeeController(CompanyDBContext context)
        {
            companyDB = context;
        }
        [ActionName("Intro")]
        [AcceptVerbs("GET")]
        public IActionResult Intro()
        {

            return View();
        }

        [ActionName("Index")]
        [AcceptVerbs("GET")]
        public IActionResult Index()
        {
            
            return View(companyDB);
        }
        //Retrieve
        [ActionName("Retrieve")]
        [AcceptVerbs("Get")]
        public IActionResult Retrieve(int? id)
        {
            if (null == id)
            {
                return BadRequest();
            }

            var emp = companyDB.Employees.Find(id);
            if (null == emp)
                return NotFound();
            CreateModel model = new CreateModel() { Employee = emp, DepartmentName = companyDB.GetDeparmentById(emp.DepartmentId).Name };
            return View(model);
        }

        //Create
        [ActionName("Create")]
        [AcceptVerbs("GET")]
        public IActionResult Create()
        {
            CreateModel model = new CreateModel() { Depts = (from d in companyDB.Departments select d).ToList() };
            return View(model);
        }

        [ActionName("Create")]
        [AcceptVerbs("POST")]
        public IActionResult Create(CreateModel Emp)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (Emp == null || Emp.Employee == null || string.IsNullOrEmpty( Emp.Employee.LastName))
                return NotFound();
            int checkExist = (from e in companyDB.Employees
                             where e.LastName == Emp.Employee.LastName && e.FirstName == Emp.Employee.FirstName
                             select e).Count();
            if (0 < checkExist)
                return BadRequest();
            if (Emp.DepartmentName != null)
                Emp.Employee.DepartmentId = companyDB.GetDepartmentIdByName(Emp.DepartmentName);
            companyDB.Employees.Add(Emp.Employee);
            companyDB.SaveChanges();
            return RedirectToAction("Index");
        }
        //Delete
        [ActionName("Delete")]
        [AcceptVerbs("GET")]
        public IActionResult Delete(int? id)
        {
            if (null == id)
            {
                return BadRequest();
            }
            Employee e = companyDB.Employees.Find(id);
            if (null == e)
                return NotFound();
            companyDB.Employees.Remove(e);
            companyDB.SaveChanges();
            return RedirectToAction("Index", "Employee", companyDB);
        }
        //Update
        [ActionName("Update")]
        [AcceptVerbs("Get")]
        public IActionResult Update(int id)
        {
            Employee emp = companyDB.Employees.Find(id);
            string deptName = companyDB.GetDeparmentById(emp.DepartmentId).Name;
            CreateModel model = new CreateModel() {Employee = emp, DepartmentName=deptName,
                        Depts = (from d in companyDB.Departments select d).ToList() };
            return View(model);
        }

        [ActionName("Update")]
        [AcceptVerbs("POST")]
        public IActionResult Update(CreateModel model)
        {
            Employee e = companyDB.Employees.Find(model.Employee.Id);
            if (null == e)
                return NotFound();
            if (model.DepartmentName != null)
                e.DepartmentId = companyDB.GetDepartmentIdByName(model.DepartmentName);

            companyDB.Employees.Update(e);
            companyDB.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
