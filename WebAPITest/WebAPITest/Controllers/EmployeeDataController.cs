using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebAPITest.Controllers
{
    public class EmployeeDataController : ApiController
    {
        Random random = new Random();
        public List<string> employee = new List<string>();
        public EmployeeDataController()
        {
            for (int i = 0; i < 30; i++)
            {
                employee.Add(random.Next(1000).ToString());
            }
        }

        [HttpGet]
        public List<string> GetEmployee()
        {
            return employee;
        }

        [HttpGet]
        public string GetEmployee(int id)
        {
            return employee.ElementAt(id);
        }
    }
}
