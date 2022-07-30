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
            var employees = modelConnection.Employees.FirstOrDefault(model => model.id == id);
            return Ok(employees);
        }

        [HttpPost]
        public IHttpActionResult PostEmployee(Employee employee)
        {
            modelConnection.Employees.Add(employee);
            modelConnection.SaveChanges();
            return Ok();
        }

        [HttpPut]
        public IHttpActionResult PutEmployee(Employee employee)
        {
            var employees = modelConnection.Employees.FirstOrDefault(model => model.id == employee.id);
            if(employees != null)
            {
                employees.id = employee.id;
                employees.name = employee.name;
                employees.gender = employee.gender;
                employees.designation = employee.designation;
                employees.age = employee.age;
                employees.salary = employee.salary;
                modelConnection.SaveChanges();
            }
            else
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
