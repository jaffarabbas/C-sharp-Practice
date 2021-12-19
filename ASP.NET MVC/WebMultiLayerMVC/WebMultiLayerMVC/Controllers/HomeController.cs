using DbLayer.DbOperation;
using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMultiLayerMVC.Controllers
{
    public class HomeController : Controller
    {
        EmployeeDbHandler dbHandler = null;
        public HomeController()
        {
            dbHandler = new EmployeeDbHandler();
        }
        //create empolyee
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                int id = dbHandler.AddEmployee(employee);
                if(id > 0)
                {
                    ModelState.Clear();
                    ViewBag.isSuccess = "Data Added";
                }
            }
            return View();
        }
        public ActionResult GetAllRecords()
        {
            var results = dbHandler.GetAllEmployees();
            return View(results);
        }

        public ActionResult Details(int id)
        {
            var data = dbHandler.GetEmployee(id);
            return View(data);
        }

        public ActionResult Edit(int id)
        {
            var data = dbHandler.GetEmployee(id);
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(Employee employee)
        {
            if(ModelState.IsValid == true)
            {
                dbHandler.UpdateEmployee(employee.ID, employee);
                return RedirectToAction("GetAllRecords");
            }
            return View();
        }

        public ActionResult Delete(int id)
        {
            dbHandler.DeleteEmployee(id);
            return RedirectToAction("GetAllRecords");
        }
    }
}