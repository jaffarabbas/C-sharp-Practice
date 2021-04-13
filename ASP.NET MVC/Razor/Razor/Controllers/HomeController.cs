using Razor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Razor.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Hello";

            Employees emp = new Employees();
            emp.EmpID = "fa19-bsse-0008";
            emp.EmpName = "Jaffar";
            emp.EmpEmail = "fa19-bsse-0008@maju.edu.pk";

            ViewBag.Employee = emp;

            ViewData["Comit"] = "New COmmit";

            return View();
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