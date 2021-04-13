using StronglyTypeViewPractice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StronglyTypeViewPractice.Controllers
{
    public class HomeController : Controller
    {
        Employee emp = new Employee();
        Employee emp2 = new Employee();
        Employee emp3 = new Employee();
        public ActionResult Index()
        {
            emp.Id = 1;
            emp.Name = "Jaffar";
            emp.Email = "askjhdka@kjdh.com";

            emp2.Id = 1;
            emp2.Name = "Jaffar";
            emp2.Email = "askjhdka@kjdh.com";

            emp3.Id = 1;
            emp3.Name = "Jaffar";
            emp3.Email = "askjhdka@kjdh.com";

            List<Employee> employee = new List<Employee>();

            employee.Add(emp);
            employee.Add(emp2);
            employee.Add(emp3);

            return View(employee);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}