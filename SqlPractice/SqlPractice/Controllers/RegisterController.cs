using SqlPractice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SqlPractice.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(customer c)
        {
            using (dbMvcEntities db = new dbMvcEntities())
            {
                if(ModelState.IsValid == true)
                {
                    db.customers.Add(c);
                    var isDone = db.SaveChanges();
                    if (isDone > 0)
                    {
                        TempData["AccountCreated"] = "<script>alert('Account Created')</script>";
                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Register Failed!!";
                        return View();
                    }
                }
            }
            return View();
        }
    }
}