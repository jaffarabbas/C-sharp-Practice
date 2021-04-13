using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SessionPractice.Controllers
{
    public class HomeController : Controller
    {
        string[] arr = { "sdfsdf", "dsfsdf", "sdgdfgdf" };
        public ActionResult Index()
        {
            Session["first"] = "Hello Jaffar";
            Session["arr"] = arr;
            return View();
        }

        public ActionResult About()
        {
            if (Session["first"] != null || Session["arr"] !=null) 
            {
                Session["first"].ToString();
                Session["arr"] = arr;
            }
            return View();
        }

        public ActionResult Contact()
        {
            if (Session["first"] != null || Session["arr"] != null)
            {
                Session["first"].ToString();
                Session["arr"] = arr;
            }
            return View();
        }
    }
}