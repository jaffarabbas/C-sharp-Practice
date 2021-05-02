using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace WebValidationMessage.Controllers
{
    public class HomeController : Controller
    {
        string EmailPattern = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string Username, string Password, string Email)
        {
            if (Username.Equals("") == true)
            {
                ModelState.AddModelError("Username", "Enter Required Username Feilds");
                ViewData["error"] = "*";
            }
            if (Password.Equals("") == true)
            {
                ModelState.AddModelError("Password", "Enter Required Password Feilds");
                ViewData["error"] = "*";
            }
            if (Email.Equals("") == true)
            {
                ModelState.AddModelError("Email", "Enter Required Email Feilds");
                ViewData["error"] = "*";
            }
            else
            {
                if (Regex.IsMatch(Email, EmailPattern) == false)
                {
                    ModelState.AddModelError("Email", "Invalid Email");
                    ViewData["error"] = "*";
                }
            }
            if (ModelState.IsValid == true)
            {
                ViewData["success"] = "<script>alert('Successfully Submitted')</script>";
                ModelState.Clear();
            }
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