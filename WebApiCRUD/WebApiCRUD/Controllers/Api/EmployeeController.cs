using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiCRUD.Models;

namespace WebApiCRUD.Controllers.Api
{
    public class EmployeeController : ApiController
    {
        DB_WEB_API_CRUDEntities modelConnection = new DB_WEB_API_CRUDEntities();

        [HttpGet]
        public IHttpActionResult GetEmployee()
        {
            List<Employee> employees = modelConnection.Employees.ToList();
            return Ok(employees);
        }

        [HttpGet]
        public IHttpActionResult GetEmployee(int id)
        {
            var employees = modelConnection.Employees.Where(model => model.id == id).FirstOrDefault();
            return Ok(employees);
        }

        [HttpPost]
        public IHttpActionResult PostEmployee(Employee employee)
        {
            modelConnection.Employees.Add(employee);
            modelConnection.SaveChanges();
            return Ok();
        }
    }
}
