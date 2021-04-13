using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TempDataPractice.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            TempData["Data"] = "New Data here ";
            int[] arr = { 1, 3, 4, 5, 6, 7 };
            TempData["arry"] = arr;

            TempData.Keep();
            return View();
        }

        public ActionResult About()
        {
            if (TempData["Data"] != null)
            {
                TempData["Data"].ToString();
                TempData.Keep();
            }
            return View();
        }

        public ActionResult Contact()
        {
            if (TempData["Data"] != null)
            {
                TempData["Data"].ToString();
                TempData.Keep();
            }
            return View();
        }
    }
}