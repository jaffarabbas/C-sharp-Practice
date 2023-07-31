using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SqlPractice.Models;

namespace SqlPractice.Controllers
{
    public class SalesController : Controller
    {
        // GET: Sales
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(sale s)
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }
    }
}