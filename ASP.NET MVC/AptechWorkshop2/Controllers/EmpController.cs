using AptechWorkshop2.Models.Database;
using AptechWorkshop2.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AptechWorkshop2.Controllers
{
    [Route("api")]
    [ApiController]
    public class EmpController : ControllerBase
    {
        public readonly Models.Database.EmployeeContext _context;
        public EmpController(Models.Database.EmployeeContext context)
        {
            _context = context;
        }
        [HttpGet("get")]
        public IActionResult Get() {
            var emp=_context.Employees.First();
            return Ok(emp);
        }

        [HttpPost("add")]
        public IActionResult AddEmp([FromBody] EmployeeRequest EmployeeRequest) //frombody,fromheader,fromquery
        {
            var emp = new Employee { 
            EmpName= EmployeeRequest.empname,
            EmpEmail = EmployeeRequest.empemail,
            EmpCode = EmployeeRequest.empcode,
            EmpAddressId = EmployeeRequest.
            };
            return Ok();
        }
    }
}
