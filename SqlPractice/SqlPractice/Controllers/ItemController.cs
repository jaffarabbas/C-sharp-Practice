using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SqlPractice.Models;

namespace SqlPractice.Controllers
{
    public class ItemController : Controller
    {
        // GET: Item
        public ActionResult Index()
        {
            if (Session["email"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult Index(item e)
        {
            using (dbMvcEntities db = new dbMvcEntities())
            {
                db.items.Add(e);
                var data = db.SaveChanges();
                if (data > 0)
                {
                    TempData["InsertMessage"] = "<script>alert('Sales Data Inserted')</script>";
                    return RedirectToAction("Index", "Item");
                }
                else
                {
                    TempData["ErrorMessage"] = "<script>alert('Sales Data Is Not Inserted')</script>";
                    return View();
                }
            }
        }

        public IEnumerable<item> GetItems()
        {
            using (dbMvcEntities db = new dbMvcEntities())
            {
                var result = db.items.ToList();
                return result;
            }
        }

        public PartialViewResult List()
        {
            var data = GetItems();
            return PartialView(data);
        }
    }
}