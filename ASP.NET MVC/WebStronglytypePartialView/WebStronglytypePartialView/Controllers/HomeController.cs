using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStronglytypePartialView.Models;

namespace WebStronglytypePartialView.Controllers
{
    public class HomeController : Controller
    {
        //product model object
        List<Product> products = new List<Product>();
        //{
        //    new Product {id = 1 , name = "Show1" , price = 22.3 , image = "~/images/cart2.png"},
        //    new Product {id = 2 , name = "Show2" , price = 202.3 , image = "~/images/cart4.png"},
        //    new Product {id = 3 , name = "Show3" , price = 282.3 , image = "~/images/cart5.png"}
        //};
        public ActionResult Index()
        {
            return View(products);
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