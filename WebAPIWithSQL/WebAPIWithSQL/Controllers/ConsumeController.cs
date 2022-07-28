using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebAPIWithSQL.Models;

namespace WebAPIWithSQL.Controllers
{
    public class ConsumeController : Controller
    {
        HttpClient httpClient = new HttpClient();

        public List<STUDENT> GetData()
        {
            httpClient.BaseAddress = new Uri("https://localhost:44309/api/StudentApi");
            var response = httpClient.GetAsync("StudentApi");
            response.Wait();
            var test = response.Result;
            if (test.IsSuccessStatusCode)
            {
                var display = test.Content.ReadAsAsync<List<STUDENT>>();
                display.Wait();
                return display.Result;
            }
            return null;
        }
        // GET: Consume
        public ActionResult Index()
        {
            return View(GetData());
        }
    }
}