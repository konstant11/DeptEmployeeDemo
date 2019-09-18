using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MVCDemo.Model;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MVCDemo.Controllers
{
    public class DepartmentController : Controller
    {
        public readonly CompanyDBContext companyDB;
        public DepartmentController(CompanyDBContext context)
        {
            companyDB = context;
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

            var dep = companyDB.Departments.Find(id);
            if (null == dep)
                return NotFound();
            DepartmentModel model = new DepartmentModel()
            {
                department = dep,
                emploees = (from e in companyDB.Employees where e.DepartmentId == id select e).ToList()
            };
            return View(model);
        }

        //Create
        [ActionName("Create")]
        [AcceptVerbs("GET")]
        public IActionResult Create()
        {
            DepartmentModel model = new DepartmentModel();
            return View(model);
        }

        [ActionName("Create")]
        [AcceptVerbs("POST")]
        public IActionResult Create(DepartmentModel dep)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (dep == null || dep.department == null || string.IsNullOrEmpty(dep.department.Name))
                return NotFound();
            int checkExist = (from e in companyDB.Departments
                              where e.Name == dep.department.Name
                              select e).Count();
            if (0 < checkExist)
                return BadRequest();
            companyDB.Departments.Add(dep.department);
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
            Department e = companyDB.Departments.Find(id);
            if (null == e)
                return NotFound();
            CleanupEmployees(id.Value);
            companyDB.Departments.Remove(e);
            companyDB.SaveChanges();
            return RedirectToAction("Index");
        }

        private void CleanupEmployees(int depid)
        {
            foreach(var e in from e in companyDB.Employees where e.DepartmentId == depid select e)
            {
                e.DepartmentId = 0;
                companyDB.Employees.Update(e);
            }
            companyDB.SaveChanges();
        }
        //Update
        [ActionName("Update")]
        [AcceptVerbs("Get")]
        public IActionResult Update(int id)
        {
            Department dep = companyDB.Departments.Find(id);
            List<Employee> emps = (from e in companyDB.Employees where e.DepartmentId == id select e).ToList();
            DepartmentModel model = new DepartmentModel()
            {
                department = dep,
                emploees = emps
            };
            return View(model);
        }

        [ActionName("Update")]
        [AcceptVerbs("POST")]
        public IActionResult Update(DepartmentModel model)
        {
            Department e = companyDB.Departments.Find(model.department.id);
            if (null == e)
                return NotFound();
        
            companyDB.Departments.Update(e);
            companyDB.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
