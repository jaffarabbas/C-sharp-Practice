using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStringlyTypePartialViewHtmlHelper.Models;

namespace WebStringlyTypePartialViewHtmlHelper.Controllers
{
    public class Home1Controller : Controller
    {
        // GET: Home1
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Calculation calculation)
        {
            var result = calculation.num1 + calculation.num2;
            ViewBag.result = result;
            ViewData["result"] = result;
            TempData["result"] = result;
            Session["result"] = result;
            if (TempData["result"] != null)
            {
                TempData["result"] = result;
                TempData.Keep();
            }
            else
            {
                TempData["result"] = "null";
            }
            if(Session["result"] != null)
            {
                Session["result"] = result;
            }
            else
            {
                Session["result"] = "null";
            }
            return View();
        }
    }
}