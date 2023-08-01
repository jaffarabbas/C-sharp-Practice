using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SqlPractice.Models;

namespace SqlPractice.Controllers
{
    public class SalesController : Controller
    {
        // GET: Sales
        public ActionResult Index()
        {
            if (Session["email"] != null)
            {
                //populate dropdowns
                using (dbMvcEntities db = new dbMvcEntities())
                {
                    var selectCustomer = db.customers.ToList();
                    var selectItems = db.items.ToList();
                    ViewBag.CustomerList = new SelectList(selectCustomer, "id", "name");
                    ViewBag.ItemList = new SelectList(selectItems, "itemId", "iname");
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult GetItemQuantity(int itemId)
        {
            using (dbMvcEntities db = new dbMvcEntities())
            {
                // Assuming your items table has a 'quantity' column
                var item = db.items.FirstOrDefault(i => i.itemId == itemId);
                if (item != null)
                {
                    return Content(item.quantity.ToString());
                }
            }

            return Content(string.Empty);
        }

        [HttpPost]
        public ActionResult Index(sale s)
        {
            using (dbMvcEntities db = new dbMvcEntities())
            {
                db.sales.Add(s);
                var data = db.SaveChanges();
                if(data > 0)
                {
                    TempData["InsertMessage"] = "<script>alert('Sales Data Inserted')</script>";
                    return RedirectToAction("Index","Sales");
                }
                else
                {
                    TempData["ErrorMessage"] = "<script>alert('Sales Data Is Not Inserted')</script>";
                    return View();
                }
            }
        }

        public IEnumerable<sale> GetSales()
        {
            using (dbMvcEntities db = new dbMvcEntities())
            {
                var result =  db.sales
                        .Include("customer") 
                        .Include("item") 
                        .ToList();
                return result;
            }
        }

        public PartialViewResult List()
        {
            var data = GetSales();
            return PartialView(data);
        }

        public ActionResult GetItemPrice(int itemId)
        {
            using (dbMvcEntities db = new dbMvcEntities())
            {
                // Assuming your items table has a 'price' column
                var item = db.items.FirstOrDefault(i => i.itemId == itemId);
                if (item != null)
                {
                    return Content(item.price.ToString());
                }
            }

            return Content(string.Empty);
        }
    }
}