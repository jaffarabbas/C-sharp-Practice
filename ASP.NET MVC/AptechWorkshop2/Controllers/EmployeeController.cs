using Microsoft.AspNetCore.Mvc;

namespace AptechWorkshop2.Controllers
{
    public class EmployeeController : Controller
    {
        public readonly Models.Database.EmployeeContext _context;
        public EmployeeController(Models.Database.EmployeeContext context) {
            _context = context;
        }
        [HttpGet("get")]
        public IActionResult Get() { return Ok("OK"); }
    }
}
