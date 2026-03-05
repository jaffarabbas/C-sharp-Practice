using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Interview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        public AdminController() { }

        [HttpGet("test")]
        public IActionResult Get()
        {
            return Ok("AdminController is working!");
        }
    }
}
