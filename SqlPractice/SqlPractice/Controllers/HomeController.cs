﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SqlPractice.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if(Session["email"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index","Login");
            }
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