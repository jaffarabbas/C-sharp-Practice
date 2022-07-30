using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using APIHandler;
using WebApiCRUD.Models;

namespace WebApiCRUD.Controllers.ApiIntegration
{
    public class ApiIntegrationController : Controller
    {
        ApiHandler apiHandler;
        private string apiPath = "https://localhost:44316/api/Employee";
        private string apiController = "Employee";
        // GET: ApiIntegration
        public ActionResult Index()
        {
            apiHandler = new ApiHandler(apiPath, apiController);
            IEnumerable<Employee> empData = apiHandler.GetData<Employee>();
            return View(empData);
        }

        public ActionResult Create(Employee employee)
        {
            apiHandler = new ApiHandler(apiPath, apiController);
            if (apiHandler.PostData(employee))
            {
                return RedirectToAction("Index");
            }
            return View("Create");
        }

        public ActionResult Details(int id)
        {
            apiHandler = new ApiHandler(apiPath, apiController);
            Employee empData = apiHandler.GetData<Employee>(id);
            return View(empData);
        }

        public ActionResult Edit(Employee employee)
        {
            apiHandler = new ApiHandler(apiPath, apiController);
            if (apiHandler.PutData(employee))
            {
                return RedirectToAction("Index");
            }
            return View("Edit");
        }   
    }
}