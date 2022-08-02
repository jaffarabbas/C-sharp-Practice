
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiFromScratch.Models;

namespace WebApiFromScratch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        public Employee Get()
        {
            return new Employee(){ id = 1, name = "John" };
        }

        [Route("{id}")]
        public IActionResult Get(int id)
        {
           if(id == 0)
           {
               return NotFound();
           }
           return Ok(new Employee() { id = 1, name = "John" });
        }
    }
}
