using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using testEntity.Models;

namespace testEntity.Controllers
{
    public class HomeController : Controller
    {
        Data obj = new Data();
        public ActionResult Index()
        {
            obj.NAME = "jaffar";
            obj.age = 4;
            List<Data> lst = new List<Data>();
            lst.Add(obj);
            return View(lst);
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