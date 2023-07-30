using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SqlPractice.Models;

namespace SqlPractice.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(customer c)
        {
            using(dbMvcEntities db = new dbMvcEntities())
            {
                if (ModelState.IsValid == true)
                {
                    var data = db.customers.FirstOrDefault(m => m.email == c.email && m.password == c.password);
                    if (data == null)
                    {
                        ViewBag.ErrorMessage = "Login Failed";
                        return View();
                    }
                    else
                    {
                        Session["id"] = data.id;
                        Session["name"] = data.name;
                        Session["email"] = data.email;
                        return RedirectToAction("Index", "Home");
                    }
                }
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index","Home");
        }
    }
}