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
            apiHandler = new ApiHandler();
            var empData = apiHandler.GetData(apiPath, apiController).Select(model => new Employee { 
                id = model.id,
                name = model.name,
                age = model.age,
                designation = model.designation,
                gender = model.gender,
                salary = model.salary 
            });
            return View(empData);
        }

        public ActionResult Create(Employee employee)
        {
            //apiHandler = new ApiHandler();
            //if (apiHandler.PostData(apiPath, employee, apiController))
            //{
            //    return RedirectToAction("Index");
            //}

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(apiPath);
            var response = httpClient.PostAsXmlAsync(apiController, employee);
            response.Wait();
            var test = response.Result;
            if (test.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}